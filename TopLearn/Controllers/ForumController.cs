using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Question;

namespace TopLearn.Controllers
{
    public class ForumController : Controller
    {
        IForumService _forumService;
        IUserService _userService;

        public ForumController(IForumService forumService, IUserService userService)
        {
            _forumService = forumService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
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
            return Redirect("/Forum/ShowQuestion" + questionId);
        }
        #endregion

        #region ShowQuestion

        public IActionResult ShowQuestion(int Id)
        {
            return View(_forumService.ShowQuestion(Id));
        }

        #endregion

        #region Answer

        public IActionResult Answer(int Id, string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                int UserId = _userService.GetUserIdByUserName(User.Identity.Name);
                DataLayer.Entities.Question.Answer answer = new Answer()
                {
                    BodyAnswer = body,
                    CreateDate = DateTime.Now,
                    UserId = UserId,
                    QuestionId = Id
                };
                _forumService.AddAnswer(answer);
            }
            return RedirectToAction("ShowQuestion", new { id = Id });
        }
        #endregion

    }
}
