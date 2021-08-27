using Selftastic_WS_Test.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class MoodAnswer
    {
        public MoodAnswerKind answerValue { get; set; }
        public String answer => answerValue.ToString();
        public DateTime timestamp { get; set; }
        public string user { get; set; }
    }
}
