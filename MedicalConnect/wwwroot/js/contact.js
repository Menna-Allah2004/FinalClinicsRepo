// ======= Contact Page JavaScript Logic =======

document.addEventListener('DOMContentLoaded', function() {
  // Handle contact form submission
  const contactForm = document.getElementById('contactForm');
  
  if (contactForm) {
    contactForm.addEventListener('submit', function(e) {
      e.preventDefault();
      
      // Get form data
      const formData = {
        name: document.getElementById('name').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value,
        subject: document.getElementById('subject').value,
        message: document.getElementById('message').value
      };
      
      console.log('Contact form submitted', formData);
      
      // Show success toast
      showToast('تم إرسال رسالتك بنجاح', 'سنقوم بالرد عليك في أقرب وقت ممكن.');
      
      // Reset form
      contactForm.reset();
    });
  }
  
  // Initialize animations
  function observeElements() {
    const observerOptions = {
      threshold: 0.1
    };
    
    const Observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-reveal');
          Observer.unobserve(entry.target);
        }
      });
    }, observerOptions);
    
    document.querySelectorAll('.animate-on-scroll').forEach(element => {
      Observer.observe(element);
    });
  }
  
  // Call initialization functions
  observeElements();
});

// Simple toast notification function
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

// Animation reveal class
document.documentElement.style.setProperty('--animate-reveal', `
  @keyframes animateReveal {
    from {
      opacity: 0;
      transform: translateY(30px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
`);

// Add the animation style
const style = document.createElement('style');
style.textContent = `
  .animate-on-scroll {
    opacity: 0;
    transform: translateY(30px);
    transition: all 0.5s ease-out;
  }
  
  .animate-reveal {
    opacity: 1;
    transform: translateY(0);
  }
  
  .img-blur-in {
    filter: blur(5px);
    transition: filter 0.5s ease-out;
  }
  
  .img-blur-in.loaded {
    filter: blur(0);
  }
`;
document.head.appendChild(style);