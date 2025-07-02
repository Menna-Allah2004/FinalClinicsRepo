document.addEventListener('DOMContentLoaded', function() {
  // Set current year in the footer copyright text
  const currentYearElement = document.getElementById('currentYear');
  if (currentYearElement) {
    currentYearElement.textContent = new Date().getFullYear();
  }
  
  // Add dynamic behavior to all buttons with the 'btn' class
  const buttons = document.querySelectorAll('.btn');
  buttons.forEach(button => {
    button.addEventListener('mousedown', function() {
      this.style.transform = 'scale(0.98)';
    });
    
    button.addEventListener('mouseup mouseout', function() {
      this.style.transform = 'scale(1)';
    });
  });
  
  // Initialize tooltips if Bootstrap is loaded
  if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
  }
  
  // Handle responsive behaviors
  function handleResponsiveBehavior() {
    const width = window.innerWidth;
    const navbarBrand = document.querySelector('.navbar-brand');
    
    if (width < 576) { // Small screens
      if (navbarBrand) {
        navbarBrand.innerHTML = '<span class="text-white fw-bold">MC</span>';
      }
    } else { // Larger screens
      if (navbarBrand) {
        navbarBrand.innerHTML = '<span class="text-white fw-bold">Medical</span><span class="text-primary fw-medium">Connect</span>';
      }
    }
  }
  
  // Initial call and event listener for resize events
  handleResponsiveBehavior();
  window.addEventListener('resize', handleResponsiveBehavior);
  
  // Create a simple user session handler
  const userSession = {
    // Check if user is logged in
    isLoggedIn: function() {
      return localStorage.getItem('userToken') !== null;
    },
    
    // Get current user type (patient or doctor)
    getUserType: function() {
      return localStorage.getItem('userType');
    },
    
    // Get user name for display
    getUserName: function() {
      return localStorage.getItem('userName') || 'مستخدم';
    },
    
    // Log out the current user
    logout: function() {
      localStorage.removeItem('userToken');
      localStorage.removeItem('userType');
      localStorage.removeItem('userName');
      window.location.href = 'login.html';
    }
  };
  
  // Update navbar based on login status
  const updateNavbar = () => {
    const authContainer = document.querySelector('.navbar .d-flex');
    if (!authContainer) return;
    
    if (userSession.isLoggedIn()) {
      const userType = userSession.getUserType();
      const dashboardLink = userType === 'doctor' ? 'doctor-dashboard.html' : 'patient-profile.html';
      
      authContainer.innerHTML = `
        <div class="dropdown">
          <button class="btn btn-outline-light dropdown-toggle" type="button" data-bs-toggle="dropdown">
            <i class="fas fa-user-circle me-1"></i>
            ${userSession.getUserName()}
          </button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li><a class="dropdown-item" href="${dashboardLink}"><i class="fas fa-tachometer-alt me-2"></i> لوحة التحكم</a></li>
            <li><a class="dropdown-item" href="#" id="logoutBtn"><i class="fas fa-sign-out-alt me-2"></i> تسجيل الخروج</a></li>
          </ul>
        </div>
      `;
      
      // Add logout functionality
      document.getElementById('logoutBtn').addEventListener('click', (e) => {
        e.preventDefault();
        userSession.logout();
      });
    }
  };
  
  // Call the navbar update
  updateNavbar();
  
  // Expose utilities to global scope (for other scripts to use)
  window.medicalConnect = {
    showToast: function(title, message, type = 'info') {
      // Check if toast container exists, if not create it
      let toastContainer = document.querySelector('.toast-container');
      
      if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(toastContainer);
      }
      
      // Create toast element
      const toastId = `toast-${Date.now()}`;
      const iconClass = type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle';
      const bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';
      
      const toastHtml = `
        <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
          <div class="toast-header ${bgClass} text-white">
            <i class="fas fa-${iconClass} me-2"></i>
            <strong class="me-auto">${title}</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
          </div>
          <div class="toast-body">
            ${message}
          </div>
        </div>
      `;
      
      toastContainer.insertAdjacentHTML('beforeend', toastHtml);
      
      // Initialize and show the toast
      const toastElement = document.getElementById(toastId);
      const toast = new bootstrap.Toast(toastElement, { delay: 5000 });
      toast.show();
      
      // Remove toast element after it's hidden
      toastElement.addEventListener('hidden.bs.toast', function() {
        toastElement.remove();
      });
    },
    
    // Helper function for making API calls
    fetchAPI: async function(url, options = {}) {
      try {
        // Set default headers if not specified
        if (!options.headers) {
          options.headers = {
            'Content-Type': 'application/json'
          };
        }
        
        // Add authentication token if user is logged in
        if (userSession.isLoggedIn()) {
          options.headers.Authorization = `Bearer ${localStorage.getItem('userToken')}`;
        }
        
        const response = await fetch(url, options);
        
        // Check if response is ok (status 200-299)
        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'حدث خطأ في الاتصال بالخادم');
        }
        
        return await response.json();
      } catch (error) {
        console.error('API Error:', error);
        this.showToast('خطأ', error.message || 'حدث خطأ في الاتصال بالخادم', 'error');
        throw error;
      }
    },
    
    // User session management
    userSession: userSession
  };
});