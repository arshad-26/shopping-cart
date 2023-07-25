using AutoMapper;
using BusinessLayer.Interfaces;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ItemService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<CategoryModel>>> GetCategories()
    {
        ServiceResponse<IEnumerable<CategoryModel>> response = new ();

        List<CategoryModel> categories = await _dbContext.Category.AsQueryable().GroupJoin(_dbContext.Item.AsQueryable(),
                                                                     c => c.CategoryID,
                                                                     i => i.CategoryID,
                                                                     (category, items) => new CategoryModel()
                                                                     {
                                                                         CategoryID = category.CategoryID,
                                                                         Name = category.Name,
                                                                         CanBeDeleted = items == null || items.Count() == 0
                                                                     }).ToListAsync();

        response.ResponseData = categories;

        return response;
    }

    public async Task<ServiceResponse> AddCategory(CategoryModel categoryModel)
    {
        ServiceResponse response = new ();

        Category category = _mapper.Map<Category>(categoryModel);
        await _dbContext.Category.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        response.StatusCode = HttpStatusCode.Created;
        return response;
    }

    public async Task<ServiceResponse> DeleteCategory(int categoryID)
    {
        ServiceResponse response = new ();

        Category? category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == categoryID);

        _dbContext.Category.Remove(category!);
        await _dbContext.SaveChangesAsync();

        response.StatusCode = HttpStatusCode.NoContent;
        return response;
    }

    public async Task<ServiceResponse<bool?>> CategoryExists(string category)
    {
        ServiceResponse<bool?> response = new ();

        response.ResponseData = await _dbContext.Category.AnyAsync(x => x.Name == category);
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<ItemModel>>> GetItems()
    {
        ServiceResponse<IEnumerable<ItemModel>> response = new ();

        List<ItemModel> totalItems = await _dbContext.Item.AsQueryable().Join(_dbContext.Category.AsQueryable(),
                                                            item => item.CategoryID,
                                                            category => category.CategoryID,
                                                            (item, category) => new ItemModel()
                                                            {
                                                                ID = item.ID,
                                                                Name = item.Name,
                                                                Price = item.Price,
                                                                Category = category.Name,
                                                                ImagePath = item.ImagePath
                                                            }).ToListAsync();

        totalItems.ForEach(item =>
        {
            {
                using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
                using Image thumbnail = itemImg.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

                byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
                item.Base64Img = Convert.ToBase64String(fileArr);
            }
        });

        response.ResponseData = totalItems;
        return response;
    }

    public async Task<ServiceResponse> AddItem(ItemAPIModel itemModel)
    {
        ServiceResponse response = new ();

        Category categoryEntity = (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;
        string category = categoryEntity.Name;

        Item item = new()
        {
            Name = itemModel.Name,
            Price = itemModel.Price,
            Category = categoryEntity
        };

        await SaveFileAsync(category, itemModel.UploadedFile!, item);

        await _dbContext.Item.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        response.StatusCode = HttpStatusCode.NoContent;
        return response;
    }

    public async Task<ServiceResponse<ItemModel>> EditItem(long ID)
    {
        ServiceResponse<ItemModel> response = new ();
        
        Item item = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == ID))!;
        ItemModel itemModel = _mapper.Map<ItemModel>(item);

        {
            using Image itemImg = Image.FromFile(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
            using Image thumbnail = itemImg.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

            byte[] fileArr = (byte[])new ImageConverter().ConvertTo(thumbnail, typeof(byte[]))!;
            itemModel.Base64Img = Convert.ToBase64String(fileArr);
        }

        response.ResponseData = itemModel;
        return response;
    }

    public async Task<ServiceResponse> EditItem(ItemAPIModel itemModel)
    {
        ServiceResponse response = new ();
        
        Item item = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == itemModel.ID))!;
        Category category = (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;

        item.Name = itemModel.Name;
        item.Price = itemModel.Price;
        item.Category = category;

        System.IO.File.Delete(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
        await SaveFileAsync(category.Name, itemModel.UploadedFile!, item);

        await _dbContext.SaveChangesAsync();

        return response;
    }

    public async Task<ServiceResponse> DeleteItem(long ID)
    {
        ServiceResponse response = new();

        Item selectedItem = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == ID))!;
        string relativeImgPath = selectedItem.ImagePath;

        System.IO.File.Delete(Path.GetFullPath("ItemImages") + "\\" + relativeImgPath);

        _dbContext.Item.Remove(selectedItem);
        await _dbContext.SaveChangesAsync();

        return response;
    }

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
