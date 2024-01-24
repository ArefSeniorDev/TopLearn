using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Core.Convertors
{
    public class TextFixer
    {
        public static string TextFixed(string text)
        {
            return text.Trim().ToLower();
        }
    }
}
