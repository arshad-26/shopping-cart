using BusinessLayer.Interfaces;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using DTO.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BusinessLayer.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly JWTModel _jwtModel;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, JWTModel jwtModel, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtModel = jwtModel;
        _dbContext = dbContext;
    }

    public async Task<ServiceResponse> RegisterUser(RegisterModel registerModel)
    {
        ServiceResponse response = new();
        
        ApplicationUser user = new()
        {
            Email = registerModel.Email,
            UserName = registerModel.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, registerModel.Password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(registerModel.Role))
                await _roleManager.CreateAsync(new (registerModel.Role));

            await _userManager.AddToRoleAsync(user, registerModel.Role);

            List<Claim> claimList = new()
            {
                new ("FirstName", registerModel.FirstName),
                new ("LastName", registerModel.LastName)
            };
            
            await _userManager.AddClaimsAsync(user, claimList);

            response.StatusCode = HttpStatusCode.Created;
        }
        else
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessage = "User cannot be created";
            response.Errors = result.Errors.Select(x => x.Description).ToList();
        }

        return response;
    }

    public async Task<ServiceResponse<TokenModel>> LoginUser(LoginModel loginModel)
    {
        ServiceResponse<TokenModel> response = new();
        
        ApplicationUser? user = await _userManager.FindByNameAsync(loginModel.Email);

        if (user is not null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
        {
            List<Claim> userClaims = new (await _userManager.GetClaimsAsync(user));
            userClaims.Add(new (ClaimTypes.Email, loginModel.Email));

            IList<string> roles = await _userManager.GetRolesAsync(user);
            userClaims.Add(new (ClaimTypes.Role, roles[0])); // User can have only one role

            string token = GetToken(userClaims);
            string refreshToken = await GetRefreshToken(user.Id);

            TokenModel tokenInfo = new()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            response.ResponseData = tokenInfo;
        }
        else
        {
            response.StatusCode = HttpStatusCode.Unauthorized;
            response.ErrorMessage = "Invalid username or password";
        }

        return response;
    }

    public async Task<ServiceResponse<TokenModel>> RefreshToken(TokenModel tokenModel)
    {
        ServiceResponse<TokenModel> response = new ();
        
        string token = tokenModel.Token;
        string refreshToken = tokenModel.RefreshToken;

        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token);

        string email = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
        ApplicationUser? user = await _userManager.Users.Include(x => x.RefreshToken).FirstOrDefaultAsync(x => x.Email == email);

        string tokenExp = principal.Claims.FirstOrDefault(x => x.Type == "exp")!.Value;
        long ticks = long.Parse(tokenExp);
        DateTime tokenExpTime = GetTokenExpTime(ticks);

        if (user is null || user.RefreshToken!.Token != refreshToken || (tokenExpTime - DateTime.UtcNow).TotalMinutes > 2 || user.RefreshToken!.ExpiresAt <= DateTime.UtcNow)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ErrorMessage = "Invalid request. Please check the parameters";

            return response;
        }

        string[] userClaimKeys = new string[] { "FirstName", "LastName", ClaimTypes.Email, ClaimTypes.Role };

        List<Claim> userClaims = principal.Claims.Where(x => userClaimKeys.Contains(x.Type)).ToList();

        string newToken = GetToken(userClaims);
        string newRefreshToken = await GetRefreshToken(user.Id, true);

        TokenModel tokenInfo = new()
        {
            Token = newToken,
            RefreshToken = newRefreshToken
        };

        response.ResponseData = tokenInfo;
        return response;
    }

    public async Task<ServiceResponse<bool?>> EmailExists(string email)
    {
        ServiceResponse<bool?> response = new ();
        response.ResponseData = await _dbContext.Users.AnyAsync(x => x.Email == email);

        return response;
    }

    private string GetToken(IEnumerable<Claim> userClaims)
    {
        SymmetricSecurityKey authSigningKey = new (Encoding.UTF8.GetBytes(_jwtModel.Secret));

        JwtSecurityToken token = new (issuer: _jwtModel.ValidIssuer,
                                        audience: _jwtModel.ValidAudience,
                                        expires: DateTime.UtcNow.AddMinutes(30),
                                        claims: userClaims,
                                        signingCredentials: new (authSigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GetRefreshToken(string userId, bool GenerateNewToken = false)
    {
        string refreshToken;

        RefreshToken? existingRefreshToken = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.UserId == userId);

        if (existingRefreshToken is null)
        {
            ApplicationUser? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            RefreshToken newRefreshToken = new ();

            newRefreshToken.UserId = userId;
            newRefreshToken.Token = Guid.NewGuid().ToString();
            newRefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(8);

            user!.RefreshToken = newRefreshToken;

            refreshToken = newRefreshToken.Token;
        }
        else
        {
            if (existingRefreshToken.ExpiresAt <= DateTime.UtcNow || GenerateNewToken)
            {
                existingRefreshToken.Token = Guid.NewGuid().ToString();
                existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(8);
            }

            refreshToken = existingRefreshToken.Token;
        }

        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters validationParameters = new ()
        {
            ValidAudience = _jwtModel.ValidAudience,
            ValidIssuer = _jwtModel.ValidIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.Secret)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private DateTime GetTokenExpTime(long ticks) => DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
}
