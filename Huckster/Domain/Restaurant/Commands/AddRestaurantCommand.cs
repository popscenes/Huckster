using System.Data.SqlClient;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using System.Configuration;
using System.Data;
using Dapper;
using infrastructure.DataAccess;
using Newtonsoft.Json;

namespace Domain.Restaurant.Commands
{
    public class AddRestaurantCommand : ICommand
    {
        public Restaurant Restaurant { get; set; }
    }

    public class AddRestaurantCommandHandler : AdoCommandHandler<AddRestaurantCommand>
    {

        protected override async Task HandleSqlCommandAsync(IDbConnection context, AddRestaurantCommand command)
        {
            context.Insert<Restaurant>(command.Restaurant);
        }

        public AddRestaurantCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }
    }
}