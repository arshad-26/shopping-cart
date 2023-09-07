using DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse<IEnumerable<OrderDetails>>> GetOrdersForUser(string email);
}
