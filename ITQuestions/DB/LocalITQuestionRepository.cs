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
            return await db.ITQuestions.ToListAsync();
        }

        public async Task AddQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            if (string.IsNullOrEmpty(q.FirebaseKey))
                q.FirebaseKey = Guid.NewGuid().ToString();

            q.LastModified = DateTime.UtcNow;
            db.ITQuestions.Add(q);
            await db.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            q.LastModified = DateTime.UtcNow;
            db.ITQuestions.Update(q);
            await db.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            q.IsDeleted = true;
            q.LastModified = DateTime.UtcNow;
            db.ITQuestions.Update(q);
            await db.SaveChangesAsync();
        }
    }
}
