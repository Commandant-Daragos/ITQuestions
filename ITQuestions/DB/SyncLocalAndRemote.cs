using ITQuestions.Enum;
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

            var localDict = localData.ToDictionary(q => q.FirebaseKey);
            var remoteDict = remoteData.ToDictionary(q => q.FirebaseKey);

            // --- Phase 1: Local → Remote ---
            foreach (var localQ in localData)
            {
                if (!remoteDict.TryGetValue(localQ.FirebaseKey, out var remoteQ))
                {
                    // Local exists but missing in remote
                    switch (localQ.SyncStatus)
                    {
                        case SyncStatus.Add:
                            await _remote.AddQuestionAsync(localQ);
                            break;

                        case SyncStatus.Update:
                            // Remote doesn’t have it anymore → re-create
                            await _remote.AddQuestionAsync(localQ);
                            break;
                        case SyncStatus.Delete:
                            await _local.HardDeleteQuestionAsync(localQ);
                            localDict.Remove(localQ.FirebaseKey);
                            continue;
                    }
                }
                else
                {
                    // Exists in both → resolve conflicts by LastModified
                    if (localQ.LastModified > remoteQ.LastModified)
                    {
                        switch (localQ.SyncStatus)
                        {
                            case SyncStatus.Update:
                                await _remote.UpdateQuestionAsync(localQ);
                                break;

                            case SyncStatus.Delete:
                                try
                                {
                                    await _remote.DeleteQuestionAsync(localQ);
                                }
                                catch (Exception ex)
                                {
                                    // Optionally log it, but don’t fail sync if it’s "not found"
                                    Console.WriteLine($"Remote delete skipped for {localQ.FirebaseKey}: {ex.Message}");
                                }
                                await _local.HardDeleteQuestionAsync(localQ);
                                continue;
                        }

                        await _local.UpdateQuestionAsync(localQ, SyncStatus.None);
                    }
                    else if (remoteQ.LastModified > localQ.LastModified)
                    {
                        await _local.UpdateQuestionAsync(remoteQ, SyncStatus.None);
                    }
                }
            }

            // --- Phase 2: Remote → Local ---
            foreach (var remoteQ in remoteData)
            {
                if (!localDict.ContainsKey(remoteQ.FirebaseKey))
                {
                    if (remoteQ.SyncStatus != SyncStatus.Delete)
                    {
                        await _local.AddQuestionAsync(remoteQ, SyncStatus.None);
                    }
                }
            }
        }
    }
}
