using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities;

public class Order
{
    public long Id { get; set; }

    public string UserId { get; set; } = String.Empty;

    public ApplicationUser User { get; set; } = new();

    public ICollection<OrderItem>? OrderItems { get; set;}
}
