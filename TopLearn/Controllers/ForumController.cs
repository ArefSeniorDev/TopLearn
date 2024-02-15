using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Question;

namespace TopLearn.Controllers
{
    public class ForumController : Controller
    {
        IForumService _forumService;
        IUserService _userService;
        IOrderService _orderService;

        public ForumController(IForumService forumService, IUserService userService, IOrderService orderService)
        {
            _forumService = forumService;
            _userService = userService;
            _orderService = orderService;
        }
        [Authorize]
        public IActionResult Index(int? courseId, string filter = "")
        {
            if (courseId != null)
            {
                if (!_orderService.IsUserInCourse(User.Identity.Name, Convert.ToInt32(courseId)))
                {
                    return Redirect("/ShowCourse/" + courseId);
                }
            }
            ViewBag.CourseId = courseId;
            return View(_forumService.GetQuestion(courseId, filter));
        }

        #region CreateQuestion
        [Authorize]
        public IActionResult CreateQuestion(int Id)
        {
            Question question = new Question()
            {
                CourseId = Id
            };
            return View(question);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateQuestion(Question question)
        {
            if (!ModelState.IsValid && question == null)
            {
                return View(question);
            }

            question.UserId = _userService.GetUserIdByUserName(User.Identity.Name);
            int questionId = _forumService.AddQuestion(question);
            return RedirectToAction("ShowQuestion", new { questionId = questionId });
        }
        #endregion

        #region ShowQuestion
        [Authorize]
        public IActionResult ShowQuestion(int questionId)
        {
            var showquestion = _forumService.ShowQuestion(questionId);
            return View(showquestion);
        }

        #endregion

        #region Answer
        [Authorize]

        public IActionResult Answer(int Id, string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                var sanitizer = new HtmlSanitizer();
                body = sanitizer.Sanitize(body);
                int UserId = _userService.GetUserIdByUserName(User.Identity.Name);
                DataLayer.Entities.Question.Answer answer = new Answer()
                {
                    BodyAnswer = body,
                    CreateDate = DateTime.Now,
                    UserId = UserId,
                    QuestionId = Id,
                    AnswerdTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)

                };
                _forumService.AddAnswer(answer);
            }
            return RedirectToAction("ShowQuestion", new { questionId = Id });
        }
        [Authorize]
        public IActionResult UpdateQuestion(int questionId, int answerId)
        {
            int UserId = _userService.GetUserIdByUserName(User.Identity.Name);
            var question = _forumService.ShowQuestion(questionId);
            if (question.Question.UserId == UserId)
            {
                _forumService.UpdateIsTrueAnswer(questionId, answerId);
            }

            return RedirectToAction("ShowQuestion", new { questionId = questionId });
        }
        #endregion

    }
}
