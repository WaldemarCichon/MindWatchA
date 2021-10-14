using MindWatchA.Models.Single;
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
        private const String AdminName = "admin";
        private const String AdminPassword = "f9d77e3d3549484485985548758a4a0c";
        private const String UserName = "user";
        private const String Password = "e1132bc38c29465dac7a285a57727555";
        public const String UserId = "a92585ae-8666-450a-89e9-2ab9edc15ee6";
        public const String UserEmail = "w@a";
        public const String UserPassword = "b";
        public const String UserId1 = "21774f13-be68-4cda-a33c-b2935216681c";
        private const string ApiUrl = "https://api.selftastic.de";


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
            Console.WriteLine(Url + "/affirmation");
            var answer = await httpClient.GetFromJsonAsync<IEnumerable<Affirmation>>("/affirmation").ConfigureAwait(false);
            return answer;
        }

        public async Task<IEnumerable<Question>> GetQuestions()
        {
            var answer = await httpClient.GetFromJsonAsync<IEnumerable<Question>>("/question").ConfigureAwait(false);
            return answer;
        }

        public async Task<IEnumerable<Models.Single.Task>> GetTasks()
        {
            var tasks = await httpClient.GetFromJsonAsync<IEnumerable<Models.Single.Task>>("/task").ConfigureAwait(false);
            return tasks;
        }


        internal async Task SendAnswer<T>(T position, MoodAnswerKind answerKind) where T : GenericAPIModel
        {
            var answer = new MoodAnswer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = User.Instance.user_id
            };
            var typeName = typeof(T).Name.ToLower();
            var answerResult = await httpClient.PostAsJsonAsync("/" + typeName + "/" + position.Id + "/answer", answer).ConfigureAwait(false);
            Console.WriteLine("Sent: " + typeName + " - " + answerKind);
            answerResult.EnsureSuccessStatusCode();
            Console.WriteLine("Sending successfull");
        }

        internal async Task SendAnswer<T>(T position, AnswerKind answerKind) where T : GenericAPIModel
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = User.Instance.user_id
            };
            var typeName = typeof(T).Name.ToLower();
            var answerResult = await httpClient.PostAsJsonAsync("/" + typeName + "/" + position.Id + "/answer", answer).ConfigureAwait(false);
            Console.WriteLine("Sent: " + typeName + " -- " + answerKind);
            answerResult.EnsureSuccessStatusCode();
            Console.WriteLine("Sending - successfull");
        }

        public async Task PostAffirmationAnswer(string id, AnswerKind answerKind)
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = User.Instance.user_id
            };
            var answerResult = await httpClient.PostAsJsonAsync("/affirmation/" + id + "/answer", answer).ConfigureAwait(false);
            answerResult.EnsureSuccessStatusCode();
        }

        public async Task PostTaskAnswer(string id, AnswerKind answerKind)
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = User.Instance.user_id
            };
            var answerResult = await httpClient.PostAsJsonAsync("/task/"+id+"/answer", answer).ConfigureAwait(false);
            answerResult.EnsureSuccessStatusCode();
        }

        public async Task PostQuestionAnswer(string id, AnswerKind answerKind)
        {
            var answer = new Answer()
            {
                answerValue = answerKind,
                timestamp = DateTime.Now,
                user = User.Instance.user_id
            };
            var answerResult = await httpClient.PostAsJsonAsync("/question/" + id + "/answer", answer).ConfigureAwait(false);
            answerResult.EnsureSuccessStatusCode();
        }

        public async Task<String> Login(String email, String password)
        {
            var loginData = new LoginData(email, password);
            var answerResult = await httpClient.PostAsJsonAsync("/user/login", loginData).ConfigureAwait(false);
            if (answerResult.StatusCode == System.Net.HttpStatusCode.InternalServerError || answerResult.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "";
            }
            answerResult.EnsureSuccessStatusCode();
            var result = await answerResult.Content.ReadAsStringAsync();
            return result;
        }

        public async Task TestLogin()
        {
            await Login(UserEmail, UserPassword);
        }
    }
}
