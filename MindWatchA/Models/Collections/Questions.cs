using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Collections
{
    public class Questions: GenericModelCollection<Question>
    {
        public Questions(IEnumerable<Question> questions): base(questions)
        {

        }

        public static Questions instance;

        public static Questions InstanceFromWebservice = new Questions(ApiCall.Instance.GetQuestions().Result);

        public static Questions Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance<Questions>();
                instance.Persist();
                return instance;
            }
        }
    }
}
