using Application.Features.Users.Queries.GetUserById;
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

namespace Application.Features.Users.Queries.GetUsers
{

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersQueryHandler> _logger; // Optional

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUsersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate pagination parameters
                if (request.PageNumber < 1) request = request with { PageNumber = 1 };
                if (request.PageSize < 1) request = request with { PageSize = 10 }; // Or a default sensible minimum
                if (request.PageSize > 100) request = request with { PageSize = 100 }; // Optional: Max page size

                var (users, totalCount) = await _unitOfWork.Users.GetPagedListAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SearchTerm,
                    cancellationToken
                );

                var userDtos = _mapper.Map<List<UserDto>>(users);

                // Sử dụng helper method của PaginatedResult
                return PaginatedResult<List<UserDto>>.Success(userDtos, totalCount, request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated users.");
                // Trả về lỗi sử dụng helper của PaginatedResult
                return PaginatedResult<List<UserDto>>.Failure((int)HttpStatusCode.InternalServerError, new List<string> { $"An error occurred: {ex.Message}" });
            }
        }
    }
    }
