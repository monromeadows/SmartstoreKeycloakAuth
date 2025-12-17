# Smartstore Keycloak Authentication Module

A Smartstore module that enables authentication via Keycloak using OpenID Connect.

## Compatibility

| Module Version | Smartstore Version |
|----------------|-------------------|
| 0.1.0          | 6.3.0             |

## Features

- Login with Keycloak identity provider
- OpenID Connect (OIDC) protocol support
- Configurable via Admin UI or environment variables
- Automatic user provisioning on first login

## Configuration

### Environment Variables

The module supports configuration via environment variables, which take precedence over database settings:

| Variable | Description |
|----------|-------------|
| `SMARTSTORE_KEYCLOAK_AUTHORITY` | Full authority URL (e.g., `https://keycloak.example.com/realms/myrealm`) |
| `SMARTSTORE_KEYCLOAK_REALM` | Keycloak realm name (alternative to full authority) |
| `SMARTSTORE_KEYCLOAK_CLIENTID` | OAuth client ID configured in Keycloak |
| `SMARTSTORE_KEYCLOAK_CLIENTSECRET` | OAuth client secret |

**Note:** If `SMARTSTORE_KEYCLOAK_AUTHORITY` is set, it is used directly. Otherwise, if `SMARTSTORE_KEYCLOAK_REALM` is set, the authority is constructed as `{BaseAuthority}/realms/{Realm}`.

### Admin Configuration

1. Go to **Admin > Configuration > Authentication > External authentication methods**
2. Enable the **Keycloak Authentication** provider
3. Click **Configure** and enter:
   - Authority URL (e.g., `https://keycloak.example.com/realms/myrealm`)
   - Client ID
   - Client Secret

### Keycloak Setup

1. Create a new client in your Keycloak realm
2. Set **Client authentication** to `On` (confidential client)
3. Set **Valid redirect URIs** to: `https://your-smartstore-url/signin-keycloak`
4. Copy the client secret from the **Credentials** tab

## Building

### Prerequisites

- .NET 9 SDK
- Smartstore source code (will be cloned automatically by build script)

### Build Steps

1. Clone this repository
2. Clone Smartstore source to a sibling folder (or set `SmartstoreSourcePath`):
   ```bash
   git clone --branch v6.3.0 https://github.com/smartstore/Smartstore.git ../Smartstore
   ```
3. Build the module:
   ```bash
   dotnet build src/Smartstore.Keycloak.Auth/Smartstore.Keycloak.Auth.csproj -c Release
   ```

### Output

The built module will be in:
```
src/Smartstore.Keycloak.Auth/bin/Release/
```

## Installation

1. Copy the built module folder to your Smartstore installation:
   ```
   Smartstore.Web/Modules/Smartstore.Keycloak.Auth/
   ```
2. Restart Smartstore
3. Go to Admin > Configuration > Plugins and install the module

## Docker Deployment

See the [DockerBuildSmartstore](https://github.com/monromeadows/DockerBuildSmartstore) repository for a Dockerfile that builds Smartstore with this module included.

## License

This project is licensed under the AGPL-3.0 License - see the [LICENSE](LICENSE) file for details.

## Credits

- Built for [Smartstore](https://github.com/smartstore/Smartstore)
- Based on the authentication module patterns from Smartstore's official modules
