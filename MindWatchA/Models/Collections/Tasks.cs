

using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Selftastic_WS_Test.Models.Collections
{
    public class Tasks: GenericModelCollection<Task>
    {
        private Tasks(IEnumerable<Task> tasks) : base(tasks) { }

        public static Tasks instance;

        public static Tasks InstanceFromWebservice = new Tasks(ApiCall.Instance.GetTasks().Result);

        public static Tasks Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance<Tasks>();
                instance.Persist();
                return instance;
            }
        }
    }
}
