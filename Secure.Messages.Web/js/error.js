let redirectToHomePage = () => {
  window.location.href = 'index.html';
};

document.addEventListener('DOMContentLoaded', function () {
  let errorMessage = sessionStorage.getItem('errorMessage');
  if (errorMessage === 'Recipient not authenticated. Please try again.') {
    document.getElementById('errorMessage').textContent = errorMessage;
    clearSessionStorage(false);
    redirectToHomePage = () => {
      window.location.href = 'user.html';
    };
  } else {
    clearSessionStorage(true);
    errorMessage = 'User not authenticated. Please log in.';
    document.getElementById('errorMessage').textContent = errorMessage;
  }
});
