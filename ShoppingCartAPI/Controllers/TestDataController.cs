using DTO.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestDataController : ControllerBase
{
    private readonly List<TestItem> _allItems;

    public TestDataController()
    {
        _allItems = GenerateRandomItems(1000); // Initial set of items
    }

    [HttpGet]
    [Route("GetTestData")]
    public ActionResult<ItemResponse> GetTestData(int startIndex, int pageSize)
    {
        List<TestItem> items = _allItems.Skip(startIndex).Take(pageSize).ToList();
        return new ItemResponse() { TotalCount = 1000, Items = items };
    }

    private List<TestItem> GenerateRandomItems(int count)
    {
        var items = new List<TestItem>();
        var rand = new Random();

        for (var i = 1; i <= count; i++)
        {
            items.Add(new TestItem { Id = i, Name = $"Item {i}", Value = rand.Next(1, 100) });
        }

        return items;
    }
}
