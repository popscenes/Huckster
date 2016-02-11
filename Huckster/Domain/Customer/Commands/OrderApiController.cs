using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Omu.ValueInjecter;

namespace Domain.Customer.Commands
{
    public class UpdateCustomer: ICommand
    {
        public Customer Customer { get; set; }
    }

    public class UpdateCustomerHandler : AdoCommandHandler<UpdateCustomer>
    {
        public UpdateCustomerHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateCustomer command)
        {
            var customer = context.Get<Customer>(command.Customer.Id);
            customer.InjectFrom(command.Customer);
            context.Update(customer);
        }
    }
}
