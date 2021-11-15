using System;
using System.Runtime.InteropServices;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA.Models.Single
{
    public class SavedTaskQuestion:GenericAPIModel
    {

        private Task task;
        private Question question;

        public SavedTaskQuestion(Question question)
        {
            this.question = question;
        }

        public SavedTaskQuestion(Task task)
        {
            this.task = task;
        }

        public bool IsTask
        {
            get => task != null;
        }

        public bool IsQuestion
        {
            get => question != null;
        }

        public Task Task
        {
            get => task;
            set => Task = value; 
        }

        public Question Question
        {
            get => question;
            set => question = value;
        }

        public new string Text
        {
            get => ToString();
        }

        public override string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string ToString()
        { 
            if (IsTask)
            {
                return task.Text;
            }

            if (IsQuestion)
            {
                return question.Text;
            }

            return null;
        }
    }
}
