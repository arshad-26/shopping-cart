using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSample()
    {
        return Ok("Hello World");
    }
}
