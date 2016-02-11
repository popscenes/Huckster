using infrastructure.Utility.Infrastructure.Framework;
using Ninject;
using Ninject.Activation.Caching;
using Ninject.Modules;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Utility
{
    public static class NinjectKernel
    {

        public static IKernel AppKernel { get; private set; }
        private static readonly object LockOb = new object();

        public static IKernel CreateKernel(List<Assembly> assem = null, bool fullReload = true, Action<IKernel> preInit = null)
        {
            lock (LockOb)
            {
                if (AppKernel != null)
                {
                    var cache = AppKernel.Components.Get<ICache>();
                    if (cache != null) cache.Clear();
                    cache = AppKernel.TryGet<ICache>();
                    if (cache != null) cache.Clear();

                    if (!fullReload) return AppKernel;

                    if (assem != null)
                        assem.SelectMany(asm => asm.GetNinjectModules().Select(m => m.Name)).ToList().ForEach(n =>
                        {
                            try
                            {
                                AppKernel.Unload(n);
                            }
                            catch (Exception e)
                            {
                            }
                        });

                    AppKernel.Dispose();
                    AppKernel = null;

                }

                if (AppKernel == null)
                    AppKernel = new StandardKernel(new NinjectSettings()
                    {
                        InjectNonPublic = true,
                        InjectParentPrivateProperties = true,
                    });

                if (preInit != null)
                    preInit(AppKernel);

                if (assem != null)
                    AppKernel.Load(assem);

                return AppKernel;
            }

        }

        
    }

    public static class NinjectExtensions
    {
        public static void ClearLogicalCallContext()
        {
            CallContext.LogicalSetData("NinjectScopeCtx", null);
        }
        public static NinjectCallContext GetNinjectLogicalCallContext()
        {
            var ob = CallContext.LogicalGetData("NinjectScopeCtx") as NinjectCallContext;
            if (ob == null)
            {
                ob = new NinjectCallContext() { CallContext = Guid.NewGuid() };
                CallContext.LogicalSetData("NinjectScopeCtx", ob);
            }
            return ob;
        }

        public class NinjectCallContext : MarshalByRefObject
        {
            private static int _allocCount = 0;
            private static int _freeCount = 0;

            private readonly int _mycount;

            public Guid CallContext { get; set; }
            ~NinjectCallContext()
            {
                Interlocked.Increment(ref _freeCount);
            }

            public NinjectCallContext()
            {
                _mycount = Interlocked.Increment(ref _allocCount);
            }
        }
        public static IBindingNamedWithOrOnSyntax<T> InCallContextScope<T>(this IBindingInSyntax<T> syntax)
        {
            return syntax.InScope(context => GetNinjectLogicalCallContext());
        }

        public static bool HasNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes().Any(IsLoadableModule);
        }

        public static IEnumerable<INinjectModule> GetNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes()
                    .Where(IsLoadableModule)
                    .Select(type => Activator.CreateInstance(type) as INinjectModule);
        }

        private static bool IsLoadableModule(Type type)
        {
            return typeof(INinjectModule).IsAssignableFrom(type)
                && !type.IsAbstract
                && !type.IsInterface
                && type.GetConstructor(Type.EmptyTypes) != null;
        }


    }
}
