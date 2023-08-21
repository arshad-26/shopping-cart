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
using Repositories.Interface;

namespace BusinessLayer.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IBaseEntityRepository<Category> _categoryRepository;
    private readonly IBaseEntityRepository<Item> _itemRepository;
    private readonly IMapper _mapper;

    public ItemService(ApplicationDbContext dbContext,
        IBaseEntityRepository<Category> categoryRepository,
        IBaseEntityRepository<Item> itemRepository,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<CategoryModel>>> GetCategories()
    {
        ServiceResponse<IEnumerable<CategoryModel>> response = new ();

        List<CategoryModel> categories = await _categoryRepository.GetAll().GroupJoin(_itemRepository.GetAll(),
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
        await _categoryRepository.AddAsync(category);

        response.StatusCode = HttpStatusCode.Created;
        return response;
    }

    public async Task<ServiceResponse> DeleteCategory(int categoryID)
    {
        ServiceResponse response = new ();

        Category? category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == categoryID);
        await _categoryRepository.RemoveAsync(category!);

        response.StatusCode = HttpStatusCode.NoContent;
        return response;
    }

    public async Task<ServiceResponse<bool?>> CategoryExists(string category)
    {
        ServiceResponse<bool?> response = new ();

        response.ResponseData = await _categoryRepository.AnyAsync(x => x.Name == category);
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<ItemModel>>> GetItems()
    {
        ServiceResponse<IEnumerable<ItemModel>> response = new ();

        List<ItemModel> totalItems = await _itemRepository.GetAll().Join(_categoryRepository.GetAll(),
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

        Category? categoryEntity = await _categoryRepository.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID);
        string category = categoryEntity!.Name;

        Item item = new()
        {
            Name = itemModel.Name,
            Price = itemModel.Price,
            Category = categoryEntity
        };

        await SaveFileAsync(category, itemModel.UploadedFile!, item);

        await _itemRepository.AddAsync(item);

        response.StatusCode = HttpStatusCode.NoContent;
        return response;
    }

    public async Task<ServiceResponse<ItemModel>> EditItem(long ID)
    {
        ServiceResponse<ItemModel> response = new ();
        
        Item item = (await _itemRepository.FirstOrDefaultAsync(x => x.ID == ID))!;
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
        
        Item item = (await _itemRepository.FirstOrDefaultAsync(x => x.ID == itemModel.ID))!;
        Category category = (await _categoryRepository.FirstOrDefaultAsync(x => x.CategoryID == itemModel.CategoryID))!;

        item.Name = itemModel.Name;
        item.Price = itemModel.Price;
        item.Category = category;

        File.Delete(Path.GetFullPath("ItemImages") + "\\" + item.ImagePath);
        await SaveFileAsync(category.Name, itemModel.UploadedFile!, item);

        await _itemRepository.UpdateAsync(item);

        return response;
    }

    public async Task<ServiceResponse> DeleteItem(long ID)
    {
        ServiceResponse response = new();

        Item selectedItem = (await _dbContext.Item.FirstOrDefaultAsync(x => x.ID == ID))!;
        string relativeImgPath = selectedItem.ImagePath;

        File.Delete(Path.GetFullPath("ItemImages") + "\\" + relativeImgPath);

        await _itemRepository.RemoveAsync(selectedItem);

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
