using Newtonsoft.Json;
using Selftastic_WS_Test.Enums;
using Selftastic_WS_Test.Models;
using Selftastic_WS_Test.Models.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Selftastic_WS_Test.API
{
    public class ApiCall
    {
        private const String UserName = "user";
        private const String Password = "e1132bc38c29465dac7a285a57727555";
        public const String UserId1 = "40bbc293-6b9b-4673-91b2-9a324794c7a3";
        public const String UserId2 = "a93c96ae-5394-4a14-b803-1320e4bf1078";
        private const string ApiUrl = "https://api.selftastic.de/";


        private static ApiCall instance;

        private HttpClient httpClient;

        public ApiCall(string url, Authentification authentification)
        {
            this.Url = url;
            this.Authentification = authentification;
            Init();
        }

        public string Url { get; }
        public Authentification Authentification { get; }
        private void Init()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Url);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            //var message = new HttpRequestMessage(HttpMethod.Get,"/user");
            //message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Authentification.Base64String);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Authentification.Base64String);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static ApiCall Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiCall(ApiUrl, new Authentification(UserName, Password));
                }
                return instance;
            }
        }


        public async Task<IEnumerable<Affirmation>> GetAffirmations() { 
            Console.WriteLine(Url + "//affirmation");
            var answer = await httpClient.GetAsync("/affirmation");
            var content = answer.Content;
            var affirmations = await content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<Affirmation>>(affirmations);
            return deserialized;
        }

        public async Task<IEnumerable<Question>> GetQuestions()
        {
            var answer = await httpClient.GetAsync("/question");
            var content = answer.Content;
            var questions = await content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<Question>>(questions);
            return deserialized;
        }

        public async Task<IEnumerable<Models.Single.Task>> GetTasks()
        {
            var tasks = await httpClient.GetFromJsonAsync<IEnumerable<Models.Single.Task>>("/task");
            return tasks;
        }

        public async Task PostTaskAnswer(string id, AnswerKind answerKind)
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = UserId1
            };
            var answerResult = await httpClient.PostAsJsonAsync("/task/"+id+"/answer", answer);
            answerResult.EnsureSuccessStatusCode();
        }

        public async Task PostQuestionAnswer(string id, AnswerKind answerKind)
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = UserId1
            };
            var answerResult = await httpClient.PostAsJsonAsync("/question/" + id + "/answer", answer);
            answerResult.EnsureSuccessStatusCode();
        }
    }
}
