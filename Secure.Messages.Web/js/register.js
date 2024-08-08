document.addEventListener('DOMContentLoaded', function () {
  const registerForm = document.getElementById('registerForm');

  clearSessionStorage(true);

  registerForm.addEventListener('submit', function (event) {
    event.preventDefault();
    const name = document.getElementById('name').value.trim();
    const password = document.getElementById('password').value.trim();
    registerUser(name, password);
  });

  const registerUser = async (name, password) => {
    try {
      const response = await axios.post(apiCall + `user/register?userName=${encodeURIComponent(name)}&password=${encodeURIComponent(password)}`);
      console.log(response.data.message);
      showToast(`${response.data.message}\nYour ID is ${response.data.userId}`, 'success');
      setTimeout(() => {
        window.location.href = 'login.html';
      }, 3500);
    } catch (error) {
      console.error(error);
      showToast('An error occurred while registering the user.', 'error');
    }
  };
});

function redirectToHomePage() {
  window.location.href = 'index.html';
}
