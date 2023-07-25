using DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces;

public interface ICartService
{
    Task<ServiceResponse<IEnumerable<CategoryModel>>> GetItems();

    Task<ServiceResponse> PlaceOrder(string email, IEnumerable<CartModel> cartItems);
}
