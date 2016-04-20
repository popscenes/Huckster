using Dapper;
using Domain.Order;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Order.Quries
{
    public class GetNextPrintItemForRestaurant : IQuery<GetNextPrintItemForRestaurant, PrintQueue>
    {
        public string RestaurantId { get; set; }
    }

    public class GetNextPrintItemForRestaurantHandler : AdoQueryHandler<GetNextPrintItemForRestaurant, PrintQueue>
    {
        public GetNextPrintItemForRestaurantHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<PrintQueue> HandleSqlCommandAsync(IDbConnection context, GetNextPrintItemForRestaurant argument)
        {
            var printItem =
                context.Query<PrintQueue>("Select * from [dbo].[PrintQueue] where [RestaurantId] = @RestaurantId and Printed = 0 order by DateTimeAdded ASC",
                    new { RestaurantId = argument.RestaurantId }).FirstOrDefault();

            return printItem;
        }
    }
}