using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SecureMessagesAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int RecipientId { get; set;}
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
