using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Core.Genrator
{
    public class GetUserActiveCode
    {
        public static string GetActiveCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
