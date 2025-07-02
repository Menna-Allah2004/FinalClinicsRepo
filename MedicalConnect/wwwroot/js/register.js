// ======= Register Page JavaScript Logic =======

// DOM Elements
const patientBtn = document.getElementById('patientBtn');
const doctorBtn = document.getElementById('doctorBtn');
const registerForm = document.getElementById('registerForm');
const doctorFields = document.getElementById('doctorFields');
const togglePasswordBtn = document.getElementById('togglePassword');
const toggleConfirmPasswordBtn = document.getElementById('toggleConfirmPassword');
const passwordInput = document.getElementById('password');
const confirmPasswordInput = document.getElementById('confirmPassword');
const passwordIcon = document.getElementById('passwordIcon');
const confirmPasswordIcon = document.getElementById('confirmPasswordIcon');

// User type state (default: patient)
let userType = 'patient';

// Event Listeners
document.addEventListener('DOMContentLoaded', function() {
  // User type tabs functionality
  patientBtn.addEventListener('click', () => setUserType('patient'));
  doctorBtn.addEventListener('click', () => setUserType('doctor'));
  
  // Toggle password visibility
  togglePasswordBtn.addEventListener('click', () => togglePasswordVisibility(passwordInput, passwordIcon));
  toggleConfirmPasswordBtn.addEventListener('click', () => togglePasswordVisibility(confirmPasswordInput, confirmPasswordIcon));
  
  // Form submission
  registerForm.addEventListener('submit', handleSubmit);
});

// Set user type function
function setUserType(type) {
  userType = type;
  
  if (type === 'patient') {
    patientBtn.classList.add('active-tab');
    doctorBtn.classList.remove('active-tab');
    doctorFields.classList.add('d-none');
    
    // Make doctor fields not required
    const doctorRequiredFields = doctorFields.querySelectorAll('input, select, textarea');
    doctorRequiredFields.forEach(field => {
      field.removeAttribute('required');
    });
  } else {
    doctorBtn.classList.add('active-tab');
    patientBtn.classList.remove('active-tab');
    doctorFields.classList.remove('d-none');
    
    // Make doctor fields required
    const doctorRequiredFields = doctorFields.querySelectorAll('input, select');
    doctorRequiredFields.forEach(field => {
      if (field.id !== 'bio') {
        field.setAttribute('required', '');
      }
    });
  }
}

// Toggle password visibility
function togglePasswordVisibility(inputField, iconEl) {
  if (inputField.type === 'password') {
    inputField.type = 'text';
    iconEl.classList.remove('fa-eye');
    iconEl.classList.add('fa-eye-slash');
  } else {
    inputField.type = 'password';
    iconEl.classList.remove('fa-eye-slash');
    iconEl.classList.add('fa-eye');
  }
}

// Handle form submission
function handleSubmit(event) {
  event.preventDefault();
  
  // Form validation
  if (!registerForm.checkValidity()) {
    event.stopPropagation();
    registerForm.classList.add('was-validated');
    return;
  }
  
  // Check if passwords match
  if (passwordInput.value !== confirmPasswordInput.value) {
    showToast('خطأ في التسجيل', 'كلمات المرور غير متطابقة', 'danger');
    confirmPasswordInput.setCustomValidity('كلمات المرور غير متطابقة');
    registerForm.classList.add('was-validated');
    return;
  }
  
  // Get form data
  const formData = {
    fullName: document.getElementById('fullName').value,
    email: document.getElementById('email').value,
    phone: document.getElementById('phone').value,
    birthDate: document.getElementById('birthDate').value,
    gender: document.getElementById('gender').value,
    city: document.getElementById('city').value,
    password: document.getElementById('password').value,
    userType: userType
  };
  
  // Add doctor specific fields if applicable
  if (userType === 'doctor') {
    formData.specialty = document.getElementById('specialty').value;
    formData.experience = document.getElementById('experience').value;
    formData.license = document.getElementById('license').value;
    formData.workplace = document.getElementById('workplace').value;
    formData.bio = document.getElementById('bio').value;
  }
  
  console.log('Registration submitted', formData);
  
  // Show toast notification
  showToast('تم إنشاء الحساب بنجاح', 'جاري توجيهك لصفحة تسجيل الدخول');
  
  // Redirect to login page after successful registration
  setTimeout(() => {
    window.location.href = 'login.html';
  }, 1500);
}

// Toast notification function
function showToast(title, message, type = 'primary') {
  // Create toast container if it doesn't exist
  let toastContainer = document.querySelector('.toast-container');
  if (!toastContainer) {
    toastContainer = document.createElement('div');
    toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    document.body.appendChild(toastContainer);
  }
  
  // Create toast element
  const toastEl = document.createElement('div');
  toastEl.className = 'toast show fade-in';
  toastEl.setAttribute('role', 'alert');
  toastEl.setAttribute('aria-live', 'assertive');
  toastEl.setAttribute('aria-atomic', 'true');
  
  // Toast content with appropriate type
  toastEl.innerHTML = `
    <div class="toast-header bg-${type} text-white">
      <strong class="ms-auto">${title}</strong>
      <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="إغلاق"></button>
    </div>
    <div class="toast-body">
      ${message}
    </div>
  `;
  
  // Add toast to container
  toastContainer.appendChild(toastEl);
  
  // Auto remove after 5 seconds
  setTimeout(() => {
    toastEl.remove();
  }, 5000);
  
  // Close button functionality
  const closeBtn = toastEl.querySelector('.btn-close');
  closeBtn.addEventListener('click', () => {
    toastEl.remove();
  });
}