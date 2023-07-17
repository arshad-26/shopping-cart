using AutoMapper;
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
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetItems()
    {
        List<Category> tabsContentDb = _dbContext.Category.Include(x => x.Items).ToList();
        List<CategoryModel> tabsContent = _mapper.Map<List<CategoryModel>>(tabsContentDb);

        tabsContent.Select(x => x.Items!).SelectMany(x => x).ToList().ForEach(item =>
        {
            using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
            using Image thumbnail = itemImg.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);

            byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
            item.Base64Img = Convert.ToBase64String(fileArr);
        });

        return Ok(tabsContent);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(List<CartModel> cartItems)
    {
        string email = User.FindFirstValue(ClaimTypes.Email)!;
        ApplicationUser user = (await _userManager.FindByNameAsync(email))!;

        List<OrderItem> orderItems = new ();
        
        foreach(CartModel item in cartItems)
        {
            Item dbItem = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == item.ItemID))!;
            
            OrderItem orderedItem = new ();
            orderedItem.Item = dbItem;
            orderedItem.Quantity = item.Quantity;

            orderItems.Add(orderedItem);
        }

        Order order = new() { User = user, OrderItems = orderItems };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
