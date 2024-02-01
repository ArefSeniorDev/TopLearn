using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace TopLearn.Core.Security
{
    public static class ImageValidator
    {
        public static bool IsImage(this IFormFile file)
        {
            try
            {
                //Image validator
                //var img = Image.FromStream(file.OpenReadStream());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
