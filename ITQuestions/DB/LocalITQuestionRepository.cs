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

            db.ITQuestions.Add(q);
            await db.SaveChangesAsync();
        }
        //public async Task AddQuestionAsync(string question, string answer, DateTime dt)
        //{
        //    using var db = new DBContext();
        //    db.ITQuestions.Add(new ITQuestion { FirebaseKey = Guid.NewGuid().ToString() ,Question = question, Answer = answer, LastModified = dt });
        //    await db.SaveChangesAsync();
        //}

        public async Task UpdateQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            db.ITQuestions.Attach(q);
            db.Entry(q).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(ITQuestion q)
        {
            using var db = new DBContext();
            db.ITQuestions.Attach(q);
            db.ITQuestions.Remove(q);
            await db.SaveChangesAsync();
        }
    }
}
