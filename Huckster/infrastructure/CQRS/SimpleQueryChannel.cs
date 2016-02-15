using infrastructure.CQRS;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Utility;
using NLog;

namespace infrastructure.CQRS
{
    public class SimpleQueryChannel : IQueryChannel
    {
        [Inject]
        private IResolutionRoot ResolutionRoot { get; set; }

        [Inject]
        public ObjectCache ObjectCache { get; set; }

        private static Logger logger = LogManager.GetLogger("mail");
        public async Task<TReturn> QueryAsync<TQuery, TReturn>(IQuery<TQuery, TReturn> argument, CacheOptions cacheOptions = null, TReturn defaultReturn = default(TReturn)) where TQuery : IQuery<TQuery, TReturn>
        {
            try
            {

                if (cacheOptions != null && cacheOptions.CacheKey.IsNotNullOrWhiteSpace())
                {
                    var result = (TReturn)ObjectCache[cacheOptions.CacheKey];
                    if (result != null)
                    {
                        return result;
                    }
                }
                var handler = NinjectKernel.AppKernel.TryGet<IQueryHandler<TQuery, TReturn>>();

                if (handler == null)
                    throw new ArgumentException("no query found for " + argument.GetType().Name);

                var queryResult = await handler.HandleAsync((TQuery)argument);

                if (cacheOptions != null && cacheOptions.CacheKey.IsNotNullOrWhiteSpace())
                {
                    ObjectCache.Add(cacheOptions.CacheKey, queryResult,
                        DateTimeOffset.Now.AddMinutes(cacheOptions.CacheForMinutes));
                }

                return queryResult;
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }
    }
}
