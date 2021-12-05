using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA.Models.Single
{
    public class SavedTaskQuestion:GenericAPIModel
    {

        private Task task;
        private Question question;

        public SavedTaskQuestion(): base()
        {

        }

        public SavedTaskQuestion(Question question)
        {
            this.question = question;
        }

        public SavedTaskQuestion(Task task)
        {
            this.task = task;
        }

        [JsonIgnore]
        public bool IsTask
        {
            get => task != null;
        }

        [JsonIgnore]
        public bool IsQuestion
        {
            get => question != null;
        }

        public Task Task
        {
            get => task;
            set { task = value; }
        }

        public Question Question
        {
            get => question;
            set { question = value; } 
        }

        public override string Text { get => IsTask ? Task.Text : Question.Text; set => base.Text = value; }

        [JsonIgnore]
        public string Type
        {
            get => IsTask ? "Aufgabe" : "Frage";
        }

        public override string Id { get; set; } = "???";

        public override string ToString()
        { 
            if (IsTask)
            {
                return $"{task.Title} ( {Type} )." ;
            }

            if (IsQuestion)
            {
                return $"{question.Title} ( {Type} ).";
            }

            return "????";
        }
    }
}
