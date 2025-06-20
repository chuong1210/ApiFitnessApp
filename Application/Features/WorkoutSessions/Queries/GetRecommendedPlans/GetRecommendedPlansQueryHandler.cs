using Application.Features.WorkoutSessions.Queries.Common;
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
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Queries.GetRecommendedPlans
{

    public class GetRecommendedPlansQueryHandler : IRequestHandler<GetRecommendedPlansQuery, IResult<List<WorkoutPlanDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecommendedPlansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<WorkoutPlanDto>>> Handle(GetRecommendedPlansQuery request, CancellationToken cancellationToken)
        {
            // Logic gợi ý đơn giản: Lấy ngẫu nhiên hoặc lấy các plan mới nhất
            var recommendedPlans = await _unitOfWork.WorkoutPlans.GetAllQueryable()
                .OrderByDescending(p => p.CreatedAt) // Ví dụ: lấy các plan mới nhất
                .Take(request.Count)
                .ProjectTo<WorkoutPlanDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<WorkoutPlanDto>>.Success(recommendedPlans, StatusCodes.Status200OK);
        }
    }
    }
