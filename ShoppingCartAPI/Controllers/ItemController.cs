using AutoMapper;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            byte[] fileArr = System.IO.File.ReadAllBytes(x.ImagePath);
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
