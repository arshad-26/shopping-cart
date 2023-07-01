using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class ItemModel
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;

    public decimal Price { get; set; }

    public int? CategoryID { get; set; }

    public string Category { get; set; } = String.Empty;

    public IBrowserFile? File { get; set; }

    public IFormFile? UploadedFile { get; set; }

    public string Base64Img { get; set; } = String.Empty;
}
