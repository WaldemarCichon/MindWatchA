using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Collections
{
    public class Affirmations: GenericModelCollection<Affirmation>
    {
        public Affirmations(IEnumerable<Affirmation> affirmations): base(affirmations) { }

        public static Affirmations instance;

        public static Affirmations InstanceFromWebservice = new Affirmations(ApiCall.Instance.GetAffirmations().Result);

        public static Affirmations Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance<Affirmations>();
                instance.Persist();
                return instance;
            }
        }
    }

}
