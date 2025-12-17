using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Smartstore.Engine;

namespace Smartstore.Keycloak.Auth
{
    internal sealed class KeycloakOptionsConfigurer : IConfigureOptions<AuthenticationOptions>, IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IApplicationContext _appContext;

        public KeycloakOptionsConfigurer(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public void Configure(AuthenticationOptions options)
        {
            // Register the OpenID Connect client handler in the authentication handlers collection.
            options.AddScheme(KeycloakDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Keycloak";
                builder.HandlerType = typeof(OpenIdConnectHandler);
            });
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            // Ignore OpenID Connect client handler instances that don't correspond to our Keycloak scheme.
            if (name.HasValue() && !string.Equals(name, KeycloakDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            var settings = _appContext.Services.Resolve<KeycloakExternalAuthSettings>();

            // Get configuration from environment variables first, falling back to database settings
            var authority = GetConfigValue("SMARTSTORE_KEYCLOAK_AUTHORITY", settings.Authority)?.TrimEnd('/');
            var realm = GetConfigValue("SMARTSTORE_KEYCLOAK_REALM", settings.Realm);
            var clientId = GetConfigValue("SMARTSTORE_KEYCLOAK_CLIENTID", settings.ClientId);
            var clientSecret = GetConfigValue("SMARTSTORE_KEYCLOAK_CLIENTSECRET", settings.ClientSecret);

            // Build the Keycloak authority URL
            // If authority already contains "/realms/", use it as-is (it's the full OIDC authority URL)
            // Otherwise, construct it as {Authority}/realms/{Realm}
            if (!string.IsNullOrEmpty(authority))
            {
                if (authority.Contains("/realms/"))
                {
                    // Authority is already the full URL (e.g., https://keycloak.example.com/realms/myrealm)
                    options.Authority = authority;
                }
                else if (!string.IsNullOrEmpty(realm))
                {
                    // Authority is the base URL, append /realms/{realm}
                    options.Authority = $"{authority}/realms/{realm}";
                }
            }

            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.CallbackPath = KeycloakDefaults.CallbackPath;

            // Disable Pushed Authorization Requests (PAR) - not all Keycloak configurations support it
            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

            // Standard OIDC scopes
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            options.Events = new OpenIdConnectEvents
            {
                OnRemoteFailure = context =>
                {
                    var errorMessage = context.Failure?.Message ?? "Unknown error";
                    var errorUrl = $"/identity/externalerrorcallback?provider=keycloak&errorMessage={errorMessage.UrlEncode()}";
                    context.Response.Redirect(errorUrl);
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        }

        public void Configure(OpenIdConnectOptions options)
            => Debug.Fail("This infrastructure method shouldn't be called.");

        /// <summary>
        /// Gets configuration value from environment variable first, falling back to settings value.
        /// </summary>
        private static string? GetConfigValue(string envVarName, string? settingsValue)
        {
            var envValue = Environment.GetEnvironmentVariable(envVarName);
            return !string.IsNullOrEmpty(envValue) ? envValue : settingsValue;
        }
    }
}
