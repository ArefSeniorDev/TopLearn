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
    }
}
