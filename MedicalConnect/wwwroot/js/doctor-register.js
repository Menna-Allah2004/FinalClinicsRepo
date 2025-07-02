// ======= Doctor Register Page JavaScript Logic (Cleaned) =======

document.addEventListener('DOMContentLoaded', function () {
    // Password visibility toggle
    const togglePassword = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('Password');
    const passwordIcon = document.getElementById('passwordIcon');

    if (togglePassword) {
        togglePassword.addEventListener('click', function () {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
            passwordIcon.classList.toggle('fa-eye');
            passwordIcon.classList.toggle('fa-eye-slash');
        });
    }

    // Confirm password visibility toggle
    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const confirmPasswordIcon = document.getElementById('confirmPasswordIcon');

    if (toggleConfirmPassword) {
        toggleConfirmPassword.addEventListener('click', function () {
            const type = confirmPasswordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            confirmPasswordInput.setAttribute('type', type);
            confirmPasswordIcon.classList.toggle('fa-eye');
            confirmPasswordIcon.classList.toggle('fa-eye-slash');
        });
    }

    // Password match check only
    const registerForm = document.getElementById('doctorRegisterForm');

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            if (passwordInput.value !== confirmPasswordInput.value) {
                e.preventDefault(); // prevent form submission
                showToast('خطأ في التحقق', 'كلمات المرور غير متطابقة، يرجى التحقق والمحاولة مرة أخرى', 'danger');
            }
        });
    }
});

// Toast notification function
function showToast(title, message, type = 'primary') {
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(toastContainer);
    }

    const toastEl = document.createElement('div');
    toastEl.className = 'toast show fade-in';
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');

    toastEl.innerHTML = `
        <div class="toast-header bg-${type} text-white">
            <strong class="ms-auto">${title}</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="إغلاق"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    toastContainer.appendChild(toastEl);

    setTimeout(() => {
        toastEl.remove();
    }, 5000);

    toastEl.querySelector('.btn-close').addEventListener('click', () => {
        toastEl.remove();
    });
}