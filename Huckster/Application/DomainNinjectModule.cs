using infrastructure.CQRS;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Application
{
    public class ApplicationNinjectModule:NinjectModule
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
