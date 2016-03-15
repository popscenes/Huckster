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
            var dbMenus = context.Query<Menu>("Select * from [dbo].[Menu] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = command.Id }).ToList();

            var deleteMenus = dbMenus.Where(_ => !command.Menus.Select(m => m.Id).Contains(_.Id));
            var newMenus = command.Menus.Where(_ => !dbMenus.Select(m => m.Id).Contains(_.Id));
            var updatedMenus = command.Menus.Where(_ => dbMenus.Select(m => m.Id).Contains(_.Id));

            foreach (var menu in deleteMenus)
            {
                menu.MenuItems = context.Query<MenuItem>("Select * from [dbo].[MenuItem] where MenuId = @MenuId", new { MenuId = menu.Id }).ToList();
                menu.Deleted = true;
                foreach (var menuItem in menu.MenuItems)
                {
                    menuItem.Deleted = true;
                    context.Update(menuItem);
                }
                context.Update(menu);
            }

            foreach (var menu in newMenus)
            {
                var newMenuId = context.Insert(menu);
                menu.MenuItems.ForEach((_) => { _.MenuId = newMenuId; context.Insert(_); });
            }

            foreach (var menu in updatedMenus)
            {
                var dbMenuItems = context.Query<MenuItem>("Select * from [dbo].[MenuItem] where MenuId = @MenuId", new { MenuId = menu.Id }).ToList();
                //var deleteMenusItems = dbMenuItems.Where(_ => !menu.MenuItems.Select(m => m.Id).Contains(_.Id));
                //var newMenuItems = menu.MenuItems.Where(_ => !dbMenuItems.Select(m => m.Id).Contains(_.Id));
                //var updatedMenuitemss = menu.MenuItems.Where(_ => dbMenuItems.Select(m => m.Id).Contains(_.Id));

                //foreach (var menuItem in deleteMenusItems)
                //{
                //    menuItem.Deleted = true;
                //    context.Update(menuItem);
                //}
                //foreach (var menuItem in updatedMenuitemss)
                //{
                //    context.Update(menuItem);
                //}
                //foreach (var menuItem in newMenuItems)
                //{
                //    menuItem.MenuId = menu.Id;
                //    context.Insert(menuItem);
                //}

                foreach (var menuItem in dbMenuItems)
                {
                    menuItem.Deleted = true;
                    context.Update(menuItem);
                }
                foreach (var menuItem in menu.MenuItems)
                {
                    menuItem.MenuId = menu.Id;
                    context.Insert(menuItem);
                }
                context.Update(menu);
            }


           

        }
    }
}