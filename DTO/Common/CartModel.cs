using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class CartModel
{
    public long ItemID { get; set; }

    public string ItemName { get; set; } = String.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice => Quantity * Price;
}
