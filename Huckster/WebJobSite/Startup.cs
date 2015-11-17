using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebJobSite.Startup))]
namespace WebJobSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
