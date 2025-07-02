// login-bootstrap.js

document.addEventListener('DOMContentLoaded', function () {
    const togglePasswordBtn = document.getElementById('togglePassword');
    const passwordInput = document.querySelector('input[type="password"][name="Password"]');
    const passwordIcon = document.getElementById('passwordIcon');

    if (togglePasswordBtn && passwordInput && passwordIcon) {
        togglePasswordBtn.addEventListener('click', function () {
            const isHidden = passwordInput.type === 'password';
            passwordInput.type = isHidden ? 'text' : 'password';

            passwordIcon.classList.toggle('fa-eye', !isHidden);
            passwordIcon.classList.toggle('fa-eye-slash', isHidden);
        });
    }
});
