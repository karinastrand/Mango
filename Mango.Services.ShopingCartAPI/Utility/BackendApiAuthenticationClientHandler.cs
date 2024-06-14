﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace Mango.Services.ShopingCartAPI.Utility;

public class BackendApiAuthenticationClientHandler :DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;
    public BackendApiAuthenticationClientHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _accessor.HttpContext.GetTokenAsync("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
