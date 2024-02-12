using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.Enum;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int AddOrder(string UserName, int CourseId);
        void UpdateOrderPrice(int OrderId);
        Order GetOrderForUserPanel(string UserName, int OrderId);
        void DeleteOrderDetail(int OrderId,int DetailOrderId);
        bool FinallyOrder(string UserName, int OrderId);
        List<Order> GetUserOrders(string UserName);
        DiscountUseType UseDiscount(int orderId, string code);
        Order GetByOrder(int orderId);
        void UpdateOrder(Order order);

        bool IsUserInCourse(string UserName, int CorseId);

        #region Discount
        int AddDiscount(Discount discount);
        Discount GetByDiscountId(int discountId);
        void UpdateDiscount(Discount discount);
        List<Discount> GetAllDiscounts();
        void DeleteDiscount(Discount Discount);
        bool IsExistCode(string code);

        #endregion

    }
}
