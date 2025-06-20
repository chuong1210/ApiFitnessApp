using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Application.Responses.Dtos;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Application.Common.Dtos;
using Hangfire;

namespace Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
         private readonly IMapper _mapper;
        private readonly IEmailService _emailService; // Inject
        private readonly IOtpService _otpService;     // Inject
        private readonly ILogger<RegisterUserCommandHandler> _logger; // Inject
        private readonly IBackgroundJobClient _backgroundJobClient; // Inject Hangfire client

        public RegisterUserCommandHandler(
       IUnitOfWork unitOfWork,
       IPasswordHasher<User> passwordHasher,
       IEmailService emailService, // Inject
       IOtpService otpService,     // Inject
       ILogger<RegisterUserCommandHandler> logger,
       IMapper mapper,
               IBackgroundJobClient backgroundJobClient) // Inject Hangfire client
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _emailService = emailService; // Gán
            _otpService = otpService;     // Gán
            _logger = logger;             // Gán
            _backgroundJobClient = backgroundJobClient; // Gán client
            _mapper= mapper;

        }
        public async Task<IResult<int>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate input (FluentValidation nên được dùng ở pipeline behavior)
            // Ví dụ kiểm tra cơ bản:
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return await Task.FromResult(Result<int>.Failure("Password is required.", StatusCodes.Status400BadRequest));
            }

            // 2. Check if email exists
            var emailExists = await _unitOfWork.Users.DoesEmailExistAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                return await Task.FromResult(Result<int>.Failure($"Email '{request.Email}' is already registered.", StatusCodes.Status400BadRequest));
            }

            //// 3. Create User entity
            //var user = User.Create(
            //    request.Name,
            //    request.Email,
            //    request.BirthDate,
            //    request.Gender,
            //    request.HeightCm,
            //    request.WeightKg
            //// Chưa hash password ở đây
            //);

            var user=_mapper.Map<User>(request);

            // 4. Hash password
            var hashedPassword = _passwordHasher.HashPassword(user, request.Password); // Hash password cho entity user (hoặc tạo biến riêng)
                                                                                       // -> Cần một phương thức trong User entity để set PasswordHash hoặc làm trực tiếp qua reflection/cách khác nếu property là private set
                                                                                       // Giả sử User có phương thức `SetPasswordHash(string hash)`
                                                                                       // user.SetPasswordHash(hashedPassword);
                                                                                       // Hoặc nếu bạn thêm PasswordHash vào phương thức User.Create()
                                                                                       // Hoặc bạn có thể phải thiết kế lại User entity một chút để dễ dàng set hash sau khi tạo

            // --> Giả sử User.Create có thể nhận PasswordHash (Cách tốt hơn là có method riêng)
            // var user = User.Create(..., passwordHash: hashedPassword); // Nếu User.Create hỗ trợ

            // --> Cách tạm thời nếu không sửa User.Create (Không khuyến khích cho production):
            var tempUserWithHash = User.Create(request.Name, request.Email, request.BirthDate, request.Gender, request.HeightCm, request.WeightKg, hashedPassword);

            var otpCode = _otpService.GenerateOtp();

            try
            {
                // 5. Add user to repository
                await _unitOfWork.Users.AddAsync(tempUserWithHash, cancellationToken);

                // 6. Save changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 5. Lưu OTP vào Redis (dùng UserId làm key)
                //    Cần xử lý nếu bước này lỗi sau khi đã lưu user
                string otpKey = tempUserWithHash.UserId.ToString(); // Key là UserId
                await _otpService.StoreOtpAsync(otpKey, otpCode);

                EmailDto mailDto = new EmailDto(
                   tempUserWithHash.Email,
                   "Your Account Verification Code",
                   otpCode
               );

                // 6. Gửi Email chứa OTP (chạy ngầm, không cần await hoàn thành nếu không muốn block response)
                //    Không nên để lỗi gửi mail làm hỏng quá trình đăng ký
                //                _ = _emailService.SendOtpEmailAsync(
                //mailDto
                //                ); // Dùng discard (_) để chạy background task


                // --- Gửi Email bằng Hangfire ---
                // Thay vì gọi trực tiếp: await _emailService.SendOtpEmailAsync(...)
                // Chúng ta Enqueue công việc này
                var jobId = _backgroundJobClient.Enqueue<IEmailService>( // Chỉ định service sẽ thực thi
                    emailService => emailService.SendOtpEmailAsync(mailDto
                    ));


                _logger.LogInformation("Enqueued email OTP job {JobId} for User {UserId} to email {Email}.", jobId, user.UserId, user.Email);
                var userDetailsDto = _mapper.Map<UserDto>(user); // Sử dụng AutoMapper


                // 7. Return success result with UserId
                return await Task.FromResult(Result<int>.Success(tempUserWithHash.UserId, StatusCodes.Status201Created));
            }
            catch (StackExchange.Redis.RedisConnectionException redisEx) // Bắt lỗi Redis cụ thể
            {
                _logger.LogError(redisEx, "Redis connection error during registration for email {Email}.", request.Email);
                // Có thể cần xóa user vừa tạo nếu lưu OTP lỗi? (Phức tạp)
                // Hoặc chỉ báo lỗi chung
                return Result<int>.Failure("Registration failed due to a temporary issue storing verification code.", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return await Task.FromResult(Result<int>.Failure($"An error occurred during registration: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }
    }
    }
