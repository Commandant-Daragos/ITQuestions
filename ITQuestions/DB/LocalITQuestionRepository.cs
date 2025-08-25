using ITQuestions.Enum;
using ITQuestions.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.DB
{
    public class LocalITQuestionRepository
    {
        private static readonly Lazy<LocalITQuestionRepository> _instance =
            new(() => new LocalITQuestionRepository()); 

        public static LocalITQuestionRepository Instance => _instance.Value;

        private LocalITQuestionRepository() { }

        public async Task<List<ITQuestion>> GetQuestionsAsync()
        {
            using var db = new DBContext();
            var questions =  await db.ITQuestions.ToListAsync();

            //foreach(var q in questions)
            //{
            //    q.SyncStatus = SyncStatus.None;
            //}

            return questions;
        }

        public async Task AddQuestionAsync(ITQuestion q, SyncStatus status)
        {
            using var db = new DBContext();
            if (string.IsNullOrEmpty(q.FirebaseKey))
                q.FirebaseKey = Guid.NewGuid().ToString();

            q.LastModified = DateTime.UtcNow;
            q.SyncStatus = status;
            db.ITQuestions.Add(q);
            await db.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(ITQuestion q, SyncStatus status)
        {
            using var db = new DBContext();
            q.LastModified = DateTime.UtcNow;
            q.SyncStatus = status;
            db.ITQuestions.Update(q);
            await db.SaveChangesAsync();
        }

        public async Task SoftDeleteQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            q.LastModified = DateTime.UtcNow;
            q.SyncStatus = SyncStatus.Delete;
            db.ITQuestions.Update(q);
            await db.SaveChangesAsync();
        }

        public async Task HardDeleteQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            var entity = await db.ITQuestions.FindAsync(q.FirebaseKey);
            if (entity != null)
            {
                db.ITQuestions.Remove(entity);
                await db.SaveChangesAsync();
            }
        }
    }
}
