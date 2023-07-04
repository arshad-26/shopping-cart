using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Common;

public class ItemAPIModel
{
    public long ID { get; set; }
    
    [Required(ErrorMessage = "This field is required")]
    public string Name { get; set; } = String.Empty;

    [Required, Range(1, int.MaxValue, ErrorMessage = "Price can't be smaller than 1")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public int? CategoryID { get; set; }

    [Required(ErrorMessage = "Please upload a file")]
    public IFormFile? UploadedFile { get; set; }
}
