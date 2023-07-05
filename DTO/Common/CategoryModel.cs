using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class CategoryModel
{
    public int CategoryID { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public string Name { get; set; } = String.Empty;

    public bool CanBeDeleted { get; set; }
}
