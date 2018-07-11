using Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;

namespace Api.Service
{
    /// <summary>
    /// Custom filter to open the Authorization header
    /// </summary>
    public class CheckTokenFilter : IAuthorizationFilter
    {
        /// <summary>
        /// Configuration object (appsettings.json)
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        public CheckTokenFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Event triggered on the authorization phase of the lifecycle process
        /// </summary>
        /// <param name="context">Request context</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // gets the authorization header
            StringValues authorization;
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorization) || string.IsNullOrEmpty(authorization))
            {
                // returns forbidden
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            try
            {
                // parses the token to get the claims
                TokenClaims claims = Jose.JWT.Decode<TokenClaims>(authorization.ToString().Replace("bearer ", ""));
                context.HttpContext.Request.Headers.Add("X-GitHub-Token", claims.GitHubToken);
                context.HttpContext.Request.Headers.Add("X-GitHub-User-Id", claims.User.Id);
            }
            catch (Exception ex)
            {
                // returns forbidden to force another authorization
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
        }
    }
}
