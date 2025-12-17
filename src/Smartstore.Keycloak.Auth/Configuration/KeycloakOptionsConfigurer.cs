using System;
using System.Diagnostics;
using System.Net.Http;
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
            
            // Optional: separate URL for backend/metadata calls (useful in Docker where container 
            // needs to reach Keycloak via different hostname than browser)
            var metadataAuthority = Environment.GetEnvironmentVariable("SMARTSTORE_KEYCLOAK_METADATA_AUTHORITY")?.TrimEnd('/');

            // Build the Keycloak authority URL
            // If authority already contains "/realms/", use it as-is (it's the full OIDC authority URL)
            // Otherwise, construct it as {Authority}/realms/{Realm}
            string authorityUrl = null;
            string metadataAuthorityUrl = null;
            
            if (!string.IsNullOrEmpty(authority))
            {
                if (authority.Contains("/realms/"))
                {
                    authorityUrl = authority;
                }
                else if (!string.IsNullOrEmpty(realm))
                {
                    authorityUrl = $"{authority}/realms/{realm}";
                }
            }
            
            // Build metadata authority URL if specified (for Docker scenarios)
            if (!string.IsNullOrEmpty(metadataAuthority))
            {
                if (metadataAuthority.Contains("/realms/"))
                {
                    metadataAuthorityUrl = metadataAuthority;
                }
                else if (!string.IsNullOrEmpty(realm))
                {
                    metadataAuthorityUrl = $"{metadataAuthority}/realms/{realm}";
                }
            }
            
            options.Authority = authorityUrl;
            
            // If a separate metadata authority is configured, use it for OIDC discovery
            // This allows the browser to redirect to localhost while the container fetches
            // metadata from a Docker-accessible hostname (e.g., host.docker.internal)
            if (!string.IsNullOrEmpty(metadataAuthorityUrl))
            {
                options.MetadataAddress = $"{metadataAuthorityUrl}/.well-known/openid-configuration";
            }

            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.CallbackPath = KeycloakDefaults.CallbackPath;

            // Disable Pushed Authorization Requests (PAR) - not all Keycloak configurations support it
            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

            // For development: optionally bypass SSL certificate validation
            // WARNING: Never use this in production!
            var dangerouslyAcceptAnyCert = Environment.GetEnvironmentVariable("SMARTSTORE_KEYCLOAK_DANGEROUSLY_ACCEPT_ANY_CERT");
            if (!string.IsNullOrEmpty(dangerouslyAcceptAnyCert) && 
                (dangerouslyAcceptAnyCert.Equals("true", StringComparison.OrdinalIgnoreCase) || dangerouslyAcceptAnyCert == "1"))
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            }

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
