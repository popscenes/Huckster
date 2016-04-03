using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestaurantPortal.Startup))]
namespace RestaurantPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
