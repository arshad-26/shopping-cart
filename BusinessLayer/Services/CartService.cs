using AutoMapper;
using BusinessLayer.Interfaces;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    
    public CartService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<IEnumerable<CategoryModel>>> GetItems()
    {
        ServiceResponse<IEnumerable<CategoryModel>> response = new ();

        List<CategoryModel> tabsContent = await _dbContext.Category.AsQueryable().GroupJoin(_dbContext.Item.AsQueryable(),
                                                        c => c.CategoryID,
                                                        i => i.CategoryID,
                                                        (category, items) => new CategoryModel()
                                                        {
                                                            CategoryID = category.CategoryID,
                                                            Name = category.Name,
                                                            Items = _mapper.Map<IEnumerable<ItemModel>>(items)
                                                        }).ToListAsync();

        tabsContent.SelectMany(x => x.Items!).ToList().ForEach(item =>
        {
            using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
            using Image thumbnail = itemImg.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);

            byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
            item.Base64Img = Convert.ToBase64String(fileArr);
        });

        response.ResponseData = tabsContent;
        return response;
    }

    public async Task<ServiceResponse> PlaceOrder(string email, IEnumerable<CartModel> cartItems)
    {
        ServiceResponse response = new();
        
        ApplicationUser user = (await _userManager.FindByNameAsync(email))!;

        List<OrderItem> orderItems = new();

        foreach (CartModel item in cartItems)
        {
            Item dbItem = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == item.ItemID))!;

            OrderItem orderedItem = new();
            orderedItem.Item = dbItem;
            orderedItem.Quantity = item.Quantity;

            orderItems.Add(orderedItem);
        }

        Order order = new() { User = user, OrderItems = orderItems };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        return response;
    }
}
