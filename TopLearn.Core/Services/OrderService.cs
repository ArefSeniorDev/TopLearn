using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.Enum;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs.UserViewModel;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;
using TopLearn.DataLayer.Entities.User;

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

        public int AddDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            _context.SaveChanges();
            return discount.DiscountId;
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

        public void DeleteDiscount(Discount Discount)
        {
            _context.Discounts.Remove(Discount);
            _context.SaveChanges();
        }

        public void DeleteOrderDetail(int OrderId, int DetailOrderId)
        {
            var orderdetail = _context.OrderDetails.SingleOrDefault(x => x.DetailId == DetailOrderId && x.OrderId == OrderId);
            var order = _context.Orders.SingleOrDefault(x => x.OrderId == OrderId);
            if (orderdetail.Count > 1)
            {
                orderdetail.Count -= 1;
                order.OrderSum -= orderdetail.Price;
                _context.OrderDetails.Update(orderdetail);
            }
            else
            {
                _context.OrderDetails.Remove(orderdetail);
                _context.Orders.Remove(order);
            }
            _context.SaveChanges();
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
                    Amount = Order.OrderSum,
                    CreateDate = Order.CreateDate,
                    Description = "فاکتور شماره #" + Order.OrderId,
                    IsPay = true,
                    UserId = UserId,
                    TypeId = 1,
                    WalletTypeId = 1,
                    WalletType = "برداشت"

                }); ;
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

        public List<Discount> GetAllDiscounts()
        {
            return _context.Discounts.ToList();
        }

        public Discount GetByDiscountId(int discountId)
        {
            return _context.Discounts.SingleOrDefault(x => x.DiscountId == discountId);
        }

        public Order GetByOrder(int orderId)
        {
            return _context.Orders.SingleOrDefault(x => x.OrderId == orderId);
        }

        public Order GetOrderForUserPanel(string UserName, int OrderId)
        {
            int UserId = _userService.GetUserIdByUserName(UserName);
            return _context.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Course).FirstOrDefault(x => x.OrderId == OrderId && x.UserId == UserId);
        }

        public List<Order> GetUserOrders(string UserName)
        {
            int UserId = _userService.GetUserIdByUserName(UserName);
            return _context.Orders.Where(x => x.UserId == UserId).ToList();
        }

        public bool IsExistCode(string code)
        {
            return _context.Discounts.Any(x => x.DiscountCode == code);
        }

        public bool IsUserInCourse(string UserName, int CorseId)
        {
            int UserId = _userService.GetUserIdByUserName(UserName);
            return _context.UserCourses.Any(x => x.UserId == UserId && x.CourseId == CorseId);
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void UpdateOrderPrice(int OrderId)
        {
            var order = _context.Orders.Find(OrderId);
            order.OrderSum = _context.OrderDetails.Where(x => x.OrderId == OrderId).Sum(x => x.Price * x.Count);
            _context.Update(order);
            _context.SaveChanges();
        }

        public DiscountUseType UseDiscount(int orderId, string code)
        {
            var Discount = _context.Discounts.SingleOrDefault(x => x.DiscountCode == code);
            if (Discount == null)
                return DiscountUseType.NotFound;

            if (Discount.StartTime != null && Discount.StartTime < DateTime.Now)
                return DiscountUseType.ExpireDate;


            if (Discount.EndTime != null && Discount.EndTime >= DateTime.Now)
                return DiscountUseType.ExpireDate;

            if (Discount.UsableCount != null && Discount.UsableCount < 1)
                return DiscountUseType.Finished;



            var order = GetByOrder(orderId);

            if (_context.UserDiscountCodes.Include(x => x.User).Include(x => x.Discount).Any(x => x.UserId == order.UserId && x.DiscountId == Discount.DiscountId))
            {
                return DiscountUseType.Used;
            }

            if (order.UsedDisCount)
                return DiscountUseType.UsedTwice;
            int Percent = (order.OrderSum * Discount.DiscountPercent) / 100;
            order.OrderSum = order.OrderSum - Percent;
            order.UsedDisCount = true;
            UpdateOrder(order);

            if (Discount.UsableCount != null)
            {
                Discount.UsableCount -= 1;
            }
            _context.UserDiscountCodes.Add(new DataLayer.Entities.User.UserDiscountCode()
            {
                DiscountId = Discount.DiscountId,
                UserId = order.UserId
            });
            _context.Discounts.Update(Discount);

            _context.SaveChanges();
            return DiscountUseType.Success;
        }
    }
}
