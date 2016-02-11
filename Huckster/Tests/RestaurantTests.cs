using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Domain.Restaurant;
using System.Linq;
using infrastructure.CQRS;
using Ninject;
using System.Threading.Tasks;
using Domain.Restaurant.Commands;
using Domain.Restaurant.Queries;
using infrastructure.DataAccess;
using infrastructure.Utility.Infrastructure.Framework;
using infrastructure.Utility;

namespace Tests
{
    [TestClass]
    public class RestaurantTests
    {
        [Inject]
        ICommandDispatcher _commandDispatcher { get; set; }

        [Inject]
        IQueryChannel _queryChannel { get; set; }

        [TestInitialize]
        public void Setup()
        {
            InitNinject("Application.", "Infrastructure.", "Domain.");
            NinjectKernel.AppKernel.Bind<AdoContext>().ToMethod(_ => new AdoContext()
            {
                DatabaseName = "BootleggerSql"
            });

            _commandDispatcher = NinjectKernel.AppKernel.Get<ICommandDispatcher>();
            _queryChannel = NinjectKernel.AppKernel.Get<IQueryChannel>();
        }

        [TestMethod]
        public async Task AddRestaurant()
        {
            var testRestaurant = new Restaurant() {
                Name = "Teddys",
                AggregateRootId = Guid.NewGuid(),
            };

            await _commandDispatcher.DispatchAsync<AddRestaurantCommand>(new AddRestaurantCommand() { Restaurant = testRestaurant });

            var restaurants = await _queryChannel.QueryAsync<GetRestaurantsQuery, IEnumerable<Restaurant>>(new GetRestaurantsQuery(), null);

            Assert.AreEqual(restaurants.ToList().Count(), 1);
        }

        public static IKernel InitNinject(params String[] defaultAssemblies)
        {
            AllAssemblies.DefaultAssemblyPrefixStrings = defaultAssemblies;
            var assem = AllAssemblies.MatchingDefault()
                .GetOrdered()
                .ThenBy(assembly => assembly
                    .GetName().Name
                    .ToLower().Contains("infrastructure")
                    ? 0
                    : 1)
                .ToList();

            Ninject.IKernel kernel = NinjectKernel.CreateKernel(assem);
            return kernel;
        }


    }
}
