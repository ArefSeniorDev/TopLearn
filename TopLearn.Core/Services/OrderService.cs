using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services
{
    public class OrderService : IOrderService
    {
        TopLearnContext _context;
        IUserService _userService;
        public OrderService(TopLearnContext topLearnContext, IUserService userService)
        {
            _context = topLearnContext;
            _userService = userService;
        }

        public int AddOrder(string UserName, int CourseId)
        {
            int userId = _userService.GetUserIdByUserName(UserName);

            Order order = _context.Orders
                .FirstOrDefault(o => o.UserId == userId && !o.IsFinaly);

            var course = _context.Courses.Find(CourseId);

            if (order == null)
            {
                order = new Order()
                {
                    UserId = userId,
                    IsFinaly = false,
                    CreateDate = DateTime.Now,
                    OrderSum = course.CoursePrice,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId = CourseId,
                            Count = 1,
                            Price = course.CoursePrice
                        }
                    }
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            else
            {
                OrderDetail detail = _context.OrderDetails
                    .FirstOrDefault(d => d.OrderId == order.OrderId && d.CourseId == CourseId);
                if (detail != null)
                {
                    detail.Count += 1;
                    order.OrderSum = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).Sum(x => x.Price);
                    _context.OrderDetails.Update(detail);
                }
                else
                {
                    detail = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        Count = 1,
                        CourseId = CourseId,
                        Price = course.CoursePrice,
                    };
                    _context.OrderDetails.Add(detail);
                }

                _context.SaveChanges();
                UpdateOrderPrice(order.OrderId);
            }


            return order.OrderId;
        }

        public bool FinallyOrder(string UserName, int OrderId)
        {
            int UserId = _userService.GetUserIdByUserName(UserName);
            var Order = _context.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Course).FirstOrDefault(x => x.UserId == UserId && x.OrderId == OrderId);
            if (Order == null || Order.IsFinaly)
            {
                return false;
            }
            if (_userService.BalanceUserWallet(UserName) >= Order.OrderSum)
            {
                Order.IsFinaly = true;

                _userService.AddWallet(new DataLayer.Entities.Wallet.Wallet()
                {
                    Amount = Order.OrderDetails.Sum(x => x.Price * x.Count),
                    CreateDate = Order.CreateDate,
                    Description = "فاکتور شماره #" + Order.OrderId,
                    IsPay = true,
                    UserId = UserId,
                    TypeId = 1,
                    WalletTypeId = 1,
                    WalletType = "برداشت"

                });
                _context.Orders.Update(Order);

                foreach (var item in Order.OrderDetails)
                {
                    _context.Add(new UserCourse()
                    {
                        CourseId = item.CourseId,
                        UserId = UserId,
                    });
                }
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public Order GetOrderForUserPanel(string UserName, int OrderId)
        {
            int UserId = _userService.GetUserIdByUserName(UserName);

            return _context.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Course).SingleOrDefault(x => x.OrderId == OrderId && x.UserId == UserId);
        }

        public void UpdateOrderPrice(int OrderId)
        {
            var order = _context.Orders.Find(OrderId);
            order.OrderSum = _context.OrderDetails.Where(x => x.OrderId == OrderId).Sum(x => x.Price);
            _context.Update(order);
            _context.SaveChanges();
        }
    }
}
