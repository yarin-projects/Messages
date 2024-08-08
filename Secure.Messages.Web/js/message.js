document.addEventListener('DOMContentLoaded', function () {
   const userId = sessionStorage.getItem('userId');
   const userName = sessionStorage.getItem('userName');
   const password = sessionStorage.getItem('password');
   const recipientId = sessionStorage.getItem('recipientId');
   const recipientName = sessionStorage.getItem('recipientName');

   if (recipientId === userId) {
      sessionStorage.setItem('errorMessage', 'You cannot chat with yourself.');
      window.location.href = 'error.html';
      return;
   }

   validateUser(userId, userName, password, false).then(x => {
      if (!x) {
         window.location.href = 'error.html';
         return;
      } else {
         validateRecipient(Number(recipientId), recipientName).then(x => {
            if (!x) {
               sessionStorage.setItem('errorMessage', 'Recipient not authenticated. Please try again.');
               window.location.href = 'error.html';
               return;
            }
         });
      }
   });

   const chatMessages = document.getElementById('chatMessages');
   const messageForm = document.getElementById('messageForm');
   const messageText = document.getElementById('messageText');
   const goBackBtn = document.getElementById('goBackBtn');
   let receivedMessageIds = [];
   let lastMessageId = 0;

   const recipientNameElement = document.getElementById('recipientName');
   const title = document.getElementById('title');
   title.textContent = `${userName}'s Chats`;
   recipientNameElement.textContent = `Chatting with ${recipientName}`;

   const fetchMessages = () => {
      axios
         .get(apiCall + 'message/get', {
            params: {
               id: userId,
               name: userName,
               recipientId: recipientId,
               recipientName: recipientName,
               lastMessageId: lastMessageId,
            },
         })
         .then(response => {
            const messages = response.data;
            messages.forEach(message => {
               if (!receivedMessageIds.includes(message.id)) {
                  const messageElement = document.createElement('div');
                  messageElement.classList.add('message');

                  if (message.recipientId == recipientId) {
                     messageElement.classList.add('sent');
                  } else {
                     messageElement.classList.add('received');
                  }

                  // Format message date
                  const messageDate = new Date(message.date);
                  const formattedDate = formatTimestamp(messageDate);

                  // Set message content as text
                  const messageContent = document.createElement('div');
                  messageContent.classList.add('message-content');
                  const safeMessage = escapeHtml(message.content);
                  const formattedMessage = convertNewlinesToBreaks(safeMessage);
                  messageContent.innerHTML = formattedMessage;

                  // Create message timestamp element
                  const timestampElement = document.createElement('div');
                  timestampElement.classList.add('message-timestamp');
                  timestampElement.textContent = formattedDate;

                  // Append message content and timestamp to message element
                  messageElement.appendChild(messageContent);
                  messageElement.appendChild(timestampElement);

                  // Append message element to chat messages container
                  chatMessages.appendChild(messageElement);

                  // Add message ID to receivedMessageIds array
                  receivedMessageIds.push(message.id);
                  lastMessageId = message.id;
                  chatMessages.scrollTop = chatMessages.scrollHeight;
               }
            });
         })
         .catch(error => {
            console.error('Error fetching messages:', error);
            showToast('Error fetching messages. Please try again later.', 'error');
         });
   };

   messageForm.addEventListener('submit', async function (event) {
      event.preventDefault();

      const message = messageText.value;
      if (message === '') return;

      try {
         const response = await axios.post(apiCall + `message/add`, null, {
            params: {
               messageText: message,
               senderId: userId,
               senderName: userName,
               receiverId: recipientId,
            },
         });
         receivedMessageIds.push(response.data.messageId);

         // Display the sent message in the chat
         const messageElement = document.createElement('div');
         messageElement.classList.add('message', 'sent');

         const messageDate = new Date();
         const formattedDate = formatTimestamp(messageDate);

         // Set message content as text
         const messageContent = document.createElement('div');
         messageContent.classList.add('message-content');
         const safeMessage = escapeHtml(message);
         const formattedMessage = convertNewlinesToBreaks(safeMessage);
         messageContent.innerHTML = formattedMessage;

         // Create message timestamp element
         const timestampElement = document.createElement('div');
         timestampElement.classList.add('message-timestamp');
         timestampElement.textContent = formattedDate;

         // Append message content and timestamp to message element
         messageElement.appendChild(messageContent);
         messageElement.appendChild(timestampElement);

         // Append message element to chat messages container
         chatMessages.appendChild(messageElement);

         // Scroll to the bottom
         chatMessages.scrollTop = chatMessages.scrollHeight;

         // Clear the message input
         messageText.value = '';
      } catch (error) {
         console.error('Error sending message:', error);
         showToast('An error occurred while sending the message.', 'error');
      }
   });

   messageText.addEventListener('keydown', function (event) {
      // Check if Enter key is pressed
      if (event.key === 'Enter' && !event.shiftKey) {
         event.preventDefault(); // Prevent default behavior (adding newline)
         // Trigger the form submission
         messageForm.dispatchEvent(new Event('submit'));
      }
   });

   messageText.addEventListener('keydown', function (event) {
      // Check if Shift + Enter key combination is pressed
      if (event.key === 'Enter' && event.shiftKey) {
         // Add a newline character in the textarea
         this.value += '\n';
         // Prevent the default behavior of adding a newline
         event.preventDefault();
      }
   });

   goBackBtn.addEventListener('click', function () {
      clearSessionStorage(false);
      window.location.href = 'user.html';
   });

   function formatTimestamp(messageDate) {
      const now = new Date();
      const diff = Math.abs(now - messageDate);
      const minutes = Math.floor(diff / (1000 * 60));
      const oneYearInMinutes = 60 * 24 * 365;

      if (minutes < 1440) {
         // Less than 24 hours
         return `${messageDate.getHours()}:${(messageDate.getMinutes() < 10 ? '0' : '') + messageDate.getMinutes()}`; // Format hours and minutes
      } else if (minutes < oneYearInMinutes) {
         // Less than 1 year
         return `${messageDate.getDate()}/${messageDate.getMonth() + 1}`; // Format day and month
      } else {
         // Older than 1 year
         return `${messageDate.getDate()}/${messageDate.getMonth() + 1}/${messageDate.getFullYear()}`; // Format day, month, and year
      }
   }

   fetchMessages();
   setInterval(fetchMessages, 5000);
});
