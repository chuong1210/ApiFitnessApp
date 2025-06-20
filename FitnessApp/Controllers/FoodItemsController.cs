using Application.Features.FoodItems.Commands.CreateFoodItem;
using Application.Features.FoodItems.Commands.DeleteFoodItem;
using Application.Features.FoodItems.Commands.UpdateFoodItem;
using Application.Features.FoodItems.Queries.GetAllFoodItems;
using Application.Features.FoodItems.Queries.GetFoodCategories;
using Application.Features.FoodItems.Queries.GetFoodItemById;
using Application.Features.FoodItems.Queries.GetPopularFoods;
using Application.Features.FoodItems.Queries.GetRecommendedFoods;
using Application.Features.FoodItems.Queries.SearchFoodItemsQuery;
using Application.Responses;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Yêu cầu đăng nhập để truy cập thư viện thực phẩm
    public class FoodItemsController : ControllerBase
    {
        private readonly ISender _mediator;

        public FoodItemsController(ISender mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Gets a paginated list of all food items.
        /// </summary>
        /// <param name="query">Pagination parameters (pageNumber, pageSize).</param>
        /// <returns>A paginated list of food items.</returns>

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<List<FoodItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllFoodItems([FromQuery] GetAllFoodItemsQuery query)
        {
            // MediatR sẽ tự động bind query parameters vào object query
            var result = await _mediator.Send(query);
            // PaginatedResult đã chứa Succeeded=true và Code=200 nếu thành công
            return StatusCode(result.Code, result);

        }

        /// <summary>
        /// Searches for food items by name (paginated).
        /// </summary>
        /// <param name="query">Search term and pagination parameters.</param>
        /// <returns>A paginated list of matching food items.</returns>
        //[HttpGet("search")]
        //[ProducesResponseType(typeof(PaginatedResult<List<FoodItemDto>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> SearchFoodItems([FromQuery] SearchFoodItemsQuery query)
        //{
        //    var result = await _mediator.Send(query);
        //    return StatusCode(result.Code, result);
        //}
        /// <summary>
        /// Gets the details of a specific food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the food item.</param>
        /// <returns>The food item details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetFoodItemById(int id)
        {
            var query = new GetFoodItemByIdQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        // [Permission(Permissions.FoodItems.Create)] // Ví dụ phân quyền Admin
        [Consumes("multipart/form-data")] // Quan trọng: Cho phép nhận file upload
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)] // Nếu có phân quyền
        public async Task<IActionResult> CreateFoodItem([FromForm] CreateFoodItemCommand command)
        {
            // Model Binding sẽ tự động map các trường form và file vào command object
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Updates an existing food item.
        /// </summary>
        /// <param name="id">The ID of the food item to update.</param>
        /// <param name="command">Updated food item data and optional new image file.</param>
        /// <returns>The updated food item details.</returns>
        [HttpPut("{id}")]
        // [Permission(Permissions.FoodItems.Update)] // Ví dụ phân quyền Admin
        [Consumes("multipart/form-data")] // Quan trọng: Cho phép nhận file upload
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<FoodItemDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)] // Nếu có phân quyền
        public async Task<IActionResult> UpdateFoodItem(int id, [FromForm] UpdateFoodItemCommand command)
        {
            // Đảm bảo ID từ route khớp với ID trong command (an toàn hơn)
            if (id != command.FoodId)
            {
                return BadRequest(Result<FoodItemDto>.Failure("Route ID does not match command ID.", StatusCodes.Status400BadRequest));
            }
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Deletes a food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the food item to delete.</param>
        /// <returns>The ID of the deleted item.</returns>
        [HttpDelete("{id}")]
        // [Permission(Permissions.FoodItems.Delete)] // Ví dụ phân quyền Admin
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status400BadRequest)] // Nếu item đang được dùng
        [ProducesResponseType(typeof(IResult<int>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)] // Nếu có phân quyền
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var command = new DeleteFoodItemCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }


        /// <summary>
        /// Gets a list of all food categories.
        /// </summary>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(IResult<List<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFoodCategories()
        {
            var result = await _mediator.Send(new GetFoodCategoriesQuery());
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets a list of recommended food items.
        /// </summary>
        /// <param name="query">Query parameters like mealType and count.</param>
        [HttpGet("recommendation")]
        [ProducesResponseType(typeof(IResult<List<FoodItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendedFoods([FromQuery] GetRecommendedFoodsQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Gets a list of popular food items.
        /// </summary>
        /// <param name="query">Query parameters like mealType and count.</param>
        [HttpGet("popular")]
        [ProducesResponseType(typeof(IResult<List<FoodItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopularFoods([FromQuery] GetPopularFoodsQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Searches for food items by name and/or category (paginated).
        /// </summary>
        /// <param name="query">Search term, category, and pagination parameters.</param>
        /// <returns>A paginated list of matching food items.</returns>
        [HttpGet("search")] // Action Search đã được cập nhật
        [ProducesResponseType(typeof(PaginatedResult<List<FoodItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchFoodItems([FromQuery] SearchFoodItemsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
    }
