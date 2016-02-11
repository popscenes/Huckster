using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public class SimpleCommandDispatcher: ICommandDispatcher
    {
        [Inject]
        private IResolutionRoot ResolutionRoot { get; set; }

        public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            var handler = ResolutionRoot.TryGet<ICommandHandler<TCommand>>();
            if (handler == null)
                throw new ArgumentException("Can not find Handler for command " + command.GetType().ToString());

            await handler.HandleAsync(command);
        }
    }
}
