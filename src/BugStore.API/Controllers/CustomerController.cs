using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.DTOs;
using BugStore.Application.Services.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly IMediatorService _mediator;

    public CustomerController(IMediatorService mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        return Ok("Endpoint works.");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery]int page = 1, int pageSize = 10)
    {
        return Ok("Endpoint works.");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] RequestCustomerDTO request)
    {
        var command = new AddCustomerCommand(request);
        var response = await _mediator.SendAsync<AddCustomerCommand, ResponseDataCustomerDTO>(command);
        return Created("",response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] object customerDto)
    {
       return Ok("Endpoint works.");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        return Ok("Endpoint works.");
    }

}
