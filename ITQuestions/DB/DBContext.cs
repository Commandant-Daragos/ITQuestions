using ITQuestions.Model;
using Microsoft.EntityFrameworkCore;

namespace ITQuestions.DB
{
    public class DBContext : DbContext
    {
        public DbSet<ITQuestion> ITQuestions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=ITQuestions.db");

        public DBContext()
        {
            Database.EnsureCreated();
        }
    }
}
