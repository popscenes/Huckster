using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using infrastructure.Domain;

namespace Domain.Restaurant
{
    public class Menu: IValueObject
    {
        

        public string Title { get; set; }
        public string Description { get; set; }
        public int    Order { get; set; }
        public int   Id { get; set; }
        public Guid   ParentAggregateId { get; set; }
        public List<MenuItem> MenuItems { get; set; }
}

    public class MenuItem
    {
        public int Id { get; set; }
        public int   MenuId { get; set; }
        public string Name { get; set; }
        public string MenuGroup { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int    Order { get; set; }
    }

    public class MenuMapper : ClassMapper<Menu>
    {
        public MenuMapper()
        {
            Table("Menu");
            Map(m => m.MenuItems).Ignore();
            AutoMap();
        }
    }
}