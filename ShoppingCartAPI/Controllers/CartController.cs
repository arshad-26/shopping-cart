using AutoMapper;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "User")]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CartController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
}
