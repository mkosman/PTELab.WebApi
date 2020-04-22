using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace PTELab.WebApi.AuthHandler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _scheme = "Basic";

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault(header => header.StartsWith(_scheme, StringComparison.OrdinalIgnoreCase));
            if(string.IsNullOrEmpty(authHeader))
            {
                Logger.LogTrace("No auth header");
                return AuthenticateResult.Fail("No auth header");
            }
            
            var encodedCredentials = authHeader.Substring(_scheme.Length).Trim();
            
            if (string.IsNullOrEmpty(encodedCredentials))
            {
                const string noCredentialsMessage = "No credentials";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            try
            {
                string decodedCredentials;
                byte[] base64DecodedCredentials;
                try
                {
                    base64DecodedCredentials = Convert.FromBase64String(encodedCredentials);
                }
                catch (FormatException)
                {
                    const string failedToDecodeCredentials = "Cannot convert credentials from Base64.";
                    Logger.LogInformation(failedToDecodeCredentials);
                    return AuthenticateResult.Fail(failedToDecodeCredentials);
                }

                try
                {
                    decodedCredentials = Encoding.UTF8.GetString(base64DecodedCredentials);
                }
                catch (Exception ex)
                {
                    const string failedToDecodeCredentials = "Cannot build credentials from decoded base64 value, exception {0} encountered.";
                    var logMessage = string.Format(CultureInfo.InvariantCulture, failedToDecodeCredentials, ex.Message);
                    Logger.LogInformation(logMessage);
                    return AuthenticateResult.Fail(logMessage);
                }


                var delimiterIndex = decodedCredentials.IndexOf(":", StringComparison.OrdinalIgnoreCase);
                if (delimiterIndex == -1)
                {
                    const string missingDelimiterMessage = "Invalid credentials, missing delimiter.";
                    Logger.LogInformation(missingDelimiterMessage);
                    return AuthenticateResult.Fail(missingDelimiterMessage);
                }

                var username = decodedCredentials.Substring(0, delimiterIndex);
                var password = decodedCredentials.Substring(delimiterIndex + 1);

                // todo:: validate user and password in db or identity
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || !(username.Equals("admin") && password.Equals("admin")))
                {
                    const string missingDelimiterMessage = "Invalid credentials.";
                    Logger.LogInformation(missingDelimiterMessage);
                    return AuthenticateResult.Fail(missingDelimiterMessage);
                }

                var claims = new[] { new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, "localhost") };
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "localhost"));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Authorization");
                throw;
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (!Request.IsHttps)
            {
                const string insecureProtocolMessage = "Request is HTTP, Basic Authentication will not respond.";
                Logger.LogInformation(insecureProtocolMessage);
                // 421 Misdirected Request
                // The request was directed at a server that is not able to produce a response.
                // This can be sent by a server that is not configured to produce responses for the combination of scheme and authority that are included in the request URI.
                Response.StatusCode = StatusCodes.Status421MisdirectedRequest;
            } else
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                Response.Headers.Append(HeaderNames.WWWAuthenticate, $"{_scheme} realm=\"localhost\"");
            }

            return Task.CompletedTask;
        }
    }
}