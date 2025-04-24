using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Queries.GetFoodItemById
{

    public class GetFoodItemByIdQueryHandler : IRequestHandler<GetFoodItemByIdQuery, IResult<FoodItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFoodItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<FoodItemDto>> Handle(GetFoodItemByIdQuery request, CancellationToken cancellationToken)
        {
            var foodItem = await _unitOfWork.FoodItems.GetByIdAsync(request.FoodId, cancellationToken);

            if (foodItem == null)
            {
                return Result<FoodItemDto>.Failure($"Food item with ID {request.FoodId} not found.", StatusCodes.Status404NotFound);
            }

            // Map Entity sang DTO
            var foodItemDto = _mapper.Map<FoodItemDto>(foodItem);

            return Result<FoodItemDto>.Success(foodItemDto, StatusCodes.Status200OK);
        }
    }
    }
