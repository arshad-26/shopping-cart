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
        List<Category> categoriesDb = _dbContext.Category.Include(x => x.Items).ToList();
        List<CategoryModel> categories = new ();

        categoriesDb.ForEach(entity =>
        {
            CategoryModel category = _mapper.Map<CategoryModel>(entity);
            category.CanBeDeleted = entity.Items == null || entity.Items.Count == 0;

            categories.Add(category);
        });

        return Ok(categories);
    }

    [HttpGet]
    public IActionResult GetItems()
    {
        IEnumerable<ItemModel> items = _dbContext.Item.Include(x => x.Category).AsEnumerable().Select(x =>
        {
            ItemModel item = _mapper.Map<ItemModel>(x);
            item.Category = x.Category.Name;

            {
                using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + x.ImagePath);
                using Image thumbnail = itemImg.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

                byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
                item.Base64Img = Convert.ToBase64String(fileArr);
            }
            
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
    public async Task<IActionResult> AddItem([FromForm] ItemAPIModel itemModel)
    {
        Category categoryDb = (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;
        string category = categoryDb.Name;

        Item item = new()
        {
            Name = itemModel.Name,
            Price = itemModel.Price,
            Category = categoryDb
        };

        await SaveFileAsync(category, itemModel.UploadedFile!, item);

        await _dbContext.Item.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> EditItem(long ID)
    {
        Item item = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == ID))!;
        ItemModel itemModel = _mapper.Map<ItemModel>(item);

        {
            using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
            using Image thumbnail = itemImg.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

            byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
            itemModel.Base64Img = Convert.ToBase64String(fileArr);
        }

        return Ok(itemModel);
    }

    [HttpPut]
    public async Task<IActionResult> EditItem([FromForm] ItemAPIModel itemModel)
    {
        Item item = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == itemModel.ID))!;
        Category category = (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;

        item.Name = itemModel.Name;
        item.Price = itemModel.Price;
        item.Category = category;

        System.IO.File.Delete(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
        await SaveFileAsync(category.Name, itemModel.UploadedFile!, item);

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

    [HttpDelete]
    public async Task<IActionResult> DeleteItem(long ID)
    {
        Item selectedItem = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == ID))!;
        string relativeImgPath = selectedItem.ImagePath;

        System.IO.File.Delete(Path.GetFullPath("ItemImages") + "\\" + relativeImgPath);

        _dbContext.Item.Remove(selectedItem);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<bool> CategoryExists(string category) => await _dbContext.Category.AnyAsync(x => x.Name == category);

    private async Task SaveFileAsync(string categoryName, IFormFile uploadFile, Item itemEntity)
    {
        string categoryPath = Path.GetFullPath("ItemImages") + "\\" + categoryName;

        if (!Directory.Exists(categoryPath))
            Directory.CreateDirectory(categoryPath);

        string fileExtension = Path.GetExtension(uploadFile.FileName);
        string fileName = Guid.NewGuid().ToString() + fileExtension;
        string filePath = Path.GetFullPath(categoryPath + "\\" + fileName);

        {
            using MemoryStream stream = new();
            await uploadFile.CopyToAsync(stream);
            using Image img = Image.FromStream(stream);
            img.Save(filePath);
        }

        itemEntity.ImagePath = categoryName + "\\" + fileName;
    }
}
