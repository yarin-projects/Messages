using Microsoft.EntityFrameworkCore;
using SecureMessagesAPI.Data;
using SecureMessagesAPI.Interfaces;
using SecureMessagesAPI.Models;

namespace SecureMessagesAPI.Repositories
{
    public class UserRepository : IUserRespository
    {
        private readonly UserContext _context;
        private readonly IEncryptionHelper _encryptionHelper;

        public UserRepository(UserContext context, IEncryptionHelper encryptionHelper)
        {
            _context = context;
            _encryptionHelper = encryptionHelper;
        }
        public int AddMessage(string messageText, int senderId, string senderName, int receiverId)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                throw new ArgumentException("Message text cannot be empty or whitespace.", nameof(messageText));

            var encryptedMessageText = _encryptionHelper.Encrypt(messageText);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var sender = GetUserTracking(senderId, senderName);

                var sentMessage = new Message
                {
                    Content = encryptedMessageText,
                    Date = DateTime.Now,
                    UserId = senderId,
                    RecipientId = receiverId
                };

                sender.Messages ??= new List<Message>();
                sender.Messages.Add(sentMessage);

                _context.SaveChanges();
                transaction.Commit();

                return sentMessage.Id;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public User GetUser(int id, string name, string password)
        {
            User user = _context.Users.AsNoTracking()
                                      .FirstOrDefault(x => x.Id == id)!;
            var decryptedName = _encryptionHelper.Decrypt(user.Name);
            var decreptedPassword = _encryptionHelper.Decrypt(user.Password);
            if (decryptedName == name && password == decreptedPassword)
                return user;
            return null!;
        }
        public User GetUserMessages(int id, string name)
        {
            User user = _context.Users.AsNoTracking()
                                      .Include(x => x.Messages)
                                      .FirstOrDefault(x => x.Id == id)!;
            var decryptedName = _encryptionHelper.Decrypt(user.Name);
            if (decryptedName == name)
                return user;
            return null!;
        }
        public User GetUserTracking(int id, string name)
        {
            User user = _context.Users.Include(x => x.Messages)
                                      .FirstOrDefault(x => x.Id == id)!;
            var decryptedName = _encryptionHelper.Decrypt(user.Name);
            if (decryptedName == name)
                return user;
            return null!;
        }
        public User GetRecipient(int id, string name)
        {
            User user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id)!;
            var decryptedName = _encryptionHelper.Decrypt(user.Name);
            if (decryptedName == name)
                return user;
            return null!;
        }
        public int GetUserCount()
        {
            return _context.Users.AsNoTracking().Count();
        }

        public ICollection<Message> GetCorrespondingUserMessages(int id, string name, int recipientId, string recipientName, int lastMessageId)
        {
            User user1 = GetUserMessages(id, name);
            User user2 = GetUserMessages(recipientId, recipientName);
            Message[] messages = user1.Messages
                            .Where(x => x.RecipientId == recipientId && lastMessageId < x.Id)
                            .Concat(user2.Messages
                            .Where(x => x.RecipientId == id && lastMessageId < x.Id))
                            .OrderBy(x => x.Date)
                            .ToArray();
            foreach (Message message in messages)
            {
                message.Content = _encryptionHelper.Decrypt(message.Content);
            }
            return messages;
        }

        public int AddUser(string name, string password)
        {
            var encryptedName = _encryptionHelper.Encrypt(name);
            var encryptedPassword = _encryptionHelper.Encrypt(password);

            var newUser = new User { Name = encryptedName, Password = encryptedPassword };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser.Id;
        }

        public int UpdateUserName(int id, string oldName, string newName)
        {
            var user = GetUserTracking(id, oldName);
            user.Name = _encryptionHelper.Encrypt(newName);
            _context.SaveChanges();
            return user.Id;
        }
        public int UpdateUserPassword(int id, string oldPassword, string newPassword)
        {
            var user = GetUserTracking(id, oldPassword);
            user.Password = _encryptionHelper.Encrypt(newPassword);
            _context.SaveChanges();
            return user.Id;
        }
        public int DeleteUser(int id, string name, string password)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = GetUserTracking(id, name) ?? throw new ArgumentException("User doesn't exist");
                var decryptedPassword = _encryptionHelper.Decrypt(user.Password);
                if (password != decryptedPassword)
                    throw new ArgumentException("Passwords don't match");

                _context.Users.Remove(user);

                var messages = _context.Messages.Where(x => x.RecipientId == id || x.UserId == id).ToArray();
                _context.Messages.RemoveRange(messages);

                _context.SaveChanges();
                transaction.Commit();

                return user.Id;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public IEnumerable<User> GetDecryptedData()
        {
            var users = _context.Users.AsNoTracking().Include(x => x.Messages).ToArray();

            foreach (var user in users)
            {
                user.Name = _encryptionHelper.Decrypt(user.Name);
                user.Password = _encryptionHelper.Decrypt(user.Password);
                foreach (var message in user.Messages)
                {
                    message.Content = _encryptionHelper.Decrypt(message.Content);
                }
            }
            return users;
        }
    }
}
