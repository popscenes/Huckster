using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Restaurant;
using Domain.Restaurant.Commands;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    public class RestaurantApiController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryChannel _queryChannel;

        public RestaurantApiController(ICommandDispatcher commandDispatcher, IQueryChannel queryChannel)
        {
            _commandDispatcher = commandDispatcher;
            _queryChannel = queryChannel;
        }

        [HttpPost]
        [Route("api/restaurant/{id}/update-menu")]
        [Authorize]
        public async Task<IHttpActionResult> UpdateMenu([FromUri] Guid id, [FromBody] UpdateMenuVieModel model)
        {
            await _commandDispatcher.DispatchAsync(new UpdateMenuCommand() {Id = id, Menus = model.menus});
            return Ok();
        }

        public class UpdateMenuVieModel
        {
            public List<Menu> menus { get; set; }
        }
    }
}
