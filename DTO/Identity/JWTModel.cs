using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Identity;

public class JWTModel
{
    public string ValidIssuer { get; set; } = String.Empty;

    public string ValidAudience { get; set; } = String.Empty;

    public string Secret { get; set; } = String.Empty;
}
