using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FoodItems.Queries.GetFoodCategories
{
    public class GetFoodCategoriesQueryHandler : IRequestHandler<GetFoodCategoriesQuery, IResult<List<CategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; // Hoặc inject trực tiếp AppDbContext

        public GetFoodCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<List<CategoryDto>>> Handle(GetFoodCategoriesQuery request, CancellationToken cancellationToken)
        {
            // Lấy các giá trị category duy nhất, không null/trống và sắp xếp
            var categories = await _unitOfWork.FoodItems.GetAllQueryable()
                .Where(fi => !string.IsNullOrEmpty(fi.Category))
                .Select(fi => fi.Category!) // Dùng ! vì đã lọc NotNullOrEmpty
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync(cancellationToken);

            // Giả sử mỗi category có một ảnh đại diện (lấy ảnh của món ăn đầu tiên trong category đó)
            // Đây là một cách đơn giản, có thể tối ưu hơn nếu bạn có bảng Category riêng
            var categoryDtos = new List<CategoryDto>();
            foreach (var categoryName in categories)
            {
                var firstFoodInCategory = await _unitOfWork.FoodItems.GetAllQueryable()
                    .FirstOrDefaultAsync(fi => fi.Category == categoryName, cancellationToken);

                categoryDtos.Add(new CategoryDto(categoryName, firstFoodInCategory?.ImageUrl));
            }

            return Result<List<CategoryDto>>.Success(categoryDtos, StatusCodes.Status200OK);
        }
    }
    }
