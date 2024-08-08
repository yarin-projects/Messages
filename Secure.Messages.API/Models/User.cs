namespace SecureMessagesAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
