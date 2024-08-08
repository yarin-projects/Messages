document.addEventListener('DOMContentLoaded', function () {
  const userId = sessionStorage.getItem('userId');
  const userName = sessionStorage.getItem('userName');

  if (userId === 'admin' && userName === 'admin') {
    const getDecryptedData = async () => {
      try {
        const response = await axios.get(apiCall + 'get/decrypted/all', {
          params: {
            password: password,
          },
        });
        renderDecryptedData(response.data);
      } catch (error) {
        console.error(error);
        showToast('An error occurred while fetching decrypted data', 'error');
      }
    };

    const renderDecryptedData = data => {
      const decryptedDataDiv = document.getElementById('decryptedData');
      decryptedDataDiv.innerHTML = '';
      data.forEach(user => {
        const userDiv = document.createElement('div');
        userDiv.classList.add('user');
        const userName = document.createElement('h2');
        userName.textContent = `Name: ${user.name}`;
        userDiv.appendChild(userName);
        const userId = document.createElement('h3');
        userId.textContent = `ID: ${user.id}`;
        userDiv.appendChild(userId);
        const userPassword = document.createElement('h3');
        userPassword.textContent = `Password: ${user.password}`;
        userDiv.appendChild(userPassword);
        const messagesList = document.createElement('ul');
        user.messages.forEach(message => {
          const messageItem = document.createElement('li');
          messageItem.classList.add('message');
          messageItem.innerHTML = convertNewlinesToBreaks(message.content);
          const messageInfo = document.createElement('div');
          messageInfo.classList.add('message-info');
          const messageDate = new Date(message.date);
          const formattedDate = `${messageDate.getDate()}/${
            messageDate.getMonth() + 1
          }/${messageDate.getFullYear()} | ${messageDate.getHours()}:${messageDate.getMinutes()} `;
          messageInfo.textContent = `Date: ${formattedDate} | Recipient ID: ${message.recipientId}`;
          messageItem.appendChild(messageInfo);
          messagesList.appendChild(messageItem);
        });
        userDiv.appendChild(messagesList);
        decryptedDataDiv.appendChild(userDiv);
      });

      const goBackBtn = document.getElementById('goBackBtn');
      goBackBtn.textContent = 'Go Back';
      goBackBtn.addEventListener('click', () => {
        window.location.href = 'index.html';
      });
    };

    getDecryptedData();
  } else {
    window.location.href = 'error.html';
  }
});
