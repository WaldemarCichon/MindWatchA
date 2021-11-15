using System;
using System.Collections.Generic;
using MindWatchA.Models.Single;
using Selftastic_WS_Test.Models.Collections;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA.Models.Collections
{
    public class SavedTaskQuestions: GenericModelCollection<SavedTaskQuestion>
    {
            public SavedTaskQuestions() : base() { }
            private SavedTaskQuestions(IEnumerable<SavedTaskQuestion> savedTaskQuestions) : base(savedTaskQuestions) { }

            private static SavedTaskQuestions instance;

            public static SavedTaskQuestions Instance
            {
                get
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    instance = CreateInstance<SavedTaskQuestions>();
                    return instance;
                }
            }

        internal void Add(Question question)
        {
            Add(new SavedTaskQuestion(question));
        }

        internal void Add(Task task)
        {
            Add(new SavedTaskQuestion(task));
        }

         
        private void Add(SavedTaskQuestion savedTaskQuestion)
        {
            
            throw new NotImplementedException();
        }
    }
    }
