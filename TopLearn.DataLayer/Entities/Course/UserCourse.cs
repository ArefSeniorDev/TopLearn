using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TopLearn.DataLayer.Entities.Course
{
    public class UserCourse
    {
        [Key]
        public int UC_Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int CourseId { get; set; }



        #region Relations

        public virtual User.User User { get; set; }

        public virtual Course Course { get; set; }

        #endregion
    }
}
