using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.Exceptions
{
    public class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException()
        { }
            //: base("Database was not found.") { }

        public DatabaseNotFoundException(string message)
            : base(message) { }

        public DatabaseNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}