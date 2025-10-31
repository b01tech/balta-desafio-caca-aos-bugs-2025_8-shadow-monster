using BugStore.Application.Reports.DTOs;
using Mediator;

namespace BugStore.Application.Reports.Queries
{
    public class GetBestCustomersQuery : IRequest<ResponseBestCustomersListDTO>
    {
        public RequestBestCustomerDTO Request { get; set; }
        public GetBestCustomersQuery(RequestBestCustomerDTO request)
        {
            Request = request;
        }
    }
}
