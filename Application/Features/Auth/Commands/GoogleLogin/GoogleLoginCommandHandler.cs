using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Google.Apis.Auth; // Thư viện xác thực Google ID Token

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;
using Application.Features.Auth.Dtos;
using AutoMapper;

namespace Application.Features.Auth.Commands.GoogleLogin
{

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, IResult<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator; // Generator JWT của bạn
        private readonly IConfiguration _configuration; // Để lấy Google Client ID
        private readonly IMapper _mapper; // Để lấy Google Client ID

        private readonly ILogger<GoogleLoginCommandHandler> _logger;
        // Inject IPasswordHasher nếu cần tạo user mới với default password (không khuyến khích)
        // private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<User> _passwordHasher;

        public GoogleLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtTokenGenerator,
            IConfiguration configuration,
            ILogger<GoogleLoginCommandHandler> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
            _logger = logger;
            _mapper= mapper;
        }

        public async Task<IResult<LoginResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 1. Xác thực Google ID Token
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    _logger.LogError("Google ClientId is not configured in Authentication:Google:ClientId.");
                    return Result<LoginResponseDto>.Failure("Google authentication is not configured correctly.", StatusCodes.Status500InternalServerError);
                }

                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId } // Token phải dành cho Client ID của bạn
                                                        // Có thể thêm Issuer validation nếu cần: Issuers = new[] { "https://accounts.google.com", "accounts.google.com" }
                };

                // ValidateAsync sẽ kiểm tra chữ ký, expiry, issuer, audience
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
                // Payload chứa thông tin user: payload.Email, payload.Name, payload.Subject (Google ID), payload.Picture, payload.EmailVerified

                _logger.LogInformation("Google ID Token validated successfully for email: {Email}", payload.Email);

            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID Token received.");
                // Token không hợp lệ (sai chữ ký, hết hạn, sai audience,...)
                return Result<LoginResponseDto>.Failure("Invalid Google token.", StatusCodes.Status401Unauthorized);
            }
            catch (Exception ex) // Các lỗi khác có thể xảy ra khi validate
            {
                _logger.LogError(ex, "Error validating Google ID Token.");
                return Result<LoginResponseDto>.Failure($"An error occurred during Google token validation: {ex.Message}", StatusCodes.Status500InternalServerError);
            }

            // --- Xử lý User trong hệ thống của bạn ---

            if (payload == null || string.IsNullOrEmpty(payload.Email) || !payload.EmailVerified)
            {
                _logger.LogWarning("Google token payload is invalid or email not verified. Payload: {@Payload}", payload);
                return Result<LoginResponseDto>.Failure("Could not retrieve valid user information from Google or email not verified.", StatusCodes.Status400BadRequest);
            }

            try
            {
                // 2. Tìm User bằng Email
                var user = await _unitOfWork.Users.GetByEmailAsync(payload.Email, cancellationToken);

                if (user == null)
                {
                    // 3a. User chưa tồn tại -> Tạo mới
                    _logger.LogInformation("User with email {Email} not found. Creating new user from Google profile.", payload.Email);

                    // Kiểm tra lại lần nữa bằng GoogleId đề phòng user đổi email chính
                    user = await _unitOfWork.Users.FindByGoogleIdAsync(payload.Subject, cancellationToken); // Cần thêm phương thức FindByGoogleIdAsync vào repo
                    if (user != null)
                    {
                        _logger.LogWarning("User found by GoogleId {GoogleId} but with different email {CurrentEmail}. Login denied or handle email update.", payload.Subject, user.Email);
                        // Quyết định: Từ chối đăng nhập hay cập nhật email? Tạm thời từ chối.
                        return Result<LoginResponseDto>.Failure("Account conflict. Please contact support.", StatusCodes.Status409Conflict);
                    }

                    user = User.Create(
                        payload.Name ?? payload.Email.Split('@')[0], // Lấy tên, nếu không có thì dùng phần trước @ của email
                        payload.Email,
                        null, null, null, null // Các thông tin profile khác có thể để null ban đầu
                                               // PasswordHash có thể để null hoặc tạo random (không nên dùng mật khẩu cố định)
                    );
                    user.MarkEmailAsVerified(); // Google đã xác thực email này
                    user.LinkGoogleId(payload.Subject); // Lưu Google ID

                    // Thêm các trường khác nếu có từ payload (Picture?)
                    // user.SetProfilePicture(payload.Picture); // Nếu có method này

                    await _unitOfWork.Users.AddAsync(user, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu user mới
                    _logger.LogInformation("New user created with ID {UserId} for Google email {Email}", user.UserId, user.Email);
                }
                else
                {
                    // 3b. User đã tồn tại -> Cập nhật Google ID nếu chưa có và kiểm tra trạng thái
                    _logger.LogInformation("Existing user found with ID {UserId} for email {Email}.", user.UserId, user.Email);

                    // Liên kết Google ID nếu chưa được liên kết trước đó
                    if (string.IsNullOrEmpty(user.GoogleId))
                    {
                        user.LinkGoogleId(payload.Subject);
                        // Kiểm tra xem GoogleId này đã thuộc về user khác chưa
                        var conflictingUser = await _unitOfWork.Users.FindByGoogleIdAsync(payload.Subject, cancellationToken);
                        if (conflictingUser != null && conflictingUser.UserId != user.UserId)
                        {
                            _logger.LogError("Google ID {GoogleId} is already linked to another user {ConflictingUserId}. Cannot link to user {UserId}", payload.Subject, conflictingUser.UserId, user.UserId);
                            // Không nên tự động link, yêu cầu hỗ trợ
                            return Result<LoginResponseDto>.Failure("This Google account is linked to another user profile.", StatusCodes.Status409Conflict);
                        }
                        _unitOfWork.Users.Update(user); // Đánh dấu cần update
                        await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu Google ID mới liên kết
                        _logger.LogInformation("Linked Google ID {GoogleId} to existing user {UserId}", payload.Subject, user.UserId);
                    }
                    // Kiểm tra nếu GoogleId trong DB khác với GoogleId từ token (có thể là lỗi hoặc cố tình hack?)
                    else if (user.GoogleId != payload.Subject)
                    {
                        _logger.LogWarning("Login attempt for email {Email} with Google ID {TokenGoogleId}, but database has different Google ID {DbGoogleId} linked.", payload.Email, payload.Subject, user.GoogleId);
                        // Có thể từ chối đăng nhập hoặc yêu cầu xác thực lại
                        return Result<LoginResponseDto>.Failure("Google account mismatch. Please re-authenticate.", StatusCodes.Status409Conflict);
                    }

                    // Đảm bảo email user được đánh dấu là đã xác thực
                    if (!user.EmailVerified)
                    {
                        user.MarkEmailAsVerified();
                        _unitOfWork.Users.Update(user);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Marked email as verified for existing user {UserId} during Google login.", user.UserId);
                    }
                }

                // 4. Tạo JWT Token của ứng dụng bạn cho user này
                var userDetailsDto = _mapper.Map<UserDto>(user); // Sử dụng AutoMapper
                var jwtDuration = _configuration["JwtSettings:DurationInMinutes"];

                var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtDuration)); // Token hết hạn sau 1 giờ (tùy chỉnh)

                var appAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
                var response = new LoginResponseDto { Token= appAccessToken, ExpiresAt= expires,UserDetails= userDetailsDto };


                // 5. Trả về thành công
                return Result<LoginResponseDto>.Success(response, StatusCodes.Status200OK);

            }
            catch (DbUpdateException dbEx) // Lỗi khi lưu DB (ví dụ: constraint unique GoogleId)
            {
                _logger.LogError(dbEx, "Database error during Google login/registration for email {Email}.", payload.Email);
                // Kiểm tra InnerException để biết chi tiết
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint failed: Users.GoogleId") ?? false)
                {
                    return Result<LoginResponseDto>.Failure("This Google account is already linked to another profile.", StatusCodes.Status409Conflict);
                }
                return Result<LoginResponseDto>.Failure("A database error occurred.", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex) // Lỗi không mong muốn khác
            {
                _logger.LogError(ex, "Unexpected error during Google login/registration for email {Email}.", payload.Email);
                return Result<LoginResponseDto>.Failure($"An unexpected error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
