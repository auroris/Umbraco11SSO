using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Security;

namespace Umbraco11SSO.App_Code
{
    public class IdentityServerProviderOptions : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        /// <summary>
        /// Inject stuff we need
        /// </summary>
        /// <param name="userService"></param>
        public IdentityServerProviderOptions(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        public void Configure(BackOfficeExternalLoginProviderOptions providerOptions)
        {
            providerOptions.ButtonStyle = "btn-microsoft";
            providerOptions.Icon = "fa fa-windows";
            providerOptions.DenyLocalLogin = false;
            providerOptions.AutoRedirectLoginToExternalProvider = _config.GetValue<bool>("IdentityServer:AutoRedirectLoginToExternalProvider");
            providerOptions.AutoLinkOptions = new ExternalSignInAutoLinkOptions(
                autoLinkExternalAccount: true,
                defaultUserGroups: Array.Empty<string>(),
                defaultCulture: null,
                allowManualLinking: false
            )
            {
                OnAutoLinking = OnAutoLinking,
                OnExternalLogin = OnExternalLogin
            };
        }

        public void Configure(string? name, BackOfficeExternalLoginProviderOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// When automatically linking a back office account to an external account
        /// </summary>
        /// <param name="autoLinkUser">The back office user the external account is being linked to</param>
        /// <param name="loginInfo">Information about the external account</param>
        void OnAutoLinking(BackOfficeIdentityUser autoLinkUser, ExternalLoginInfo loginInfo)
        {
            SetupUser(autoLinkUser, loginInfo);
        }

        /// <summary>
        /// When a user logins via the external login provider
        /// </summary>
        /// <param name="user">The back office user logging in</param>
        /// <param name="loginInfo">Information about the external account</param>
        /// <returns></returns>
        bool OnExternalLogin(BackOfficeIdentityUser user, ExternalLoginInfo loginInfo)
        {
            SetupUser(user, loginInfo);

            // If the user has roles, return true, else return false
            if (user.Roles.Count > 0) { return true; }
            return false;
        }

        /// <summary>
        /// Set up a user; specify their username, name, phone number, and roles
        /// </summary>
        /// <param name="user">The back office user logging in</param>
        /// <param name="loginInfo">Information about the external account</param>
        private void SetupUser(BackOfficeIdentityUser user, ExternalLoginInfo loginInfo)
        {
            user.EnableChangeTracking();

            user.UserName = (loginInfo.Principal.FindFirstValue(ClaimTypes.WindowsAccountName) ?? "\\").Split('\\')[1];
            user.Name = loginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            user.PhoneNumber = loginInfo.Principal.FindFirstValue(ClaimTypes.HomePhone);

            // If user is member of the administrators group, don't alter their roles
            if (user.Roles.FirstOrDefault(r => r.RoleId.Equals("admin")) != null)
            {
                return;
            }

            // Clear and re-add the user to any roles
            IEnumerable<IUserGroup> groups = _userService.GetAllUserGroups();
            user.Roles.Clear();

            foreach (Claim role in loginInfo.Principal.Claims.Where(c => c.Type == ClaimTypes.Role))
            {
                IUserGroup? group = groups.FirstOrDefault(g => g.Name.Equals(role.Value, StringComparison.OrdinalIgnoreCase));
                if (group != null) { user.AddRole(group.Alias); }
            }
        }
    }
}