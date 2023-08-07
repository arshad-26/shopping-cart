using AutoMapper;
using BusinessLayer.Interfaces;
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
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        ServiceResponse<IEnumerable<CategoryModel>> response = await _itemService.GetCategories();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        ServiceResponse<IEnumerable<ItemModel>> response = await _itemService.GetItems();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryModel categoryModel)
    {
        ServiceResponse response = await _itemService.AddCategory(categoryModel);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromForm] ItemAPIModel itemModel)
    {
        ServiceResponse response = await _itemService.AddItem(itemModel);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> EditItem(long ID)
    {
        ServiceResponse<ItemModel> response = await _itemService.EditItem(ID);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> EditItem([FromForm] ItemAPIModel itemModel)
    {
        ServiceResponse response = await _itemService.EditItem(itemModel);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int categoryID)
    {
        ServiceResponse response = await _itemService.DeleteCategory(categoryID);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteItem(long ID)
    {
        ServiceResponse response = await _itemService.DeleteItem(ID);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> CategoryExists(string category)
    {
        ServiceResponse<bool?> response = await _itemService.CategoryExists(category);
        return StatusCode((int)response.StatusCode, response);
    }
}
