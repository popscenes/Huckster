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
    public class UpdateMenuCommand : ICommand
    {
        public List<Menu> Menus { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateMenuCommandHandler : AdoCommandHandler<UpdateMenuCommand>
    {
        public UpdateMenuCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, UpdateMenuCommand command)
        {
            var menus = context.Query<Menu>("Select * from [dbo].[Menu] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = command.Id }).ToList();

            foreach (var menu in menus)
            {
                menu.MenuItems = context.Query<MenuItem>("Select * from [dbo].[MenuItem] where MenuId = @MenuId", new { MenuId = menu.Id }).ToList();
                foreach (var menuItem in menu.MenuItems)
                {
                    context.Delete(menuItem);
                }
                context.Delete(menu);
            }

            foreach (var menu in command.Menus)
            {
                var newMenuId = context.Insert(menu);
                menu.MenuItems.ForEach((_) => { _.MenuId = newMenuId; context.Insert(_); });
            }

        }
    }
}