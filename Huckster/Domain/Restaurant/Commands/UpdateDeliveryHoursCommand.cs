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
    public class UpdateDeliveryHoursCommand: ICommand
    {
        public List<DeliveryHours> DeliveryHours { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateDeliveryHoursCommandHandler : AdoCommandHandler<UpdateDeliveryHoursCommand>
    {
        public UpdateDeliveryHoursCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateDeliveryHoursCommand command)
        {
            var deliveryHours = context.Query<DeliveryHours>("Select * from [dbo].[DeliveryHours] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = command.Id }).ToList();

            foreach (var deliveryHour in deliveryHours)
            {
                context.Delete(deliveryHour);
            }

            foreach (var deliveryHour in command.DeliveryHours)
            {
                deliveryHour.ParentAggregateId = command.Id;
                context.Insert(deliveryHour);
            }
        }
    }
}