using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Shared;

namespace WebSite.Models
{
    public class OrderCustomerDetails
    {
        public Address Address { get; set; }
        public int Id { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
    }
}