using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.DTOs.QuestionVM;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Question;

namespace TopLearn.Core.Services
{
    public class ForumService : IForumService
    {
        private TopLearnContext _context;

        public ForumService(TopLearnContext context)
        {
            _context = context;
        }
        public int AddQuestion(Question question)
        {
            question.CreateDate = DateTime.Now;
            question.ModifiedDate = DateTime.Now;
            _context.Questions.Add(question);
            _context.SaveChanges();
            return question.QuestionId;
        }

        public IEnumerable<Question> GetQuestion(int? courseId, string filter = "")
        {
            IQueryable<Question> result = _context.Questions.Where(q => EF.Functions.Like(q.Title, $"%{filter}%"));
            if (courseId != null)
            {
                result = result.Where(x => x.CourseId == courseId);
            }
            return result.Include(x=>x.User).Include(x=>x.Course).ToList();
        }

        public ShowQuestionViewModel ShowQuestion(int questionId)
        {
            var question = new ShowQuestionViewModel();
            question.Question = _context.Questions.Include(x => x.User).SingleOrDefault(x => x.QuestionId == questionId)!;
            question.Answers = _context.Answers.Include(x => x.User).Where(x => x.QuestionId == questionId).ToList();

            return question;
        }

        public void AddAnswer(Answer answer)
        {
            _context.Answers.Add(answer);
            _context.SaveChanges();
        }

        public void UpdateIsTrueAnswer(int questionId, int answerId)
        {
            var answer = _context.Answers.Where(x => x.QuestionId == questionId);
            foreach (var item in answer)
            {
                item.IsTrueAnswer = false;
                if (item.AnswerId == answerId)
                {
                    item.IsTrueAnswer = true;
                }
            }
            _context.UpdateRange(answer);
            _context.SaveChanges();
        }
    }
}
