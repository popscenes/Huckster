using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Order.Queries;
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
        public async Task<ActionResult> Detail(int id, Guid? orderId = null)
        {
            var restaurantDetail = await _queryChannel.QueryAsync(new GetRestaurantDetailWithValidHoursByIdQuery()
            {
                Id = id
            }, new CacheOptions()
            {
                CacheKey = $"GetRestaurantDetailByIdQuery{id}",
                CacheForMinutes = 1
            });


            if (orderId != null)
            {
                var orderDetail = await
                    _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() {AggregateId = orderId.Value});
                ViewBag.OrderDetail = orderDetail;
            }

            return View(restaurantDetail);
        }
    }
}