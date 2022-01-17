using System;
using Selftastic_WS_Test.Models.Collections;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA.Repository
{
    public class PersistableRepository
    {
        public User User { get; set; }
        public Affirmations Affirmations { get; set; }
        public Questions Questions => Questions.Instance;
        public Tasks Tasks => Tasks.Instance;


        public PersistableRepository()
        {

        }
    }
}
