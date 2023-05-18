using AutoMapper;
using DAL.Context;
using DAL.Entities;
using DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        List<CategoryDTO> categories = _mapper.Map<List<CategoryDTO>>(categoriesDb);

        return Ok(categories);
    }
}
