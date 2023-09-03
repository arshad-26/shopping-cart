using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class OrderDetails
{
    public long OrderID { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    #nullable disable
    public IEnumerable<OrderItemDetails> Items { get; set; }
    #nullable restore
}
