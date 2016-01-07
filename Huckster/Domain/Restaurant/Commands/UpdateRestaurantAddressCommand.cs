using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Omu.ValueInjecter;

namespace Domain.Restaurant.Commands
{
    public class UpdateRestaurantAddressCommand: ICommand
    {
        public int Id { get; set; }
        public Address Address { get; set; }
    }

    public class UpdateRestaurantAddressCommandHandler : AdoCommandHandler<UpdateRestaurantAddressCommand>
    {
        public UpdateRestaurantAddressCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateRestaurantAddressCommand command)
        {
            var addressToUpdate = context.Get<Address>(command.Address.Id);
            addressToUpdate.InjectFrom(command.Address);
            context.Update(addressToUpdate);
        }
    }
}