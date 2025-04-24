using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;

namespace Application.Features.Users.Queries.GetUserByEmail
{

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, IResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByEmailQueryHandler> _logger;

        public GetUserByEmailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUserByEmailQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Result<UserDto>.Failure("Email cannot be empty.", (int)HttpStatusCode.BadRequest);
                }

                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found with Email: {Email}", request.Email);
                    return Result<UserDto>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                var userDto = _mapper.Map<UserDto>(user);

                return Result<UserDto>.Success(userDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by Email: {Email}", request.Email);
                return Result<UserDto>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
    }
