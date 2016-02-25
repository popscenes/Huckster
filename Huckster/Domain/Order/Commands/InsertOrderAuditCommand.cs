using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class InsertOrderAuditCommand: ICommand
    {
        public string Action { get; set; }
        public string UserName { get; set; }
        public Guid OrderAggregateRoot { get; set; }
    }

    public class InsertOrderAuditCommandHandler : AdoCommandHandler<InsertOrderAuditCommand>
    {
        public InsertOrderAuditCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, InsertOrderAuditCommand command)
        {
            var audit = new OrderAudit()
            {
                ParentAggregateId = command.OrderAggregateRoot,
                Action = command.Action,
                UserName = command.UserName,
                CreatedDateTime = DateTime.UtcNow,
            };
            context.Insert(audit);
        }
    }
}