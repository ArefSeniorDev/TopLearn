using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.DTOs.CourseViewModel;
using TopLearn.Core.Genrator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public class CourseService : ICourseService
    {
        private TopLearnContext _context;
        public CourseService(TopLearnContext topLearnContext)
        {
            _context = topLearnContext;
        }


        public int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";
            course.DemoFileName = "no-photo.jpg";

            if (imgCourse != null)//&& imgCourse.IsImage()) For Security That Is An Image Or not
            {
                course.CourseImageName = GetUserActiveCode.GetActiveCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }
            }

            // Resize To thumbnail

            //ImageConvertor imgResizer = new ImageConvertor();
            //string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);

            //imgResizer.Image_resize(imagePath, thumbPath, 150);

            if (courseDemo != null)
            {
                course.CourseImageName = GetUserActiveCode.GetActiveCode() + Path.GetExtension(courseDemo.FileName);
                string demopath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/demos", course.DemoFileName);

                using (var stream = new FileStream(demopath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return course.CourseId;
        }

        public int AddEpisode(CourseEpisode episode, IFormFile file)
        {
            episode.EpisodeFileName = file.FileName;

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", episode.EpisodeFileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _context.CourseEpisodes.Add(episode);
            _context.SaveChanges();
            return episode.EpisodeId;
        }

        public bool ChechExistFile(string FileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", FileName);
            return File.Exists(path);
        }

        public void DeleteEpisode(CourseEpisode episode)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", episode.EpisodeFileName);
            File.Delete(path);

            _context.CourseEpisodes.Remove(episode);
            _context.SaveChanges();
        }

        public List<CourseGroup> GetAllGroup()
        {
            return _context.CourseGroups.ToList();
        }

        public List<ShowCourseListItemViewModel> GetCourse(int pageId = 1, string filter = ""
           , string getType = "all", string orderByType = "date",
           int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            if (take == 0)
                take = 8;

            IQueryable<Course> result = _context.Courses;

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(c => c.CourseTitle.Contains(filter));
            }

            switch (getType)
            {
                case "all":
                    break;
                case "buy":
                    {
                        result = result.Where(c => c.CoursePrice != 0);
                        break;
                    }
                case "free":
                    {
                        result = result.Where(c => c.CoursePrice == 0);
                        break;
                    }

            }

            switch (orderByType)
            {
                case "date":
                    {
                        result = result.OrderByDescending(c => c.CreateDate);
                        break;
                    }
                case "updatedate":
                    {
                        result = result.OrderByDescending(c => c.UpdateDate);
                        break;
                    }
            }

            if (startPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > startPrice);
            }

            if (endPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < startPrice);
            }


            if (selectedGroups != null && selectedGroups.Any())
            {
                //TODo
            }

            int skip = (pageId - 1) * take;
            ;
            return result.Include(c => c.CourseEpisodes).AsEnumerable().Select(c => new ShowCourseListItemViewModel()
            {
                CourseId = c.CourseId,
                CourseImageName = c.CourseImageName,
                Price = c.CoursePrice,
                Title = c.CourseTitle,
                TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            }).Skip(skip).Take(take).ToList();


        }

        public Course GetCourseById(int Id)
        {
            return _context.Courses.SingleOrDefault(x => x.CourseId == Id);
        }

        public List<ShowCourseForAdminViewModel> GetCoursesForAdmin()
        {
            return _context.Courses.Select(x => new ShowCourseForAdminViewModel()
            {
                CourseId = x.CourseId,
                EpisodeCount = x.CourseEpisodes.Count,
                ImageName = x.CourseImageName,
                Title = x.CourseTitle,
            }).ToList();
        }

        public List<CourseEpisode> GetEpisodes(int courseId)
        {
            return _context.CourseEpisodes.Where(x => x.CourseId == courseId).ToList();
        }

        public CourseEpisode GetEpisodesById(int episodeId)
        {
            return _context.CourseEpisodes.Where(x => x.EpisodeId == episodeId).SingleOrDefault();
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

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.UpdateDate = DateTime.Now;

            if (imgCourse != null)//&& imgCourse.IsImage()) For Security That Is An Image Or not
            {
                if (course.CourseImageName != "no-photo.jpg")
                {
                    string deleteimagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/image", course.CourseImageName);
                    if (File.Exists(deleteimagePath))
                    {
                        File.Delete(deleteimagePath);
                    }

                    string deletethumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/thumb", course.CourseImageName);
                    if (File.Exists(deletethumbPath))
                    {
                        File.Delete(deletethumbPath);
                    }
                }
                course.CourseImageName = GetUserActiveCode.GetActiveCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }
            }

            // Resize To thumbnail

            //ImageConvertor imgResizer = new ImageConvertor();
            //string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);

            //imgResizer.Image_resize(imagePath, thumbPath, 150);

            if (courseDemo != null)
            {
                if (course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/demoes", course.DemoFileName);
                    if (File.Exists(deleteDemoPath))
                    {
                        File.Delete(deleteDemoPath);
                    }
                }
                course.CourseImageName = GetUserActiveCode.GetActiveCode() + Path.GetExtension(courseDemo.FileName);
                string demopath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/course/demos", course.DemoFileName);

                using (var stream = new FileStream(demopath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void UpdateEpisode(CourseEpisode episode, IFormFile file)
        {
            if (file != null)
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", episode.EpisodeFileName);
                File.Delete(deletePath);

                episode.EpisodeFileName = file.FileName;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course_ep", episode.EpisodeFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            _context.CourseEpisodes.Update(episode);
            _context.SaveChanges();
        }
    }
}
