document.addEventListener('DOMContentLoaded', function () {
  const updateUserNameBtn = document.getElementById('updateUserNameBtn');
  const updatePasswordBtn = document.getElementById('updatePasswordBtn');

  const userName = sessionStorage.getItem('userName');
  const password = sessionStorage.getItem('password');
  const userId = sessionStorage.getItem('userId');

  validateUser(userId, userName, password, false).then(x => {
    if (!x) {
      window.location.href = 'error.html';
      return;
    }
  });

  const goBackBtn = document.getElementById('goBackBtn');
  goBackBtn.textContent = 'Go Back';
  goBackBtn.addEventListener('click', () => {
    window.location.href = 'user.html';
  });

  document.getElementById('oldUserName').value = userName;
  document.getElementById('oldPassword').value = password;

  updateUserNameBtn.addEventListener('click', async function () {
    disableButtons();

    const newUserName = document.getElementById('newUserName').value.trim();

    if (!newUserName) {
      enableButtons();
      showToast('New username cannot be empty.', 'error');
      return;
    }

    try {
      const response = await axios.patch(apiCall + 'user/update/name', null, {
        params: {
          id: userId,
          oldName: userName,
          newName: newUserName,
        },
      });
      showToast(response.data.message, 'success');
      sessionStorage.setItem('userName', newUserName);
      redirectToUserPage();
    } catch (error) {
      enableButtons();
      console.error('Error updating username:', error);
      showToast('An error occurred while updating the username.', 'error');
    }
  });

  updatePasswordBtn.addEventListener('click', async function () {
    disableButtons();

    const newPassword = document.getElementById('newPassword').value.trim();

    if (!newPassword) {
      enableButtons();
      showToast('New password cannot be empty.', 'error');
      return;
    }

    try {
      const response = await axios.patch(apiCall + 'user/update/password', null, {
        params: {
          id: userId,
          oldPassword: password,
          newPassword: newPassword,
        },
      });
      showToast(response.data.message, 'success');
      sessionStorage.setItem('password', newPassword);
      redirectToUserPage();
    } catch (error) {
      enableButtons();
      console.error('Error updating password:', error);
      showToast('An error occurred while updating the password.', 'error');
    }
  });
});

const enableButtons = () => {
  updateUserNameBtn.className = 'btn';
  updatePasswordBtn.className = 'btn';
  updateUserNameBtn.disabled = false;
  updatePasswordBtn.disabled = false;
};

const disableButtons = () => {
  updateUserNameBtn.className = 'btn-disabled';
  updatePasswordBtn.className = 'btn-disabled';
  updateUserNameBtn.disabled = true;
  updatePasswordBtn.disabled = true;
};

const redirectToUserPage = () => {
  setTimeout(() => {
    enableButtons();
    window.location.href = 'user.html';
  }, 2500);
};
