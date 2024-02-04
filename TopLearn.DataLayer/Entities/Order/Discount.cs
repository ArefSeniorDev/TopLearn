using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.DataLayer.Entities.Order
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }


        [Display(Name = "کد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150)]
        public string DiscountCode { get; set; }
        [Display(Name = "درصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int DiscountPercent { get; set; }
        public int? UsableCount { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        #region Rel
        public virtual List<UserDiscountCode> UserDiscountCodes { get; set; }

        #endregion
    }
}
