using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Application.Azure;
using Domain.Restaurant;
using Domain.Restaurant.Commands;
using Domain.Restaurant.Queries;
using Domain.Shared;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        //private readonly AzureCloudBlobStorage _azureCloudBlobStorage;

        public RestaurantController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            //_azureCloudBlobStorage = azureCloudBlobStorage;
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
            ViewBag.RestaurantId = restaurant.Restaurant.AggregateRootId;
            return View(restaurant.RestaurantMenu);
        }

        [HttpGet]
        public async Task<ActionResult> EditTileImage(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            return View(restaurant);
        }

        [HttpGet]
        public async Task<ActionResult> EditAddress(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            ViewBag.RestaurantId = restaurant.Restaurant.AggregateRootId;
            return View(restaurant.RestauranAddress);
        }

        [HttpGet]
        public async Task<ActionResult> EditDetails(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            ViewBag.RestaurantId = restaurant.Restaurant.AggregateRootId;
            return View(restaurant.Restaurant);
        }

        [HttpPost]
        public async Task<ActionResult> EditDetails(int id, Restaurant restaurant)
        {

            await _commandDispatcher.DispatchAsync(new UpdateRestaurantDetailsCommand() { Id = id, Restaurant = restaurant });
            return RedirectToAction("Detail", new {id = id});
        }

        [HttpPost]
        public async Task<ActionResult> EditAddress(int id, Address restaurantAddress)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            if (restaurant.RestauranAddress == null)
            {
                restaurantAddress.ParentAggregateId = restaurant.Restaurant.AggregateRootId;
                await _commandDispatcher.DispatchAsync(new AddRestaurantAddressCommand() { Id = id, Address = restaurantAddress });
            }
            else
            {
                await _commandDispatcher.DispatchAsync(new UpdateRestaurantAddressCommand() { Id = id, Address = restaurantAddress });
            }

            return RedirectToAction("Detail", new { id = id });

        }

        [HttpGet]
        public async Task<ActionResult> EditSuburbs(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            ViewBag.RestaurantId = restaurant.Restaurant.AggregateRootId;
            return View(restaurant.DeliverySuburbs);
        }

        [HttpGet]
        public async Task<ActionResult> EditDeliveryTimes(int id)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            ViewBag.RestaurantId = restaurant.Restaurant.AggregateRootId;
            return View(restaurant.DeliveryHours);
        }

        [HttpPost]
        public async Task<ActionResult> EditTileImage(int id, HttpPostedFileBase tileImage)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByIdQuery() { Id = id });
            var extension = Path.GetExtension(tileImage.FileName);

            using (var memoryStream = new MemoryStream())
            {

                tileImage.InputStream.CopyTo(memoryStream);
                memoryStream.Seek(0, 0);
                AzureCloudBlobStorage azureCloudBlobStorage = new AzureCloudBlobStorage("restaurant");
                var url = azureCloudBlobStorage.UploadFileToBlob(memoryStream,
                    restaurant.Restaurant.Id + "/TileImage" + extension);

                await _commandDispatcher.DispatchAsync(new UpdateRestaurantTileImage() {Id = id, Url = url});

            }

            return View(restaurant);
        }

        [HttpGet]
        public ActionResult NewRestaurant()
        {
            return View(new Restaurant() {TimeZoneId = "AUS Eastern Standard Time" });
        }


        [HttpPost]
        public async Task<ActionResult> NewRestaurant(Restaurant restaurant)
        {
            await _commandDispatcher.DispatchAsync(new NewRestaurantCommand() { Restaurant = restaurant });
            return View(new Restaurant());
        }
    }
}