using BusinessLayer.Interfaces;
using DAL.Entities;
using DTO.Common;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services;

public class OrderService : IOrderService
{
    private readonly IBaseEntityRepository<Order> _orderRepository;
    private readonly IBaseEntityRepository<OrderItem> _orderItemRepository;
    private readonly IBaseEntityRepository<Item> _itemRepository;
    private readonly IBaseEntityRepository<ApplicationUser> _userRepository;

    public OrderService(
        IBaseEntityRepository<Order> orderRepository,
        IBaseEntityRepository<OrderItem> orderItemRepository,
        IBaseEntityRepository<Item> itemRepository,
        IBaseEntityRepository<ApplicationUser> userRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _itemRepository = itemRepository;
        _userRepository = userRepository;

    }

    public async Task<ServiceResponse<IEnumerable<OrderDetails>>> GetOrdersForUser(string email)
    {
        ServiceResponse<IEnumerable<OrderDetails>> response = new();
        List<OrderDetails> result = new();
        
        var dbOrderDetails = await (from order in _orderRepository.GetAll()
                                    join orderItem in _orderItemRepository.GetAll() on order.Id equals orderItem.OrderId
                                    join item in _itemRepository.GetAll() on orderItem.ItemId equals item.ID
                                    join user in _userRepository.GetAll() on order.UserId equals user.Id
                                    where user.Email == email
                                    select new
                                    {
                                        order.Id,
                                        order.OrderDate,
                                        order.TotalPrice,
                                        item.ID,
                                        item.Name,
                                        item.Price,
                                        orderItem.Quantity
                                    }).ToListAsync();

        var groupedOrderDetails = dbOrderDetails.GroupBy(x => new { x.Id, x.OrderDate, x.TotalPrice });

        foreach(var orderDetail in groupedOrderDetails)
        {
            OrderDetails userOrderDetails = new();

            userOrderDetails.OrderID = orderDetail.Key.Id;
            userOrderDetails.OrderDate = orderDetail.Key.OrderDate;
            userOrderDetails.TotalPrice = orderDetail.Key.TotalPrice;

            List<OrderItemDetails> orderItems = new();

            foreach(var orderItemDetail in orderDetail)
            {
                OrderItemDetails userOrderItemDetails = new()
                {
                    ID = orderItemDetail.ID,
                    Name = orderItemDetail.Name,
                    Quantity = orderItemDetail.Quantity,
                    Price = orderItemDetail.Price
                };

                orderItems.Add(userOrderItemDetails);
            }

            userOrderDetails.Items = orderItems;
            result.Add(userOrderDetails);
        }

        response.ResponseData = result;
        return response;
    }
}
