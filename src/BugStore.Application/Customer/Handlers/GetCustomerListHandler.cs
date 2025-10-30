using BugStore.Application.Customer.DTOs;
using BugStore.Application.Customer.Queries;
using BugStore.Application.Extensions;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Customer.Handlers
{
    public class GetCustomerListHandler : IRequestHandler<GetCustomerListQuery, ResponseListCustomerDTO>
    {
        private readonly ICustomerReadOnlyRepository _repository;
        public GetCustomerListHandler(ICustomerReadOnlyRepository repository)
        {
            _repository = repository;
        }
        public async ValueTask<ResponseListCustomerDTO> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
        {
            var customers = await _repository.GetAllAsync(request.Page, request.PageSize);
            var totalCustomers = await _repository.GetTotalItemAsync();
            return new ResponseListCustomerDTO(
                totalCustomers,
                request.Page,
                totalCustomers.CalculateTotalPages(request.PageSize),
                customers.Select(x => new ResponseDataCustomerDTO(x.Id, x.Name, x.Email, x.Phone, x.BirthDate)).ToList()
            );
        }
    }
}
