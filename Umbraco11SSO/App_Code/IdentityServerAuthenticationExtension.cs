using Microsoft.IdentityModel.Logging;
using Umbraco.Cms.Core;

namespace Umbraco11SSO.App_Code
{
    public static class IdentityServerAuthenticationExtension
    {
        public static IUmbracoBuilder AddIdentityServerBackofficeAuthentication(this IUmbracoBuilder builder, IConfiguration _config)
        {
            string SchemeLabel = _config.GetValue<string>("IdentityServer:Name") ?? "Forces";
            string SchemeName = Constants.Security.BackOfficeExternalAuthenticationTypePrefix + SchemeLabel;

            IdentityModelEventSource.ShowPII = true; // Show debugging messages during OpenID
            builder.Services.ConfigureOptions<IdentityServerProviderOptions>();

            builder.AddBackOfficeExternalLogins(logins =>
            {
                logins.AddBackOfficeLogin(
                    backOfficeAuthenticationBuilder =>
                    {
                        backOfficeAuthenticationBuilder.AddOpenIdConnect(
                            SchemeName,
                            SchemeLabel,
                            options =>
                            {
                                options.Authority = _config.GetValue<string>("IdentityServer:Authority");
                                options.ClientId = _config.GetValue<string>("IdentityServer:ClientId");
                                options.ClientSecret = _config.GetValue<string>("IdentityServer:ClientSecret");
                                options.CallbackPath = "/signin-oidc";

                                // What to ask IdentityServer for
                                options.ResponseType = "token id_token";
                                options.Scope.Add("openid");
                                options.Scope.Add("profile");
                                options.Scope.Add("email");
                                options.Scope.Add("roles");

                                // I configured IdentityServer to return all claims on the id_token so no need to go
                                // get more
                                options.GetClaimsFromUserInfoEndpoint = false;

                                // Accept any SSL or even no SSL. My intranet server isn't configured to use SSL and
                                // even if it was has no way of verifying certs. 
                                options.RequireHttpsMetadata = false;
                                options.BackchannelHttpHandler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                                };
                            });
                    });
            });

            return builder;
        }
    }
}