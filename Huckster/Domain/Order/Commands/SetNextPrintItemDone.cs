using Dapper;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Order.Commands
{
    public class SetNextPrintItemDone : ICommand
    {
        public string RestaurantId { get; set; }
    }

    public class SetNextPrintItemDoneHandler : AdoCommandHandler<SetNextPrintItemDone>
    {
        public SetNextPrintItemDoneHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, SetNextPrintItemDone command)
        {
            var printItem =
                context.Query<PrintQueue>("Select * from [dbo].[PrintQueue] where [RestaurantId] = @RestaurantId and Printed = 0 order by DateTimeAdded ASC",
                    new { RestaurantId = command.RestaurantId }).FirstOrDefault();

            printItem.Printed = true;
            context.Update(printItem);
        }
    }
}
