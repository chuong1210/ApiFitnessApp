using Application.Responses;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
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

namespace Application.Features.Users.Queries.GetUserById
{



    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger; // Optional

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found.", (int)HttpStatusCode.NotFound);
                }

                var userDto = _mapper.Map<UserDto>(user);

                return Result<UserDto>.Success(userDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by ID: {UserId}", request.UserId);
                return Result<UserDto>.Failure($"An error occurred: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }

}