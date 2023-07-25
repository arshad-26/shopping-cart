using Blazored.SessionStorage;
using DTO.Common;
using DTO.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using Toolbelt.Blazor;

namespace BlazorUI.Interceptors;

public class AuthInterceptor
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ISessionStorageService _sessionStorage;
    private readonly IHttpClientFactory _clientFactory;

    public AuthInterceptor(HttpClientInterceptor interceptor, AuthenticationStateProvider authProvider, ISessionStorageService sessionStorage, IHttpClientFactory clientFactory)
    {
        _interceptor = interceptor;
        _authProvider = authProvider;
        _sessionStorage = sessionStorage;
        _clientFactory = clientFactory;
    }

    public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

    public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        AuthenticationState authState = await _authProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (user?.Claims?.Count() > 0)
        {
            string exp = user.FindFirst(c => c.Type.Equals("exp"))!.Value;
            DateTimeOffset expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));

            DateTime timeUTC = DateTime.UtcNow;

            if((expTime - timeUTC).TotalMinutes <= 2)
            {
                string expiredToken = await _sessionStorage.GetItemAsync<string>("Token");
                string refreshToken = await _sessionStorage.GetItemAsync<string>("RefreshToken");

                HttpClient httpClient = _clientFactory.CreateClient("RefreshAPI");
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("Identity/RefreshToken", new TokenModel() { Token = expiredToken, RefreshToken = refreshToken });

                response.EnsureSuccessStatusCode();

                ServiceResponse<TokenModel>? newTokenInfo = await response.Content.ReadFromJsonAsync<ServiceResponse<TokenModel>>();

                await _sessionStorage.SetItemAsync("Token", newTokenInfo!.ResponseData!.Token);
                await _sessionStorage.SetItemAsync("RefreshToken", newTokenInfo!.ResponseData!.RefreshToken);
            }

            string token = await _sessionStorage.GetItemAsync<string>("Token");

            e.Request.Headers.Authorization = new ("Bearer", token);
        }
    }

    public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
}
