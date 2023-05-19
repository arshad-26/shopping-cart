using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorUI.Providers;

public class AuthenticationProvider : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorage;

    public AuthenticationProvider(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public void StateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await _sessionStorage.GetItemAsync<string>("Token");

        ClaimsIdentity identity = String.IsNullOrWhiteSpace(token) ? new() : GetIdentityFromToken(token);

        return await Task.FromResult(new AuthenticationState(new(identity)));
    }

    private ClaimsIdentity GetIdentityFromToken(string token)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);

        IEnumerable<Claim> claimList = jwtSecurityToken.Claims.Where(x => x.Type == ClaimTypes.Email || x.Type == ClaimTypes.Role || x.Type == "exp");
        ClaimsIdentity identity = new(claimList, "jwt");

        return identity;
    }
}
