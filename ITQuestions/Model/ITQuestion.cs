using ITQuestions.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ITQuestions.Model
{
    public class ITQuestion
    {
        [Key]
        [JsonIgnore]
        public string FirebaseKey { get; set; } = Guid.NewGuid().ToString();

        public string? Question { get; set; }
        public string? Answer { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public SyncStatus SyncStatus { get; set; } = SyncStatus.None;
    }
}
