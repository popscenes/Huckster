using System.Data;
using Dapper;
using DapperExtensions;
using System.Threading.Tasks;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Omu.ValueInjecter;

namespace Domain.Customer.Commands
{
    public class CreateCustomerCommand: ICommand
    {
        public Customer NewCustomer { get; set; }
        public Address Address { get; set; }
    }

    public class CreateCustomerCommandHandler : AdoCommandHandler<CreateCustomerCommand>
    {
        public CreateCustomerCommandHandler(AdoContext adoContext) : base(adoContext)
        {
            
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, CreateCustomerCommand command)
        {
            var orderId = context.Insert(command.NewCustomer);

            if (command.Address != null)
            {
                var customerAddress = new Address();
                customerAddress.InjectFrom(command.Address);
                customerAddress.ParentAggregateId = command.NewCustomer.AggregateRootId;
                context.Insert(customerAddress);
            }
        }
    }
}