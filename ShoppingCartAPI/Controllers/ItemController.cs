using AutoMapper;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ItemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ItemController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    [HttpGet]
    public IActionResult GetCategories()
    {
        List<Category> categoriesDb = _dbContext.Category.ToList();
        List<CategoryModel> categories = _mapper.Map<List<CategoryModel>>(categoriesDb);

        return Ok(categories);
    }

    [HttpGet]
    public IActionResult GetItems()
    {
        IEnumerable<ItemModel> items = _dbContext.Item.Include(x => x.Category).AsEnumerable().Select(x =>
        {
            ItemModel item = _mapper.Map<ItemModel>(x);
            item.Category = x.Category.Name;

            Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + x.ImagePath);
            Image thumbnail = itemImg.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            
            byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
            item.Base64Img = Convert.ToBase64String(fileArr);

            return item;
        });

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryModel categoryModel)
    {
        Category category = _mapper.Map<Category>(categoryModel);
        
        await _dbContext.Category.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromForm] ItemModel itemModel)
    {
        Category categoryDb = (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;
        string category = categoryDb.Name;

        string categoryPath = Path.GetFullPath("ItemImages") + "\\" + category;

        if(!Directory.Exists(categoryPath))
            Directory.CreateDirectory(categoryPath);

        string fileExtension = Path.GetExtension(itemModel.UploadedFile!.FileName);
        string fileName = Guid.NewGuid().ToString() + fileExtension;
        string filePath = Path.GetFullPath(categoryPath + "\\" + fileName);

        using MemoryStream stream = new ();
        await itemModel.UploadedFile!.CopyToAsync(stream);
        using Image img = Image.FromStream(stream);
        img.Save(filePath);

        Item item = new ()
        {
            Name = itemModel.Name,
            Price = itemModel.Price,
            Category = categoryDb,
            ImagePath = category + "\\" + fileName
        };

        await _dbContext.Item.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int categoryID)
    {
        Category? category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == categoryID);

        _dbContext.Category.Remove(category!);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<bool> CategoryExists(string category) => await _dbContext.Category.AnyAsync(x => x.Name == category);
}
