// ======= Main JavaScript Logic for All Pages =======

document.addEventListener('DOMContentLoaded', function () {
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                window.scrollTo({
                    top: target.offsetTop - 100,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Add image lazy loading with blur effect
    function initLazyImages() {
        const lazyImages = document.querySelectorAll('.img-blur-in:not(.loaded)');
        lazyImages.forEach(img => {
            if (img.complete) {
                img.classList.add('loaded');
            } else {
                img.addEventListener('load', () => {
                    img.classList.add('loaded');
                });
            }
        });
    }

    // Scroll animations
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

    // Navigation menu active state
    function setActiveNav() {
        const sections = document.querySelectorAll('section[id]');
        const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

        window.addEventListener('scroll', () => {
            let current = '';

            sections.forEach(section => {
                const sectionTop = section.offsetTop;
                const sectionHeight = section.clientHeight;

                if (window.scrollY >= (sectionTop - 200)) {
                    current = section.getAttribute('id');
                }
            });

            navLinks.forEach(link => {
                link.classList.remove('active');
                if (link.getAttribute('href') && link.getAttribute('href').includes(current)) {
                    link.classList.add('active');
                }
            });
        });
    }

    // Navbar scroll behavior
    function handleNavbarScroll() {
        const navbar = document.querySelector('.navbar');

        window.addEventListener('scroll', () => {
            if (window.scrollY > 50) {
                navbar.classList.add('bg-white', 'shadow-sm');
                navbar.classList.remove('bg-transparent');
            } else {
                if (window.location.pathname.endsWith('index.html') || window.location.pathname === '/' || window.location.pathname.endsWith('/')) {
                    navbar.classList.remove('bg-white', 'shadow-sm');
                    navbar.classList.add('bg-transparent');
                }
            }
        });
    }

    // Initialize functions
    initLazyImages();
    observeElements();
    setActiveNav();
    handleNavbarScroll();
});

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

// Simple toast notification function (can be used across all pages)
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