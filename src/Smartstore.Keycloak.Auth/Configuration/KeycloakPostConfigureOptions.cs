using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Smartstore.Keycloak.Auth
{
    /// <summary>
    /// Post-configures OpenIdConnect options to ensure Keycloak-specific settings are applied
    /// after the default post-configuration.
    /// </summary>
    internal sealed class KeycloakPostConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        public void PostConfigure(string name, OpenIdConnectOptions options)
        {
            // Only configure our Keycloak scheme
            if (!string.Equals(name, KeycloakDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            // Disable Pushed Authorization Requests (PAR) - ensures this is set after any other post-configure
            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;
        }
    }
}
