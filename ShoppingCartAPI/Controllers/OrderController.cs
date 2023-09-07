using BusinessLayer.Interfaces;
using DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders(string email)
    {
        ServiceResponse<IEnumerable<OrderDetails>> response = await _orderService.GetOrdersForUser(email);
        return StatusCode((int)response.StatusCode, response);
    }
}
