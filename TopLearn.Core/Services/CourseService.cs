using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services
{
    public class CourseService : ICourseService
    {
        private TopLearnContext _context;
        public CourseService(TopLearnContext topLearnContext)
        {
            _context = topLearnContext;
        }
        public List<CourseGroup> GetAllGroup()
        {
            return _context.CourseGroups.ToList();
        }

        public List<SelectListItem> GetGroupForManageCourse()
        {
            return _context.CourseGroups.Where(x => x.ParentId == null).Select(x => new SelectListItem()
            {
                Text = x.GroupTitle,
                Value = x.GroupId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetLevel()
        {
            return _context.CourseLevels.Select(x => new SelectListItem()
            {
                Text = x.LevelTitle,
                Value = x.LevelId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetStatus()
        {
            return _context.CourseStatuses.Select(x => new SelectListItem()
            {
                Text = x.StatusTitle,
                Value = x.StatusId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetSubGroupForManageCourse(int groupId)
        {
            return _context.CourseGroups.Where(x => x.ParentId == groupId).Select(x => new SelectListItem()
            {
                Text = x.GroupTitle,
                Value = x.GroupId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetTeacher()
        {
            return _context.UserRoles.Where(x => x.RoleId == 2).Include(x => x.User).Select(x => new SelectListItem()
            {
                Value = x.UserId.ToString(),
                Text = x.User.UserName
            }).ToList();
        }
    }
}
