using BugStore.Application.Order.Commands;
using BugStore.Application.Order.DTOs;
using BugStore.Application.Order.Queries;
using BugStore.Application.Services.Mediator;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediatorService _mediator;

        public OrderController(IMediatorService mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var query = new GetOrderByIdQuery(id);
            var response = await _mediator.SendAsync<GetOrderByIdQuery, ResponseOrderDetailedDTO>(
                query
            );
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, int pageSize = 10)
        {
            var query = new GetOrderListQuery(page, pageSize);
            var response = await _mediator.SendAsync<GetOrderListQuery, ResponseListOrderDTO>(
                query
            );
            return Ok(response);
        }

        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCustomerIdAsync(
            Guid customerId,
            [FromQuery] int page = 1,
            int pageSize = 10
        )
        {
            var query = new GetOrderByCustomerIdQuery(customerId, page, pageSize);
            var response = await _mediator.SendAsync<
                GetOrderByCustomerIdQuery,
                ResponseListOrderDTO
            >(query);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] RequestCreateOrderDTO request)
        {
            var command = new CreateOrderCommand(request.CustomerId);
            var response = await _mediator.SendAsync<CreateOrderCommand, ResponseOrderSummaryDTO>(
                command
            );
            return Created("", response);
        }

        [HttpPost("{id}/line")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddLineAsync(
            Guid id,
            [FromBody] RequestOrderLineDTO request
        )
        {
            var command = new AddLineCommand(id, request);
            var response = await _mediator.SendAsync<AddLineCommand, ResponseOrderDetailedDTO>(
                command
            );
            return Ok(response);
        }

        [HttpDelete("{orderId}/line")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveLineAsync(
            Guid orderId,
            [FromBody] RequestProductDTO request
        )
        {
            var command = new RemoveLineCommand(orderId, request.ProductId);
            var response = await _mediator.SendAsync<RemoveLineCommand, ResponseOrderDetailedDTO>(
                command
            );
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var command = new DeleteOrderCommand(id);
            await _mediator.SendAsync<DeleteOrderCommand, Unit>(command);
            return NoContent();
        }
    }
}
