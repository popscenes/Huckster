﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Restaurant;
using Domain.Restaurant.Commands;
using Domain.Restaurant.Queries;
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

        [HttpPost]
        [Route("api/restaurant/{id}/update-suburbs")]
        [Authorize]
        public async Task<IHttpActionResult> UpdateSuburbs([FromUri] Guid id, [FromBody] UpdateSuburbVieModel model)
        {
            await _commandDispatcher.DispatchAsync(new UpdateSuburbCommand() { Id = id, Suburbs = model.suburbs });
            return Ok();
        }

        [HttpGet]
        [Route("api/restaurant/suburbs")]
        [Authorize]
        public async Task<IHttpActionResult> Suburbs([FromUri] string searchText)
        {
            var suburbs = await _queryChannel.QueryAsync(new MasterSuburbSearchQuery() {Searchtext = searchText });
            return Ok(suburbs);
        }



        public class UpdateMenuVieModel
        {
            public List<Menu> menus { get; set; }
        }

        public class UpdateSuburbVieModel
        {
            public List<DeliverySuburb> suburbs { get; set; }
        }
    }
}
