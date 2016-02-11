using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }
    }
}
