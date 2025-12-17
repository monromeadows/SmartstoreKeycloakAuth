# Smartstore Keycloak Authentication Module

A Smartstore module that enables authentication via Keycloak using OpenID Connect.

## Compatibility

| Module Version | Smartstore Version |
|----------------|-------------------|
| 0.1.x          | 6.3.0             |

## Features

- Login with Keycloak identity provider
- OpenID Connect (OIDC) protocol support
- Configurable via Admin UI or environment variables
- Automatic user provisioning on first login

## Building

Smartstore modules are designed to be built within the Smartstore source tree. This ensures proper dependency resolution and build configuration.

### Prerequisites

- .NET 9 SDK
- Smartstore 6.3.0 source code

### Build Steps

1. Clone Smartstore:
   ```bash
   git clone --branch 6.3.0 https://github.com/smartstore/Smartstore.git
   cd Smartstore
   ```

2. Clone this module into the Smartstore Modules folder:
   ```bash
   git clone https://github.com/monromeadows/SmartstoreKeycloakAuth.git src/Smartstore.Modules/Smartstore.Keycloak.Auth
   ```

   **Alternative (symlink for development):**
   ```bash
   # Keep module source separate, create symlink
   ln -s /path/to/your/SmartstoreKeycloakAuth/src/Smartstore.Keycloak.Auth src/Smartstore.Modules/Smartstore.Keycloak.Auth-sym
   ```

3. Build the module:
   ```bash
   dotnet build src/Smartstore.Modules/Smartstore.Keycloak.Auth/Smartstore.Keycloak.Auth.csproj -c Release
   ```

### Build Output

The built module will be in:
```
src/Smartstore.Web/Modules/Smartstore.Keycloak.Auth/
```

This folder contains:
- `Smartstore.Keycloak.Auth.dll` - Module assembly
- `Microsoft.AspNetCore.Authentication.OpenIdConnect.dll` - Private dependency
- `module.json` - Module manifest
- `Views/` - Razor views
- `Localization/` - Resource files

## Installation

### From Build Output

1. Copy the entire `Smartstore.Keycloak.Auth/` folder to your Smartstore installation's `Modules/` directory
2. Restart Smartstore
3. Go to **Admin > Plugins** and install the module

### Using Smartstore Packager (Recommended for Distribution)

1. Open `Smartstore.Tools.sln` in the Smartstore source
2. Build and run `Smartstore.Packager`
3. Select the module and create a package
4. Upload the resulting `.zip` file via **Admin > Plugins > Upload Plugin**

## Configuration

### Admin Configuration

1. Go to **Admin > Configuration > Authentication > External authentication methods**
2. Enable the **Keycloak Authentication** provider
3. Click **Configure** and enter:
   - **Authority:** Base Keycloak URL (e.g., `https://keycloak.example.com`)
   - **Realm:** Your Keycloak realm name
   - **Client ID:** OAuth client ID
   - **Client Secret:** OAuth client secret

### Environment Variables (Optional)

For containerized deployments, configuration can be provided via environment variables:

| Variable | Description |
|----------|-------------|
| `SMARTSTORE_KEYCLOAK_AUTHORITY` | Full OIDC authority URL (e.g., `https://keycloak.example.com/realms/myrealm`) |
| `SMARTSTORE_KEYCLOAK_REALM` | Realm name (only if `AUTHORITY` doesn't include `/realms/`) |
| `SMARTSTORE_KEYCLOAK_CLIENTID` | OAuth client ID |
| `SMARTSTORE_KEYCLOAK_CLIENTSECRET` | OAuth client secret |

Environment variables take precedence over database settings.

### Keycloak Client Setup

1. Create a new client in your Keycloak realm
2. Set **Client authentication** to `On` (confidential client)
3. Set **Valid redirect URIs** to: `https://your-smartstore-url/signin-keycloak`
4. Copy the client secret from the **Credentials** tab

## Docker Deployment

See the [DockerBuilders/BuildSmartstore](https://github.com/monromeadows/DockerBuilders/tree/master/BuildSmartstore) directory for a Dockerfile that builds Smartstore with this module included.

## License

This project is licensed under the AGPL-3.0 License - see the [LICENSE](LICENSE) file for details.

## Credits

- Built for [Smartstore](https://github.com/smartstore/Smartstore)
- Based on the authentication module patterns from Smartstore's official modules
