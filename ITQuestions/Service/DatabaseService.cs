using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using Google.Apis.Auth.OAuth2;
using ITQuestions.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.Service
{
    public class DatabaseService
    {
        private readonly string databaseUrl = "https://itquestions-4f247-default-rtdb.europe-west1.firebasedatabase.app/";

        public async Task<List<ITQuestion>> GetQuestionsAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync($"{databaseUrl}/ITQuestions.json");
            return JsonConvert.DeserializeObject<List<ITQuestion>>(response)
                   ?? new List<ITQuestion>();
        }

        //public async Task AddQuestionAsync(string question, string answer)
        //{
        //    var newQuestion = new Question { QuestionText = question, QuestionAnswer = answer };
        //    using var client = new HttpClient();
        //    var json = JsonConvert.SerializeObject(newQuestion);
        //    await client.PostAsync($"{databaseUrl}/ITQuestions.json", new StringContent(json, Encoding.UTF8, "application/json"));
        //}
    }


    //public class QuestionModel
    //{
    //    public string Question { get; set; }
    //    public string Answer { get; set; }
    //}
}
