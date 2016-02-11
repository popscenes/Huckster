using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.CQRS;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace Domain
{
    public class DomainNinjectModule:NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x
                .FromThisAssembly()
                .SelectAllClasses().InheritedFrom(typeof(ICommandHandler<>))
                .BindAllInterfaces());

            Kernel.Bind(x => x
                .FromThisAssembly()
                .SelectAllClasses().InheritedFrom(typeof(IQueryHandler<,>))
                .BindAllInterfaces());
        }
    }
}
