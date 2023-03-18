using System.Text.Json;

namespace ShoppingCartAPI.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }

    public string Message { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public ErrorDetails()
    {
        Message= String.Empty;
    }
}
