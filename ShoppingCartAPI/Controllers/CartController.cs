using AutoMapper;
using BusinessLayer.Interfaces;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using DTO.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        ServiceResponse<IEnumerable<CategoryModel>> response = await _cartService.GetItems();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(List<CartModel> cartItems)
    {
        string email = User.FindFirstValue(ClaimTypes.Email)!;

        ServiceResponse response = await _cartService.PlaceOrder(email, cartItems);
        return StatusCode((int)response.StatusCode, response);
    }
}
