using Microsoft.AspNetCore.Mvc;
using Smartstore.ComponentModel;
using Smartstore.Core.Security;
using Smartstore.Engine.Modularity;
using Smartstore.Keycloak.Auth.Models;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling.Settings;

namespace Smartstore.Keycloak.Auth.Controllers
{
    public class KeycloakAuthController : AdminController
    {
        private readonly IOptionsMonitorCache<OpenIdConnectOptions> _optionsCache;
        private readonly IProviderManager _providerManager;

        public KeycloakAuthController(IOptionsMonitorCache<OpenIdConnectOptions> optionsCache, IProviderManager providerManager)
        {
            _optionsCache = optionsCache;
            _providerManager = providerManager;
        }

        [HttpGet, LoadSetting]
        [Permission(Permissions.Configuration.Authentication.Read)]
        public IActionResult Configure(KeycloakExternalAuthSettings settings)
        {
            var model = MiniMapper.Map<KeycloakExternalAuthSettings, ConfigurationModel>(settings);

            var host = Services.StoreContext.CurrentStore.GetBaseUrl();
            model.RedirectUrl = $"{host}signin-keycloak";

            ViewBag.Provider = _providerManager.GetProvider("Smartstore.Keycloak.Auth").Metadata;

            return View(model);
        }

        [HttpPost, SaveSetting]
        [Permission(Permissions.Configuration.Authentication.Update)]
        public IActionResult Configure(ConfigurationModel model, KeycloakExternalAuthSettings settings, int storeScope)
        {
            if (!ModelState.IsValid)
            {
                return Configure(settings);
            }

            ModelState.Clear();
            MiniMapper.Map(model, settings);

            // Clear cached options to pick up new settings.
            _optionsCache.TryRemove(KeycloakDefaults.AuthenticationScheme);

            NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));

            return RedirectToAction(nameof(Configure));
        }
    }
}
