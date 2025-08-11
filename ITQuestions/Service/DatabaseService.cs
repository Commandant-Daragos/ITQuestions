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

        //public async Task<List<ITQuestion>> GetQuestionsAsync()
        //{
        //    using var client = new HttpClient();
        //    var response = await client.GetStringAsync($"{databaseUrl}/ITQuestions.json");

        //    var dict = JsonConvert.DeserializeObject<Dictionary<string, ITQuestion>>(response);

        //    return dict?.Values.ToList() ?? new List<ITQuestion>();
        //}

        public async Task<List<ITQuestion>> GetQuestionsAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync($"{databaseUrl}/ITQuestions.json");

            if (string.IsNullOrWhiteSpace(response) || response == "null")
                return new List<ITQuestion>();

            response = response.Trim();

            // Case: JSON array: [ null, {...}, {...} ]
            if (response.StartsWith("["))
            {
                var arr = JsonConvert.DeserializeObject<List<ITQuestion>>(response);
                return arr?.Where(q => q != null).ToList() ?? new List<ITQuestion>();
            }

            // Case: JSON object/dictionary: { "1": {...}, "2": {...}, "-OX...": {...} }
            if (response.StartsWith("{"))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, ITQuestion>>(response);
                if (dict == null) return new List<ITQuestion>();

                var list = dict
                    .Where(kv => kv.Value != null)
                    .OrderBy(kv =>
                    {
                        // Put numeric keys in numeric order, push non-numeric keys to the end
                        return int.TryParse(kv.Key, out var n) ? n : int.MaxValue;
                    })
                    .Select(kv => kv.Value)
                    .ToList();

                return list;
            }

            // fallback
            return new List<ITQuestion>();
        }

        public async Task AddQuestionAsync(string question, string answer)
        {
            var newQuestion = new ITQuestion { Question = question, Answer = answer };
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(newQuestion);
            await client.PostAsync($"{databaseUrl}/ITQuestions.json", new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}
