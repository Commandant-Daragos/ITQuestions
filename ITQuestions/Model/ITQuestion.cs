using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ITQuestions.Model
{
    public class ITQuestion
    {
        [JsonIgnore] // Don’t send the key back to Firebase automatically
        public string FirebaseKey { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
