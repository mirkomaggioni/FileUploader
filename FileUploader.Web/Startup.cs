
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FileUploader.Core.ServiceLayer;
using FileUploader.Web.Controllers.Api;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FileUploader.Web.Startup))]

namespace FileUploader.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new FileBlobsService(@"c:\temp\chunks\"))
                .AsImplementedInterfaces()
                .SingleInstance();

            var apiControllersAssembly = Assembly.GetAssembly(typeof(FileBlobsController));

            builder.RegisterApiControllers(apiControllersAssembly);

            var containerBuilder = builder.Build();

            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(containerBuilder)
            };

            GlobalConfiguration.Configure(c => WebApiConfig.Register(config));
            app.UseWebApi(config);
        }
    }
}