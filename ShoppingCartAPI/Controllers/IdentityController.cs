using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DAL.Context;
using ShoppingCartAPI.Models;
using DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTO.Identity;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class IdentityController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ApplicationDbContext _dbContext;
	private readonly JWTModel _jwtModel;

	public IdentityController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext, JWTModel jwtModel)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_dbContext = dbContext;
		_jwtModel = jwtModel;
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterModel registerModel)
	{
		ApplicationUser user = new()
		{
			Email= registerModel.Email,
			UserName = registerModel.Email
		};

		IdentityResult result = await _userManager.CreateAsync(user, registerModel.Password);

		if (result.Succeeded)
		{
			if (!await _roleManager.RoleExistsAsync(registerModel.Role))
				await _roleManager.CreateAsync(new IdentityRole(registerModel.Role));

			await _userManager.AddToRoleAsync(user, registerModel.Role);

			List<Claim> claimList = new ()
			{
				new ("FirstName", registerModel.FirstName),
				new ("LastName", registerModel.LastName)
			};
			await _userManager.AddClaimsAsync(user, claimList);

			return Ok();
		}
		else
		{
			return BadRequest(result.Errors);
		}
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginModel loginModel)
	{
		ApplicationUser? user = await _userManager.FindByNameAsync(loginModel.Email);

		if(user is not null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
		{
			List<Claim> userClaims = new (await _userManager.GetClaimsAsync(user));
			userClaims.Add(new Claim(ClaimTypes.Email, loginModel.Email));

			IList<string> roles = await _userManager.GetRolesAsync(user);
			userClaims.Add(new Claim(ClaimTypes.Role, roles[0])); // User can have only one role

			string token = GetToken(userClaims);
			string refreshToken = await GetRefreshToken(user.Id);

			return Ok(new { token, refreshToken });
		}

		return Unauthorized();
	}

	[HttpPost]
	public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
	{
		string token = tokenModel.Token;
		string refreshToken = tokenModel.RefreshToken;

		ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token);

		string email = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!.Value;
		ApplicationUser? user = await _userManager.Users.Include(x => x.RefreshToken).FirstOrDefaultAsync(x => x.Email == email);

		string tokenExp = principal.Claims.FirstOrDefault(x => x.Type == "exp")!.Value;
		long ticks = long.Parse(tokenExp);
		DateTime tokenExpTime = GetTokenExpTime(ticks);

		if(user is null || user.RefreshToken!.Token != refreshToken || tokenExpTime > DateTime.UtcNow || user.RefreshToken!.ExpiresAt <= DateTime.UtcNow)
		{
			return BadRequest();
		}

		string[] userClaimKeys = new string[] { "FirstName", "LastName", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" };

		List<Claim> userClaims = principal.Claims.Where(x => userClaimKeys.Contains(x.Type)).ToList();

		string newToken = GetToken(userClaims);
		string newRefreshToken = await GetRefreshToken(user.Id, true);

		return Ok(new { newToken, newRefreshToken });
	}

	[HttpGet]
	public async Task<bool> EmailExists(string email) => await _dbContext.Users.AnyAsync(x => x.Email == email);

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

		if(existingRefreshToken is null)
		{
			ApplicationUser? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

			RefreshToken newRefreshToken = new RefreshToken();

			newRefreshToken.UserId = userId;
			newRefreshToken.Token = Guid.NewGuid().ToString();
			newRefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(8);

			user!.RefreshToken = newRefreshToken;

			refreshToken = newRefreshToken.Token;
		}
		else
		{
			if(existingRefreshToken.ExpiresAt <= DateTime.UtcNow || GenerateNewToken)
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
		TokenValidationParameters validationParameters = new()
		{
			ValidAudience = _jwtModel.ValidAudience,
			ValidIssuer = _jwtModel.ValidIssuer,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.Secret)),
			ValidateLifetime = false
		};

		JwtSecurityTokenHandler tokenHandler = new ();

		ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

		if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			throw new SecurityTokenException("Invalid token");

		return principal;
	}

	private DateTime GetTokenExpTime(long ticks) => DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
}
