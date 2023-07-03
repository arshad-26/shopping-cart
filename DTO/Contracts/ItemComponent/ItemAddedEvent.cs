using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Contracts.ItemComponent;

public class ItemAddedEvent
{
    public bool ReloadTable { get; set; }
}
