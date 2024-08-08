using Microsoft.EntityFrameworkCore;
using SecureMessagesAPI.Models;

namespace SecureMessagesAPI.Interfaces
{
    public interface IUserRespository
    {
        int GetUserCount();
        int AddUser(string name, string password);
        User GetUser(int id, string name, string password);
        User GetRecipient(int id, string name);
        int UpdateUserPassword(int id, string oldPassword, string newPassword);
        User GetUserTracking(int id, string name);
        User GetUserMessages(int id, string name);
        int UpdateUserName(int id, string oldName, string newName);
        int DeleteUser(int id, string name, string password);
        ICollection<Message> GetCorrespondingUserMessages(int id, string name, int recipientId, string recipientName, int lastMessageId);
        int AddMessage(string message, int senderId, string senderName, int reciverId);
        IEnumerable<User> GetDecryptedData();
    }
}
