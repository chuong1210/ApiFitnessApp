using Application.Features.Users.Queries.GetUserById;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.CreateUser
{

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<CreateUserCommandHandler> _logger; // Optional logging

        public CreateUserCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IUserRepository userRepository,
            ILogger<CreateUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IResult<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new CreateUserCommandValidator(_userRepository);
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return Result<UserDto>.Failure(errors, StatusCodes.Status400BadRequest);
                }
                // 1. Kiểm tra Email tồn tại (nếu email được cung cấp)
                if (!string.IsNullOrEmpty(request.Email))
                {
                    bool emailExists = await _unitOfWork.Users.DoesEmailExistAsync(request.Email, cancellationToken);
                    if (emailExists)
                    {
                        _logger.LogWarning("Attempted to create user with existing email: {Email}", request.Email);
                        // Sử dụng helper methods của Result<T>
                        return Result<UserDto>.Failure($"Email '{request.Email}' already exists.", (int)HttpStatusCode.BadRequest);
                    }
                }

                // 2. Hash Password
                // Luôn hash password trước khi lưu
                string passwordHash = _passwordHasher.HashPassword(null!, request.Password); // Tham số user đầu tiên có thể null nếu không cần salt đặc biệt từ user

                // 3. Tạo User Entity
                var userEntity = User.Create(
                    request.Name,
                    request.Email,
                    request.BirthDate,
                    request.Gender,
                    request.HeightCm,
                    request.WeightKg,
                    passwordHash // Lưu hash, không lưu plain text
                );

                // 4. Add vào Repository
                await _unitOfWork.Users.AddAsync(userEntity, cancellationToken);

                // 5. Lưu thay đổi vào DB
                var changes = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (changes <= 0)
                {
                    _logger.LogError("Failed to save changes when creating user: {Email}", request.Email);
                    return Result<UserDto>.Failure("Failed to create user. Could not save changes.", (int)HttpStatusCode.InternalServerError);
                }

                // 6. Map sang DTO
                var userDto = _mapper.Map<UserDto>(userEntity);

                _logger.LogInformation("User created successfully: UserId={UserId}, Email={Email}", userDto.UserId, userDto.Email);

                // 7. Trả về kết quả thành công (201 Created)
                return Result<UserDto>.Success(userDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                _logger.LogError(ex, "Error occurred while creating user: {Email}", request.Email);
                // Trả về lỗi chung
                return Result<UserDto>.Failure($"An error occurred while creating the user: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
