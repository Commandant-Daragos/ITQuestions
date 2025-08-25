using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using Google.Apis.Auth.OAuth2;
using ITQuestions.DB;
using ITQuestions.Enum;
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
    public sealed class DatabaseService
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly string databaseUrl = "https://itquestions-4f247-default-rtdb.europe-west1.firebasedatabase.app/";

        private static readonly Lazy<DatabaseService> _instance = new(() => new DatabaseService());
        public static DatabaseService Instance => _instance.Value;

        private DatabaseService() { }

        public async Task<List<ITQuestion>> GetQuestionsAsync()
        {
            var response = await _client.GetStringAsync($"{databaseUrl}/ITQuestions.json").ConfigureAwait(false); ;

            if (string.IsNullOrWhiteSpace(response) || response == "null")
                return new List<ITQuestion>();

            var dict = JsonConvert.DeserializeObject<Dictionary<string, ITQuestion>>(response);
            if (dict == null) return new List<ITQuestion>();

            foreach (var kv in dict)
            {
                kv.Value.FirebaseKey = kv.Key; // ensure key matches
                kv.Value.SyncStatus = SyncStatus.None;
            }

            return dict.Values.ToList();
        }

        public async Task AddQuestionAsync(ITQuestion question)
        {
            question.LastModified = DateTime.UtcNow;

            var json = JsonConvert.SerializeObject(question);

            // use FirebaseKey as the dictionary key
            var url = $"{databaseUrl}/ITQuestions/{question.FirebaseKey}.json";
            var response = await _client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateQuestionAsync(ITQuestion question)
        {
            if (string.IsNullOrEmpty(question.FirebaseKey))
                throw new InvalidOperationException("Cannot update without FirebaseKey");

            question.LastModified = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(question);

            var url = $"{databaseUrl}/ITQuestions/{question.FirebaseKey}.json";
            var response = await _client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteQuestionAsync(ITQuestion question)
        {
            if (string.IsNullOrEmpty(question.FirebaseKey))
                throw new InvalidOperationException("Cannot delete without FirebaseKey");

            var url = $"{databaseUrl}/ITQuestions/{question.FirebaseKey}.json";
            var response = await _client.DeleteAsync(url).ConfigureAwait(false); ;
            response.EnsureSuccessStatusCode();
        }
    }
}
