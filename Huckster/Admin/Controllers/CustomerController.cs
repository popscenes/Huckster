﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Customer.Queries;
using infrastructure.CQRS;
using infrastructure.Messaging;

namespace Admin.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;

        public CustomerController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
        }

        // GET: Customer
        public async Task<ActionResult> Index(string searchText = "")
        {
            var customers = await _queryChannel.QueryAsync(new GetCustomersQuery()
            {
                Searchtext = searchText
            });
            return View(customers);
        }
    }
}