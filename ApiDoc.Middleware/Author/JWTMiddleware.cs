﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Middleware
{
    public class JWTMiddleware
    {
        // Property key is used by Endpoint routing to determine if Authorization has run
        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

        private readonly RequestDelegate _next;
        private readonly IAuthorizationPolicyProvider _policyProvider;

        public JWTMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
                // The Authorization middleware can only make this claim if it observes an actual endpoint.
                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
            }

            // 通过终结点路由元素IAuthorizeData来获得对于的AuthorizeAttribute并关联到AuthorizeFilter中
            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
            var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
            if (policy == null)
            {
                await _next(context);
                return;
            }

            // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

            // Allow Anonymous skips all authorization
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // Note that the resource will be null if there is no matched endpoint
            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource: endpoint);

            if (authorizeResult.Challenged)
            {
                if (policy.AuthenticationSchemes.Any())
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await context.ChallengeAsync();
                }

                return;
            }
            else if (authorizeResult.Forbidden)
            {
                if (policy.AuthenticationSchemes.Any())
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ForbidAsync(scheme);
                    }
                }
                else
                {
                    await context.ForbidAsync();
                }

                return;
            }

            await _next(context);
        }
    }
}
