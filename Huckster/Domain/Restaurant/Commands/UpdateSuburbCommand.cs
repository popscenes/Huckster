using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Commands
{
    public class UpdateSuburbCommand: ICommand
    {
        public Guid Id { get; set; }
        public List<DeliverySuburb> Suburbs { get; set; }
    }

    public class UpdateSuburbCommandHandler : AdoCommandHandler<UpdateSuburbCommand>
    {
        public UpdateSuburbCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateSuburbCommand command)
        {
            var suburbs = context.Query<DeliverySuburb>("Select * from [dbo].[DeliverySuburb] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = command.Id }).ToList();

            foreach (var suburb in suburbs)
            {
                context.Delete(suburb);
            }

            foreach (var suburb in command.Suburbs)
            {
                suburb.ParentAggregateId = command.Id;
                context.Insert(suburb);
            }
        }
    }
}