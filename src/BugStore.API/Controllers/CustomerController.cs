using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.DTOs;
using BugStore.Application.Customer.Queries;
using BugStore.Application.Services.Mediator;
using Mediator;
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
        var query = new GetCustomerByIdQuery(id);
        var response = _mediator.SendAsync<GetCustomerByIdQuery, ResponseDataCustomerDTO>(query);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery]int page = 1, int pageSize = 10)
    {
        var query = new GetCustomerListQuery(page, pageSize);
        var response = _mediator.SendAsync<GetCustomerListQuery, ResponseListCustomerDTO>(query);
        return Ok(response);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] RequestCustomerDTO request)
    {
        var command = new UpdateCustomerCommand(id, request);
        var response = await _mediator.SendAsync<UpdateCustomerCommand, ResponseDataCustomerDTO>(command);
       return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCustomerCommand(id);
        await _mediator.SendAsync<DeleteCustomerCommand, Unit>(command);
        return NoContent();
    }

}
