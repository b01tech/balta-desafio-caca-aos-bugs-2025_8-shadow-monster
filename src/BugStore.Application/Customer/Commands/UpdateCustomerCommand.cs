using BugStore.Application.Customer.DTOs;
using Mediator;

﻿namespace BugStore.Application.Customer.Commands;
public class UpdateCustomerCommand: IRequest<ResponseDataCustomerDTO>
{
    public Guid Id { get; set; }
    public RequestCustomerDTO Request { get; set; }
    public UpdateCustomerCommand(Guid id, RequestCustomerDTO request)
    {
        Id = id;
        Request = request;
    }
}
