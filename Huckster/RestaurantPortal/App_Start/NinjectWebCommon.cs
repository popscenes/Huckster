using System.Linq;
using infrastructure.DataAccess;
using infrastructure.Messaging;
using infrastructure.Messaging.Azure;
using infrastructure.Utility;
using infrastructure.Utility.Infrastructure.Framework;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(RestaurantPortal.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(RestaurantPortal.App_Start.NinjectWebCommon), "Stop")]

namespace RestaurantPortal.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);

            NinjectKernel.AppKernel.Bind<AdoContext>().ToMethod(_ => new AdoContext()
            {
                DatabaseName = "BootleggerSql"
            });

            NinjectKernel.AppKernel.Bind<IMessageBus>().To<WebJobMessageBus>();
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                InitNinject(kernelInit =>
                {
                    kernelInit.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                    kernelInit.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                }, "Application.", "Infrastructure.", "Domain.");
                return NinjectKernel.AppKernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
        }

        public static IKernel InitNinject(Action<IKernel> preInit = null, params String[] defaultAssemblies)
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

            Ninject.IKernel kernel = NinjectKernel.CreateKernel(assem, true, preInit);
            return kernel;
        }
    }
}
