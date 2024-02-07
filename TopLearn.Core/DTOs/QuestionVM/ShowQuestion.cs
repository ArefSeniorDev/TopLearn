using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Question;

namespace TopLearn.Core.DTOs.QuestionVM
{
    public class ShowQuestionViewModel
    {
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
