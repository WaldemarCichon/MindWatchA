using Selftastic_WS_Test.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class Answer
    {
        public AnswerKind answerValue { get; set; }

        public AnswerKind answer => answerValue;
        public String answer1 => answerValue.ToString();
        public DateTime timestamp { get; set; }
        public string user { get; set; }
    }
}
