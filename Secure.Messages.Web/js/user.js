document.addEventListener('DOMContentLoaded', function () {
  clearSessionStorage(false);

  const userName = sessionStorage.getItem('userName');
  const userId = sessionStorage.getItem('userId');
  const password = sessionStorage.getItem('password');

  validateUser(userId, userName, password, false).then(x => {
    if (!x) {
      window.location.href = 'error.html';
      return;
    }
  });

  const welcomeHeader = document.getElementById('welcomeHeader');
  welcomeHeader.textContent = `${userName} Homepage`;

  const openChatBtn = document.getElementById('openChatBtn');
  openChatBtn.addEventListener('click', async function () {
    const recipientUserId = Number(document.getElementById('userId').value);
    const recipientUserName = document.getElementById('userName').value.trim();

    try {
      const response = await axios.get(
        apiCall + `recipient/validate?recipientId=${recipientUserId}&recipientName=${encodeURIComponent(recipientUserName)}`
      );
      const recipient = response.data;

      if (recipient) {
        // Check if recipient ID is different from current user ID
        if (userId !== recipientUserId.toString()) {
          // Store recipient ID and Name in sessionStorage
          sessionStorage.setItem('recipientId', recipientUserId);
          sessionStorage.setItem('recipientName', recipientUserName);

          // Redirect to message.html
          window.location.href = 'message.html';
        } else {
          showToast('You cannot send a message to yourself. Please choose a different recipient.', 'error');
        }
      } else {
        showToast("User doesn't exist. Please try again.", 'error');
      }
    } catch (error) {
      console.error('Error validating user:', error);
      showToast('An error occurred while validating recipient user.', 'error');
    }
  });

  const logoutBtn = document.getElementById('logoutBtn');
  logoutBtn.addEventListener('click', function () {
    window.location.href = 'index.html';
  });

  const updateBtn = document.getElementById('updateBtn');
  updateBtn.addEventListener('click', function () {
    window.location.href = 'update.html';
  });

  const removeUserBtn = document.getElementById('removeUserBtn');
  const modal = document.getElementById('confirmationModal');
  const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
  const cancelDeleteBtn = document.getElementById('cancelDeleteBtn');

  removeUserBtn.addEventListener('click', function () {
    modal.style.display = 'block';
  });

  confirmDeleteBtn.addEventListener('click', async function () {
    modal.style.display = 'none';
    try {
      const response = await axios.delete(apiCall + 'user/delete', {
        params: {
          id: userId,
          name: userName,
          password: password,
        },
      });
      if (response.status === 200) {
        sessionStorage.clear();
        showToast(response.data.message, 'success');
        setTimeout(() => {
          window.location.href = 'index.html';
        }, 3500);
      } else {
        showToast('An error occurred while deleting the user.', 'error');
      }
    } catch (error) {
      console.error('Error deleting user:', error);
      showToast('An error occurred while deleting the user.', 'error');
    }
  });

  cancelDeleteBtn.addEventListener('click', function () {
    modal.style.display = 'none';
  });
});
