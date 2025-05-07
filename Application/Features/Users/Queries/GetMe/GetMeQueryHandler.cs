using Application.Common.Interfaces;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetMe
{
    public class GetMeQueryHandler : IRequestHandler<GetMeQuery, IResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper; // Inject Mapper nếu bạn sử dụng
        private readonly ILogger<GetMeQueryHandler> _logger;

        public GetMeQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper, // Inject Mapper
            ILogger<GetMeQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper; // Gán Mapper
            _logger = logger;
        }

        public async Task<IResult<UserDto>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy User ID từ service đã được xác thực
            var userId = _currentUserService.UserId;

            // 2. Kiểm tra xem User ID có tồn tại không (Middleware Authentication nên đảm bảo điều này)
            if (!userId.HasValue)
            {
                _logger.LogWarning("GetMeQueryHandler: Attempted to get profile for unauthenticated user.");
                // Người dùng chưa đăng nhập hoặc token không hợp lệ / không chứa claim UserId
                return Result<UserDto>.Unauthorized(); // Trả về lỗi 401
            }

            _logger.LogDebug("GetMeQueryHandler: Fetching profile for User ID {UserId}", userId.Value);

            // 3. Lấy thông tin User từ Database bằng User ID
            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);

            // 4. Kiểm tra xem User có thực sự tồn tại trong DB không
            if (user == null)
            {
                // Trường hợp lạ: User ID có trong token nhưng không có trong DB (có thể user bị xóa?)
                _logger.LogError("GetMeQueryHandler: User with ID {UserId} found in token but not in database.", userId.Value);
                // Trả về lỗi 404 vì tài nguyên (user profile) không tồn tại
                return Result<UserDto>.Failure($"User profile not found.", StatusCodes.Status404NotFound);
            }

            // 5. Map từ User Entity sang UserDto để trả về
            // Điều này đảm bảo không lộ các thông tin nhạy cảm như PasswordHash
            var userDto = _mapper.Map<UserDto>(user);
            // Nếu không dùng AutoMapper:
            // var userDto = new UserDto(
            //     user.UserId, user.Name, user.Email, user.BirthDate, user.Gender,
            //     user.HeightCm, user.WeightKg, user.CreatedAt, user.IsPremium, user.EmailVerified
            // );

            _logger.LogInformation("GetMeQueryHandler: Successfully retrieved profile for User ID {UserId}", userId.Value);

            // 6. Trả về kết quả thành công chứa UserDto
            return Result<UserDto>.Success(userDto, StatusCodes.Status200OK);
        }
    }
    }
