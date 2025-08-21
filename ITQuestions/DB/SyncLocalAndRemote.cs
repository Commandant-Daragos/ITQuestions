using ITQuestions.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.DB
{
    public class SyncLocalAndRemote
    {
        private static readonly Lazy<SyncLocalAndRemote> _instance = new(() => new SyncLocalAndRemote());
        public static SyncLocalAndRemote Instance => _instance.Value;

        private readonly LocalITQuestionRepository _local = LocalITQuestionRepository.Instance;
        private readonly DatabaseService _remote = DatabaseService.Instance;

        private SyncLocalAndRemote() { }

        public async Task SyncAsync()
        {
            var localData = await _local.GetQuestionsAsync();
            var remoteData = await _remote.GetQuestionsAsync();

            var remoteDict = remoteData.ToDictionary(q => q.FirebaseKey);
            var localDict = localData.ToDictionary(q => q.FirebaseKey);

            // Handle local -> remote
            foreach (var localQ in localData)
            {
                if (!remoteDict.TryGetValue(localQ.FirebaseKey, out var remoteQ))
                {
                    // Only local → push to Firebase
                    await _remote.AddQuestionAsync(localQ.Question, localQ.Answer);
                }
                else
                {
                    // Exists in both → compare LastModified
                    if (localQ.LastModified > remoteQ.LastModified)
                    {
                        await _remote.UpdateQuestionAsync(localQ);
                    }
                    else if (remoteQ.LastModified > localQ.LastModified)
                    {
                        await _local.UpdateQuestionAsync(remoteQ);
                    }
                }
            }

            //Handle remote -> local(new records in Firebase)
            foreach (var remoteQ in remoteData)
            {
                if (!localDict.ContainsKey(remoteQ.FirebaseKey))
                {
                    await _local.AddQuestionAsync(remoteQ);
                }
            }
        }

        //public async Task SyncAsync()
        //{
        //    // 1. Pull remote data
        //    var remote = await _remote.GetQuestionsAsync();
        //    var local = await _local.GetQuestionsAsync();

        //    // Compare & merge logic here
        //    // (new items, updates, deletes)
        //    // -> Update SQLite first
        //    // -> Then push changes back to Firebase
        //}
    }
}
