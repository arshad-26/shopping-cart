using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class ItemResponse
{
    public int TotalCount { get; set; }

    public List<TestItem> Items { get; set; } = new();
}
