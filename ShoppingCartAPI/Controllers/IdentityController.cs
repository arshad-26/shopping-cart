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
using BusinessLayer.Interfaces;
using DTO.Common;
using System.Net;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class IdentityController : ControllerBase
{
	private readonly IAuthService _authService;

	public IdentityController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterModel registerModel)
	{
		ServiceResponse response = await _authService.RegisterUser(registerModel);
		return StatusCode((int)response.StatusCode, response);
    }

	[HttpPost]
	public async Task<IActionResult> Login(LoginModel loginModel)
	{
		ServiceResponse<TokenModel> response = await _authService.LoginUser(loginModel);
        return StatusCode((int)response.StatusCode, response);
    }

	[HttpPost]
	public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
	{
		ServiceResponse<TokenModel> response = await _authService.RefreshToken(tokenModel);
        return StatusCode((int)response.StatusCode, response);
    }

	[HttpGet]
	public async Task<IActionResult> EmailExists(string email)
	{
		ServiceResponse<bool?> response = await _authService.EmailExists(email);
        return StatusCode((int)response.StatusCode, response);
    }
}
