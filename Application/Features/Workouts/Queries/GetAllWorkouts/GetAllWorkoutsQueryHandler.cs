using Application.Features.Workouts.Queries.Common;
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

namespace Application.Features.Workouts.Queries.GetAllWorkouts
{

    public class GetAllWorkoutsQueryHandler : IRequestHandler<GetAllWorkoutsQuery, IResult<List<WorkoutDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllWorkoutsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<WorkoutDto>>> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var workouts = await _unitOfWork.Workouts.GetAllQueryable()
                .OrderBy(w => w.Name)
                .ProjectTo<WorkoutDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để tối ưu
                .ToListAsync(cancellationToken);

            return Result<List<WorkoutDto>>.Success(workouts, StatusCodes.Status200OK);
        }
    }

}