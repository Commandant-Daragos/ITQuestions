using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ITQuestions.Model
{
    public class ITQuestion
    {
        [Key]
        [JsonIgnore]
        public string FirebaseKey { get; set; } = Guid.NewGuid().ToString();

        public string Question { get; set; }
        public string Answer { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
