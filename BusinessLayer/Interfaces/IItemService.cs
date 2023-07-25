using DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces;

public interface IItemService
{
    Task<ServiceResponse<IEnumerable<CategoryModel>>> GetCategories();

    Task<ServiceResponse> AddCategory(CategoryModel categoryModel);

    Task<ServiceResponse> DeleteCategory(int categoryID);

    Task<ServiceResponse<bool?>> CategoryExists(string category);

    Task<ServiceResponse<IEnumerable<ItemModel>>> GetItems();

    Task<ServiceResponse> AddItem(ItemAPIModel itemModel);

    Task<ServiceResponse<ItemModel>> EditItem(long ID);

    Task<ServiceResponse> EditItem(ItemAPIModel itemModel);

    Task<ServiceResponse> DeleteItem(long ID);
}
