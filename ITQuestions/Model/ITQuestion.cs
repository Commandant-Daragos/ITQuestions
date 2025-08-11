using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITQuestions.Model
{
    public class ITQuestion
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
