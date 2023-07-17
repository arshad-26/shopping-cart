using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities;

public class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ItemId { get; set; }

    public int Quantity { get; set; }

    public Order Order { get; set; } = new ();

    public Item Item { get; set; } = new();
}
