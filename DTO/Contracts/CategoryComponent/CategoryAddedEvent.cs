using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Contracts.CategoryComponent;

public class CategoryAddedEvent
{
    public bool ReloadTable { get; set; }
}
