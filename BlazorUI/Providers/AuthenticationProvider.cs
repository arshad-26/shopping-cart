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

        ClaimsIdentity identity = String.IsNullOrWhiteSpace(token) ? new ClaimsIdentity() : GetIdentityFromToken(token);

        return await Task.FromResult(new AuthenticationState(new(identity)));
    }

    private ClaimsIdentity GetIdentityFromToken(string token)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);

        IEnumerable<Claim> claimList = jwtSecurityToken.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || x.Type == "exp");
        ClaimsIdentity identity = new(claimList, "jwt");

        return identity;
    }
}
