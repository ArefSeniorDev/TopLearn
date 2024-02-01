using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Core.DTOs.CourseViewModel
{
    public class ShowCourseListItemViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        public string CourseImageName { get; set; }
        public int Price { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}
