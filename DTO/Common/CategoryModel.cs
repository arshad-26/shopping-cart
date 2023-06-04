using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class CategoryModel
{
    [Required(ErrorMessage = "This field is required")]
    public string Category { get; set; } = default!;
}
