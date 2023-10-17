using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class OrderItemDetails
{
    public long ID { get; set; }
    
    public string Name { get; set; } = String.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}
