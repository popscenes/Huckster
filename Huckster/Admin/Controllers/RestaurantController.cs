using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Restaurant.Queries;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    [Authorize]
    public class RestaurantController : Controller
    {
        private readonly IQueryChannel _queryChannel;

        public RestaurantController(IQueryChannel queryChannel)
        {
            _queryChannel = queryChannel;
        }

        // GET: Restaurant
        public async Task<ActionResult> Index()
        {
            var restaurants = await _queryChannel.QueryAsync(new GetRestaurantsQuery());
            return View(restaurants.ToList());
        }

        public async Task<ActionResult> Detail(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() {Id = id});
            return View(restaurant);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            return View(restaurant);
        }

        public async Task<ActionResult> EditMenu(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            return View(restaurant.RestaurantMenu);
        }
    }
}