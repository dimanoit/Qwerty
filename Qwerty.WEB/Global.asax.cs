using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Ninject;
using Ninject.Modules;
using Qwerty.Settings;

namespace Qwerty.WEB
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile<MapperSetting>());
            GlobalConfiguration.Configure(WebApiConfig.Register);

            NinjectModule ServiceBinding = new NinjectRegistrations();
            var kernel = new StandardKernel(ServiceBinding);
            var ningectResolver = new NinjectDependencyResolver(kernel);
            GlobalConfiguration.Configuration.DependencyResolver = ningectResolver;
        }
    }
}
