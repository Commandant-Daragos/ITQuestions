using ITQuestions.Exceptions;
using ITQuestions.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.DB
{
    public class DBContext : DbContext
    {
        public DbSet<ITQuestion> ITQuestions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=ITQuestions.db");

        public DBContext()
        {
            // Quick way: if DB or tables don’t exist yet, they’ll be created
            Database.EnsureCreated();
        }
    }
}
