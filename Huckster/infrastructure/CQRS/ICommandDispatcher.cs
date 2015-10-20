using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command)
             where TCommand : class, ICommand;
    }
}
