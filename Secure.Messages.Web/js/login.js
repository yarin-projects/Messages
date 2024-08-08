document.addEventListener('DOMContentLoaded', function () {
  clearSessionStorage(true);

  const getAllUsers = async () => {
    try {
      const response = await axios.get(apiCall + 'users/get');
      const users = response.data;
      if (users < 2) {
        showToast('There are less than 2 users. Please register to proceed.', 'error');
        setTimeout(() => {
          window.location.href = 'register.html'; 
        }, 3500);
      }
    } catch (error) {
      console.error(error);
      showToast('An error occurred while polling the server', 'error');
    }
  };

  getAllUsers();

  const checkAdmin = () => {
    if (document.getElementById('userName').value === 'admin' && document.getElementById('password').value === 'admin') {
      return true;
    }
    return false;
  };

  const loginForm = document.getElementById('loginForm');

  loginForm.addEventListener('submit', async function (event) {
    event.preventDefault();
    if (checkAdmin()) {
      sessionStorage.setItem('userId', 'admin');
      sessionStorage.setItem('userName', 'admin');
      window.location.href = 'admin.html';
      return;
    }
    const userId = Number(document.getElementById('userId').value);
    const userName = document.getElementById('userName').value.trim();
    const password = document.getElementById('password').value.trim();

    validateUser(userId, userName, password, true, "User doesn't exist. Please try again.").then(x => {
      if (x) {
        sessionStorage.setItem('userId', userId);
        sessionStorage.setItem('userName', userName);
        sessionStorage.setItem('password', password);

        window.location.href = 'user.html';
      } else {
        return;
      }
    });
  });
});

function redirectToHomePage() {
  window.location.href = 'index.html';
}
