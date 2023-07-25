using DTO.Common;
using DTO.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse> RegisterUser(RegisterModel registerModel);

    Task<ServiceResponse<TokenModel>> LoginUser(LoginModel loginModel);

    Task<ServiceResponse<TokenModel>> RefreshToken(TokenModel tokenModel);

    Task<ServiceResponse<bool?>> EmailExists(string email);
}
