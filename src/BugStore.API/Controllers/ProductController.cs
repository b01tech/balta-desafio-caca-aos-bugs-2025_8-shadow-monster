using BugStore.Application.Product.Commands;
using BugStore.Application.Product.DTOs;
using BugStore.Application.Services.Mediator;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediatorService _mediator;

        public ProductController(IMediatorService mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAsync(RequestProductDTO request)
        {
            var command = new AddProductCommand(request);
            var response = await _mediator.SendAsync<AddProductCommand, ResponseProductDetailedDTO>(
                command
            );
            return Created("", response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RequestProductDTO request)
        {
            var command = new UpdateProductCommand(id, request);
            var response = await _mediator.SendAsync<
                UpdateProductCommand,
                ResponseProductDetailedDTO
            >(command);
            return Ok(response);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] decimal price)
        {
            var command = new UpdatePriceCommand(id, price);
            var response = await _mediator.SendAsync<
                UpdatePriceCommand,
                ResponseProductDetailedDTO
            >(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteProductCommand(id);
            await _mediator.SendAsync<DeleteProductCommand, Unit>(command);
            return NoContent();
        }
    }
}
