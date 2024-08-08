const clearSessionStorage = isLogout => {
   if (isLogout) {
      sessionStorage.removeItem('userId');
      sessionStorage.removeItem('userName');
      sessionStorage.removeItem('password');
   }
   sessionStorage.removeItem('recipientId');
   sessionStorage.removeItem('recipientName');
   sessionStorage.removeItem('errorMessage');
};

function showToast(message, type) {
   // Configure toastr options
   toastr.options = {
      closeButton: true,
      progressBar: true,
      positionClass: 'toast-top-right',
      showMethod: 'slideDown',
      timeOut: 3500,
   };

   if (type === 'error') {
      toastr.error(message);
   } else {
      toastr.success(message);
   }
}

const apiCall = 'http://localhost:5259/api/';

const validateUser = async (id, name, password, toastrFlag, toastrMsg) => {
   try {
      const response = await axios.get(
         apiCall + `user/login?userId=${id}&userName=${encodeURIComponent(name)}&password=${encodeURIComponent(password)}`
      );
      const user = response.data;
      if (user) {
         return true;
      }
   } catch (error) {
      if (toastrFlag) {
         console.error('Error validating user:', error);
         showToast(toastrMsg, 'error');
      }
      return false;
   }
};

const validateRecipient = async (id, name) => {
   try {
      const response = await axios.get(apiCall + `recipient/validate?recipientId=${id}&recipientName=${encodeURIComponent(name)}`);
      const recipient = response.data;

      if (recipient) {
         return true;
      }
      return false;
   } catch (error) {
      return false;
   }
};

const escapeHtml = text => {
   return text.replace(/&/g, '&amp;')
              .replace(/</g, '&lt;')
              .replace(/>/g, '&gt;')
              .replace(/"/g, '&quot;')
              .replace(/'/g, '&#039;');
};

const convertNewlinesToBreaks = text => {
   return text.replace(/\n/g, '<br>');
};

const password = 'admin';

const getDecryptedData = async password => {
   try {
      const response = await axios.get(apiCall + 'get/decrypted/all', {
         params: {
            password: password,
         },
      });
      return response.data;
   } catch (error) {
      return;
   }
};
