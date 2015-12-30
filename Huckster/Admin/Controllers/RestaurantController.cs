using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Application.Azure;
using Domain.Restaurant.Commands;
using Domain.Restaurant.Queries;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    [Authorize]
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
    }
}