using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.DTOs.CourseViewModel;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Group
        List<CourseGroup> GetAllGroup();
        List<SelectListItem> GetGroupForManageCourse();
        List<SelectListItem> GetSubGroupForManageCourse(int groupId);
        List<SelectListItem> GetTeacher();
        List<SelectListItem> GetStatus();
        List<SelectListItem> GetLevel();

        void AddGroupe(CourseGroup courseGroup);
        void UpdateGroupe(CourseGroup courseGroup);

        CourseGroup GetByCourseGroupId(int id);
        #endregion

        #region Course

        List<ShowCourseForAdminViewModel> GetCoursesForAdmin();

        int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Course GetCourseById(int Id);



        Tuple<List<ShowCourseListItemViewModel>, int> GetCourse(int pageId = 1, string filter = "", string getType = "all",
            string orderByType = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0);

        Course GetCourseForShow(int Id);

        List<ShowCourseListItemViewModel> GetPopularCourses();

        #endregion


        #region Episode

        int AddEpisode(CourseEpisode episode, IFormFile file);
        List<CourseEpisode> GetEpisodes(int courseId);
        CourseEpisode GetEpisodesById(int episodeId);
        void UpdateEpisode(CourseEpisode episode, IFormFile file);
        void DeleteEpisode(CourseEpisode episode);
        bool ChechExistFile(string FileName);

        #endregion

        #region Comments

        void AddComment(CourseComment courseComment);
        Tuple<List<CourseComment>, int> CourseComment(int courseId, int pageId = 1);

        #endregion
    }
}
