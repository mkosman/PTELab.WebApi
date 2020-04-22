using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;

namespace PTELab.WebApi.AuthHandler
{
    public static class BasicAuthenticationAppBuilderExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
            => builder.AddBasic(BasicAuthenticationDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddBasic(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
            => builder.AddBasic(BasicAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddBasic(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<AuthenticationSchemeOptions> configureOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
    }
