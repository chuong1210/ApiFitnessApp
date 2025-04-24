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

namespace Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
         private readonly IMapper _mapper; 

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
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


            try
            {
                // 5. Add user to repository
                await _unitOfWork.Users.AddAsync(tempUserWithHash, cancellationToken);

                // 6. Save changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var userDetailsDto = _mapper.Map<UserDto>(user); // Sử dụng AutoMapper


                // 7. Return success result with UserId
                return await Task.FromResult(Result<int>.Success(tempUserWithHash.UserId, StatusCodes.Status201Created));
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return await Task.FromResult(Result<int>.Failure($"An error occurred during registration: {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }
    }
    }
