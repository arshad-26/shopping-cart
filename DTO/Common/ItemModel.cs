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

    public string Category { get; set; } = String.Empty;

    public string Base64Img { get; set; } = String.Empty;
}
