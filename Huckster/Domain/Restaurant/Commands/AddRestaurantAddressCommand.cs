using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Commands
{
    public class AddRestaurantAddressCommand: ICommand
    {
        public int Id { get; set; }
        public Address Address { get; set; }
    }

    public class AddRestaurantAddressCommandHandler : AdoCommandHandler<AddRestaurantAddressCommand>
    {
        public AddRestaurantAddressCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, AddRestaurantAddressCommand command)
        {
            context.Insert(command.Address);
        }
    }
}