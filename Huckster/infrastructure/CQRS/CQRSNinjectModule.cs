using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public class CQRSNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IQueryChannel>().To<SimpleQueryChannel>().InTransientScope();
            Kernel.Bind<ICommandDispatcher>().To<SimpleCommandDispatcher>().InTransientScope();

            Func<ObjectCache> getInMemCache = () =>
            {
                var cacheSettings = new NameValueCollection(3)
                        {
                            {"CacheMemoryLimitMegabytes", Convert.ToString(0)},
                            {"physicalMemoryLimitPercentage", Convert.ToString(49)},
                            {"pollingInterval", Convert.ToString("00:00:30")}
                        };
                return new MemoryCache("WebSiteCache", cacheSettings);
            };

            Kernel.Bind<ObjectCache>().ToMethod(context => getInMemCache())
                                 .InSingletonScope();
        }
    }
}
