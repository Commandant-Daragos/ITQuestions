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
                // case: local record has no FirebaseKey yet
                if (string.IsNullOrEmpty(localQ.FirebaseKey))
                {
                    if (!localQ.IsDeleted)
                        await _remote.AddQuestionAsync(localQ); // will assign FirebaseKey + update local
                    continue; // skip further checks since it’s handled
                }

                if (!remoteDict.TryGetValue(localQ.FirebaseKey, out var remoteQ))
                {
                    // Local exists but missing in Firebase → push it
                    if (!localQ.IsDeleted)
                        await _remote.AddQuestionAsync(localQ);
                }
                else
                {
                    // Exists in both → resolve conflicts
                    if (localQ.LastModified > remoteQ.LastModified)
                    {
                        if (localQ.IsDeleted)
                        {
                            await _remote.DeleteQuestionAsync(localQ);

                            // also remove locally
                            using (var db = new DBContext())
                            {
                                db.ITQuestions.Remove(localQ);
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            await _remote.UpdateQuestionAsync(localQ);
                        }
                    }
                    else if (remoteQ.LastModified > localQ.LastModified)
                    {
                        if (remoteQ.IsDeleted)
                        {
                            using (var db = new DBContext())
                            {
                                db.ITQuestions.Remove(remoteQ);
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            await _local.UpdateQuestionAsync(remoteQ);
                        }
                    }
                }
            }

            // Handle remote -> local (new records from Firebase)
            foreach (var remoteQ in remoteData)
            {
                if (!localDict.ContainsKey(remoteQ.FirebaseKey))
                {
                    if (!remoteQ.IsDeleted)
                        await _local.AddQuestionAsync(remoteQ);
                }
            }
        }

        public async Task PushLocalToRemoteAsync()
        {
            var localData = await _local.GetQuestionsAsync();

            foreach (var localQ in localData)
            {
                try
                {
                    if (localQ.IsDeleted)
                    {
                        await _remote.DeleteQuestionAsync(localQ);
                    }
                    else
                    {
                        // For now always ADD to Firebase
                        await _remote.AddQuestionAsync(localQ);
                    }

                    Console.WriteLine($"Synced {localQ.FirebaseKey}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed on {localQ.FirebaseKey}: {ex.Message}");
                }
            }
        }
    }
}
