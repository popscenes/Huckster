using infrastructure.CQRS;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public class SimpleQueryChannel : IQueryChannel
    {
        [Inject]
        private IResolutionRoot ResolutionRoot { get; set; }

        public async Task<TReturn> QueryAsync<TQuery, TReturn>(IQuery<TQuery, TReturn> argument, TReturn defaultReturn = default(TReturn)) where TQuery : IQuery<TQuery, TReturn>
        {
            var handler = ResolutionRoot.TryGet<IQueryHandler<TQuery, TReturn>>();

            if (handler == null)
                throw new ArgumentException("no query found for " + argument.GetType().Name);

            return await handler.HandleAsync((TQuery)argument);
        }
    }
}
