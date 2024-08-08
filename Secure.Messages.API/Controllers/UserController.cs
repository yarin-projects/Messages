using Microsoft.AspNetCore.Mvc;
using SecureMessagesAPI.Encryption;
using SecureMessagesAPI.Interfaces;
using SecureMessagesAPI.Models;

namespace SecureMessagesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserRespository _repository;

        public UserController(IUserRespository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet("/api/users/get")]
        public IActionResult GetUserCount()
        {
            try
            {
                var usersCount = _repository.GetUserCount();
                return Ok(usersCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates a user based on userId and userName.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The validated user.</returns>
        [HttpGet("/api/user/login")]
        public IActionResult ValidateUser([FromQuery] int userId, [FromQuery] string userName, [FromQuery] string password)
        {
            try
            {
                User user = _repository.GetUser(userId, userName, password);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates a recipient based on recipientId and recipientName.
        /// </summary>
        /// <param name="recipientId">The ID of the recipient.</param>
        /// <param name="recipientName">The name of the recipient.</param>
        /// <returns>The validated recipient.</returns>
        [HttpGet("/api/recipient/validate")]
        public IActionResult ValidateRecipient(int recipientId, string recipientName)
        {
            try
            {
                User user = _repository.GetRecipient(recipientId, recipientName);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userName">The name of the user to register.</param>
        /// <param name="password">The password of the user to register.</param>
        /// <returns>The ID of the registered user.</returns>
        [HttpPost("/api/user/register")]
        public IActionResult RegisterUser([FromQuery] string userName, [FromQuery] string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest("Invalid name or password");
                }

                var id = _repository.AddUser(userName, password);
                return Ok(new { message = "User registered successfully!", userId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves messages between two users.
        /// </summary>
        /// <param name="id">The ID of the first user.</param>
        /// <param name="name">The name of the first user.</param>
        /// <param name="recipientId">The ID of the second user.</param>
        /// <param name="recipientName">The name of the second user.</param>
        /// <param name="lastMessageId">The last message id requested from client</param>
        /// <returns>The messages exchanged between the two users.</returns>
        [HttpGet("/api/message/get")]
        public IActionResult GetCorrespondingUserMessages(int id, string name, int recipientId, string recipientName, int lastMessageId)
        {
            try
            {
                var messages = _repository.GetCorrespondingUserMessages(id, name, recipientId, recipientName, lastMessageId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new message sent by a user.
        /// </summary>
        /// <param name="messageText">The content of the message.</param>
        /// <param name="senderId">The ID of the sender.</param>
        /// <param name="senderName">The name of the sender.</param>
        /// <param name="receiverId">The ID of the recipient.</param>
        /// <returns>The ID of the added message.</returns>
        [HttpPost("/api/message/add")]
        public IActionResult AddMessage(string messageText , int senderId, string senderName, int receiverId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageText))
                {
                    return BadRequest("Message text cannot be empty or whitespace.");
                }

                var messageId = _repository.AddMessage(messageText, senderId, senderName, receiverId);
                return Ok(new { message = "Message added successfully!", messageId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves decrypted data for authorized users.
        /// </summary>
        /// <param name="password">The password for authorization.</param>
        /// <returns>Decrypted user data.</returns>
        [HttpGet("/api/get/decrypted/all")]
        public IActionResult GetDecryptedData(string password) 
        {
            try
            {
                if (password == "admin")
                {
                    var data = _repository.GetDecryptedData();
                    return Ok(data);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the username of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="oldName">The old name of the user.</param>
        /// <param name="newName">The new name of the user.</param>
        /// <returns>The ID of the updated user.</returns>
        [HttpPatch("/api/user/update/name")]
        public IActionResult UpdateUserName(int id, string oldName, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    return BadRequest("New name cannot be empty or whitespace.");
                }

                var updatedUserId = _repository.UpdateUserName(id, oldName, newName);
                return Ok(new { message = "User name updated successfully!", userId = updatedUserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the password of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="oldPassword">The old password of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <returns>The ID of the updated user.</returns>
        [HttpPatch("/api/user/update/password")]
        public IActionResult UpdateUserPassword(int id, string oldPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return BadRequest("New password cannot be empty or whitespace.");
                }

                var updatedUserId = _repository.UpdateUserPassword(id, oldPassword, newPassword);
                return Ok(new { message = "User password updated successfully!", userId = updatedUserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a user from the system based on the provided ID, name, and password.
        /// </summary>
        /// <param name="id">The ID of the user to be deleted.</param>
        /// <param name="name">The name of the user to be deleted.</param>
        /// <param name="password">The password of the user to be deleted.</param>
        /// <returns>Returns success message and user ID if the operation is successful.</returns>
        [HttpDelete("/api/user/delete")]
        public IActionResult DeleteUser(int id, string name, string password)
        {
            try
            {
                var deletedUserId = _repository.DeleteUser(id, name, password);
                return Ok(new { message = "User deleted successfully!", userId = deletedUserId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
