# Umbraco11SSO
This is the Umbraco 11.4 sample using an OpenIddict implementation to provide Active Directory authentication for back office users. You can find the github project for that component here: https://github.com/auroris/OpenIddict-WindowsAuth

# Installation
You will need to run Umbraco's normal installation routine and set up a database with an administrative account.

# Doing Single Sign-On
SSO will work outside of an Active Directory environment for testing purposes. OpenIddict-WindowsAuth will use your local machine account.

* Run both Umbraco11SSO and OpenIddict-WindowsAuth. 
* Access Umbraco using Microsoft Edge or Google Chrome at https://localhost:44307/umbraco/#/login/false
* Click "Sign in with CompanyName."
* Your first sign-in attempt will fail because your Umbraco account will not be assigned to any useful groups. 
* Return to the login form using the link above and sign in using your administrator account.
* Assign your SSO Umbraco account a valid group (such as administrators), and also enable the account.
* Log out and then log in using your SSO account by clicking "Sign in with CompanyName."

# Configuration
Configuration is via the IdentityServer section in appsettings.json.

* Authority: the URI to where you installed OpenIddict-WindowsAuth. If you downloaded and ran the project in Visual Studio without changing anything, the Uri will be https://localhost:44353. You can verify this is correct by accessing the the OpenId configuration document at https://localhost:44353/.well-known/openid-configuration
* Name: the name of your organization.
* ClientId: the name of your client. If you're using unmodified OpenIddict-WindowsAuth, then it doesn't care and this value can be anything.
* ClientSecret: a shared secret for your OpenId client. Again, if you're using unmodified OpenIddict-WIndowsAuth, this can be anything.
* AutoRedirectLoginToExternalProvider: If set to true, Umbraco will try to skip the login form and proceed with authenticating with the OpenId server.
