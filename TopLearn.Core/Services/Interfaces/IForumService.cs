using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.QuestionVM;
using TopLearn.DataLayer.Entities.Question;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IForumService
    {
        #region Question

        public int AddQuestion(Question question);
        public IEnumerable<Question> GetQuestion(int? courseId, string filter = "");

        #endregion

        #region Answare
        ShowQuestionViewModel ShowQuestion(int questionId);
        void AddAnswer(Answer answer);
        void UpdateIsTrueAnswer(int questionId, int answerId);

        #endregion
    }
}
