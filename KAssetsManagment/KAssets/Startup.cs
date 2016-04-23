using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KAssets.Startup))]
namespace KAssets
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
