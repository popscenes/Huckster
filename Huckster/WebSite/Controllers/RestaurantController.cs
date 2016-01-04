using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Restaurant;
using Domain.Restaurant.Queries;
using infrastructure.CQRS;

namespace WebSite.Controllers
{
    
    public class RestaurantController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        // GET: Restaurant
        public RestaurantController(IQueryChannel queryChannel)
        {
            _queryChannel = queryChannel;
        }

        [HttpGet]
        [Route("Restaurant/{id}")]
        public async Task<ActionResult> Detail(int id)
        {
            var restaurantDetail = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery()
            {
                Id = id
            });

            restaurantDetail.DeliveryHours = restaurantDetail.ValidDeliveryHours(DateTime.Now.DayOfWeek, DateTime.Now.AddMinutes(30).TimeOfDay);
            return View(restaurantDetail);
        }
    }
}