# Umbraco11SSO
This is the Umbraco 11.4 sample using an OpenIddict implementation to provide Active Directory authentication for back office users. You can find the github project for that component here: https://github.com/auroris/OpenIddict-WindowsAuth

# Installation
Umbraco will need to set up a database.

# Logging in
* Run both Umbraco11SSO and OpenIddict-WindowsAuth. 
* Access Umbraco using Microsoft Edge or Google Chrome at https://localhost:44307/umbraco/#/login/false
* Click "Sign in with CompanyName."
* Your first sign-in attempt will fail because your Umbraco account will not be assigned to any useful groups. 
* Return to the login form using the link above and sign in using your administrator account.
* Assign your SSO Umbraco account a valid group (such as administrators), and also enable the account.
* Log out and then log in using your SSO account by clicking "Sign in with CompanyName."
