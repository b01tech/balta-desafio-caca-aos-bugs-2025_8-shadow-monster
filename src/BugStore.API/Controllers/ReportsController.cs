using BugStore.Application.Reports.DTOs;
using BugStore.Application.Reports.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("period")]
        public async Task<ActionResult<ResponseRevenueByPeriodDTO>> GetRevenueByPeriod(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var request = new RequestRevenueByPeriodDTO(startDate, endDate);
            var query = new GetRevenueByPediodQuery(request);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("best")]
        public async Task<ActionResult<ResponseBestCustomersListDTO>> GetBestCustomers(
            [FromQuery] int topCustomers = 5)
        {
            var request = new RequestBestCustomerDTO(topCustomers);
            var query = new GetBestCustomersQuery(request);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{customerId:guid}")]
        public async Task<ActionResult<ResponseRevenueByCustomerDTO>> GetRevenueByCustomer(
            [FromRoute] Guid customerId)
        {
            var request = new RequestRevenueByCustomerDTO(customerId);
            var query = new GetRevenueByCustomerQuery(request);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
