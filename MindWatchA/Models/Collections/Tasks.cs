

using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Selftastic_WS_Test.Models.Collections
{
    public class Tasks: GenericModelCollection<Task>
    {
        public Tasks(): base() { }
        private Tasks(IEnumerable<Task> tasks) : base(tasks) { }

        private static Tasks instance;

        public static Tasks InstanceFromWebservice => new Tasks(ApiCall.Instance.GetTasks().Result);

        public static Tasks Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance<Tasks>();
                return instance;
            }
        }
    }
}
