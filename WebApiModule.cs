using Autofac;
using VHTED.Api.Infrastructure.Configuration;
using VHTED.Api.Infrastructure.Email;
using VHTED.Api.Infrastructure.HttpClients;
using Microsoft.Extensions.Configuration;

namespace VHTED.Api
{
    public class WebApiModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // infrastructure
            builder.RegisterType<ApplicationConfiguration>().As<IApplicationConfiguration>().SingleInstance();
            builder.RegisterType<IConfiguration>().SingleInstance();

            builder.RegisterType<IdentityClientUtil>().SingleInstance();
            builder.RegisterType<MailClientUtils>().SingleInstance();
            builder.RegisterType<HttpClientUtils>().InstancePerLifetimeScope();
        }
    }
}
