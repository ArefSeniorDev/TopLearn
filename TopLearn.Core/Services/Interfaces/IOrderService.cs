using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int AddOrder(string UserName,int CourseId);
        void UpdateOrderPrice(int OrderId);
        Order GetOrderForUserPanel(string UserName, int OrderId);
        bool FinallyOrder(string UserName, int OrderId);

    }
}
