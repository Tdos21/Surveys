using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Surveys.Startup))]
namespace Surveys
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
