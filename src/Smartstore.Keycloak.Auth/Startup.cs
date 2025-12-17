using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Smartstore.Engine;
using Smartstore.Engine.Builders;

namespace Smartstore.Keycloak.Auth
{
    internal class Startup : StarterBase
    {
        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterType<KeycloakOptionsConfigurer>()
                .As<IConfigureOptions<AuthenticationOptions>>()
                .As<IConfigureOptions<OpenIdConnectOptions>>()
                .InstancePerDependency();

            builder.RegisterType<OpenIdConnectPostConfigureOptions>()
                .As<IPostConfigureOptions<OpenIdConnectOptions>>()
                .InstancePerDependency();

            // Register our custom post-configure to ensure PAR is disabled after default post-configuration
            builder.RegisterType<KeycloakPostConfigureOptions>()
                .As<IPostConfigureOptions<OpenIdConnectOptions>>()
                .InstancePerDependency();
        }
    }
}
