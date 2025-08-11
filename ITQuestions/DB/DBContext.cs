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
        public DbSet<ITQuestion> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Placeholder for future connection string
            try
            {
                optionsBuilder.UseSqlite("DB connection string"); // or some other database, will see
            }
            catch (Exception)
            {
                throw new DatabaseNotFoundException();
            }
        }
    }
}
