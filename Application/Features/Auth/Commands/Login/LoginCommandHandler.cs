using Application.Features.Auth.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Application.Common.Interfaces;
using Application.Responses.Dtos;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, IResult<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration; // Inject IConfiguration
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IMapper _mapper; // <<=== INJECT IMapper
        private readonly IJwtTokenGenerator _jwtTokenGenerator;



        public LoginCommandHandler(
             IUnitOfWork unitOfWork,
             IPasswordHasher<User> passwordHasher,
             IConfiguration configuration,
             IMapper mapper, // <<=== THÊM IMapper VÀO CONSTRUCTOR
             ILogger<LoginCommandHandler> logger,
             IJwtTokenGenerator jwtTokenGenerator
)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _mapper = mapper; // <<=== GÁN MAPPER
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<IResult<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Tìm user bằng email
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

                // 2. Kiểm tra user tồn tại và mật khẩu
                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: User not found or no password set for email {Email}", request.Email);
                    return Result<LoginResponseDto>.Failure("Invalid email or password.", (int)HttpStatusCode.Unauthorized);
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Login failed: Incorrect password for email {Email}", request.Email);
                    return Result<LoginResponseDto>.Unauthorized();
                }
                if (!user.EmailVerified)
                {
                    // Log hoặc có thể trả về thông báo đặc biệt trong Result, hoặc thêm claim vào token
                    _logger.LogWarning("User {UserId} logged in but email is not verified.", user.UserId);
                    // Có thể thêm thông báo vào Messages của Result trả về
                    // hoặc thêm claim đặc biệt vào token để client hiển thị nhắc nhở
                }


                // 3. Tạo JWT Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["JwtSettings:Key"]; // Đọc key từ config
                var jwtIssuer = _configuration["JwtSettings:Issuer"];
                var jwtAudience = _configuration["JwtSettings:Audience"];
                var jwtDuration = _configuration["JwtSettings:DurationInMinutes"];


                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
                {
                    _logger.LogError("JWT configuration (Key, Issuer, Audience) is missing or incomplete.");
                    return Result<LoginResponseDto>.Failure("Server configuration error.", (int)HttpStatusCode.InternalServerError);
                }

                var key = Encoding.UTF8.GetBytes(jwtKey);
                var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtDuration)); // Token hết hạn sau 1 giờ (tùy chỉnh)

                //    // Tạo các Claims cho token (thông tin về user)
                //    var claims = new List<Claim>
                //{
                //    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject = UserId
                //    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                //    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique Token ID
                //    // --- Thêm các claim khác nếu cần (ví dụ: role) ---
                //    // new Claim(ClaimTypes.Role, "Member"),
                //    // new Claim("CustomClaim", "CustomValue")
                //};

                //    var tokenDescriptor = new SecurityTokenDescriptor
                //    {
                //        Subject = new ClaimsIdentity(claims),
                //        Expires = expires,
                //        Issuer = jwtIssuer,
                //        Audience = jwtAudience,
                //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //    };

                //    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                //    var tokenString = tokenHandler.WriteToken(securityToken);
                var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

                var userDetailsDto = _mapper.Map<UserDto>(user); // Sử dụng AutoMapper
                // 4. Tạo response
                var response = new LoginResponseDto(accessToken, expires,userDetailsDto);

                _logger.LogInformation("User logged in successfully: UserId={UserId}, Email={Email}", user.UserId, user.Email);
                var result = Result<LoginResponseDto>.Success(response, StatusCodes.Status200OK);
                if (!user.EmailVerified)
                {
                    result.Messages.Add("Please verify your email address.");
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
                return Result<LoginResponseDto>.Failure($"An error occurred during login: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
