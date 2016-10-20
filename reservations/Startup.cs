using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(reservations.Startup))]
namespace reservations
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
