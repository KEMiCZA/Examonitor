using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Examonitor.Startup))]
namespace Examonitor
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
