/**
 * Medical Connect - Main JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
  // تهيئة التوست (إشعارات)
  initializeToasts();
  
  // معالجة تسجيل الخروج
  setupLogout();
  
  // تهيئة بيانات الأطباء المميزين للصفحة الرئيسية
  if (document.getElementById('featuredDoctors')) {
    loadFeaturedDoctors();
  }
  
  // تهيئة التوصيات للصفحة الرئيسية
  if (document.getElementById('testimonialsContainer')) {
    loadTestimonials();
  }
  
  // تغيير المظهر عند التمرير
  handleNavbarScroll();
});

/**
 * إعداد وظيفة التوست (الإشعارات)
 */
function initializeToasts() {
  // تعريف كائن عام للتوست
  window.medicalConnect = window.medicalConnect || {};
  
  // إضافة وظيفة إظهار التوست
  window.medicalConnect.showToast = function(title, message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) return;
    
    // إنشاء معرف فريد للتوست
    const toastId = `toast-${Date.now()}`;
    
    // تحديد أيقونة وخلفية التوست بناءً على النوع
    let iconClass = 'fa-info-circle';
    let toastClass = 'toast-info';
    
    if (type === 'success') {
      iconClass = 'fa-check-circle';
      toastClass = 'toast-success';
    } else if (type === 'error') {
      iconClass = 'fa-exclamation-circle';
      toastClass = 'toast-error';
    }
    
    // إنشاء HTML للتوست
    const toastHTML = `
      <div id="${toastId}" class="toast ${toastClass}" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="toast-header">
          <i class="fas ${iconClass} me-2"></i>
          <strong class="me-auto">${title}</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
          ${message}
        </div>
      </div>
    `;
    
    // إضافة التوست إلى الحاوية
    toastContainer.insertAdjacentHTML('beforeend', toastHTML);
    
    // تهيئة وعرض التوست
    const toast = new bootstrap.Toast(document.getElementById(toastId), { delay: 5000 });
    toast.show();
    
    // إزالة التوست بعد الإخفاء
    document.getElementById(toastId).addEventListener('hidden.bs.toast', function() {
      this.remove();
    });
  };
}

/**
 * إعداد وظيفة تسجيل الخروج
 */
function setupLogout() {
  const logoutBtn = document.getElementById('logoutBtn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', function(e) {
      e.preventDefault();
      
      // إزالة بيانات المستخدم من التخزين المحلي
      localStorage.removeItem('userToken');
      localStorage.removeItem('userType');
      localStorage.removeItem('userName');
      
      // عرض إشعار نجاح تسجيل الخروج
      window.medicalConnect.showToast('تم تسجيل الخروج بنجاح', 'جاري إعادة توجيهك...', 'success');
      
      // توجيه المستخدم إلى الصفحة الرئيسية بعد 1.5 ثانية
      setTimeout(() => {
        window.location.href = 'index.html';
      }, 1500);
    });
  }
}

/**
 * تحميل بيانات الأطباء المميزين في الصفحة الرئيسية
 */
function loadFeaturedDoctors() {
  // بيانات الأطباء المميزين
  const doctors = [
    {
      id: 1,
      name: "د. عبدالرحمن الأحمد",
      specialty: "طب القلب",
      rating: 4.9,
      reviews: 124,
      location: "مستشفى الملك فهد - الرياض",
      image: "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true
    },
    {
      id: 2,
      name: "د. سارة العمري",
      specialty: "أمراض النساء والتوليد",
      rating: 4.8,
      reviews: 98,
      location: "مركز الرعاية الطبية - جدة",
      image: "https://images.unsplash.com/photo-1594824476967-48c8b964273f?ixlib=rb-4.0.3&auto=format&fit=crop&w=987&q=80",
      available: true
    },
    {
      id: 3,
      name: "د. محمد السعيد",
      specialty: "طب الأطفال",
      rating: 4.7,
      reviews: 87,
      location: "مستشفى المملكة - الرياض",
      image: "https://images.unsplash.com/photo-1622253692010-333f2da6031d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1964&q=80",
      available: false
    },
    {
      id: 4,
      name: "د. نورة الغامدي",
      specialty: "طب الأعصاب",
      rating: 4.9,
      reviews: 132,
      location: "المركز الطبي الدولي - الدمام",
      image: "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true
    }
  ];
  
  // الحصول على حاوية الأطباء
  const featuredDoctorsContainer = document.getElementById('featuredDoctors');
  if (!featuredDoctorsContainer) return;
  
  // إنشاء HTML لكل طبيب وإضافته للحاوية
  doctors.forEach(doctor => {
    const doctorCard = `
      <div class="col-md-6 col-lg-3">
        <div class="card h-100 border-0 shadow-sm hover-card">
          <div class="position-relative">
            <img src="${doctor.image}" alt="${doctor.name}" class="card-img-top img-blur-in" style="height: 180px; object-fit: cover;" onload="this.classList.add('loaded')">
            ${doctor.available ? `
              <div class="position-absolute top-0 end-0 m-2 badge bg-success text-white px-2 py-1 rounded-pill d-flex align-items-center">
                <span class="bg-white rounded-circle d-inline-block me-1" style="width: 6px; height: 6px;"></span>
                متاح اليوم
              </div>
            ` : ''}
          </div>
          <div class="card-body p-3 text-end">
            <div class="d-flex justify-content-between align-items-start mb-2">
              <div class="d-flex align-items-center">
                <i class="fas fa-star text-warning me-1"></i>
                <span class="fw-medium">${doctor.rating}</span>
                <span class="text-muted small ms-1">(${doctor.reviews})</span>
              </div>
              <h5 class="card-title mb-0 h6 fw-bold">${doctor.name}</h5>
            </div>
            <p class="card-text text-medical-600 small fw-medium mb-2">${doctor.specialty}</p>
            <div class="d-flex align-items-center text-muted small mb-3">
              <i class="fas fa-map-marker-alt me-1"></i>
              <span>${doctor.location}</span>
            </div>
            <div class="d-flex justify-content-between border-top pt-3">
              <a href="doctor-details.html?id=${doctor.id}" class="text-medical-600 small fw-medium">
                عرض الملف الكامل
                <i class="fas fa-arrow-left ms-1"></i>
              </a>
              <a href="book-appointment.html?id=${doctor.id}" class="btn btn-sm btn-primary rounded-pill px-3">
                <i class="fas fa-calendar-check me-1"></i>
                حجز موعد
              </a>
            </div>
          </div>
        </div>
      </div>
    `;
    
    featuredDoctorsContainer.insertAdjacentHTML('beforeend', doctorCard);
  });
}

/**
 * تحميل التوصيات في الصفحة الرئيسية
 */
function loadTestimonials() {
  // بيانات التوصيات
  const testimonials = [
    {
      id: 1,
      name: "أحمد السعيد",
      role: "مريض",
      content: "تجربة رائعة مع منصة ميديكال كونكت! استطعت حجز موعد مع طبيب متخصص في وقت قياسي، وكانت المتابعة بعد الزيارة ممتازة. أنصح بها بشدة لكل من يبحث عن خدمة طبية متميزة.",
      rating: 5,
      image: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=987&q=80"
    },
    {
      id: 2,
      name: "منى العتيبي",
      role: "والدة مريض",
      content: "سهلت علي المنصة الكثير من الوقت والجهد في البحث عن طبيب أطفال متخصص لابني. الواجهة سهلة الاستخدام والدعم الفني سريع الاستجابة. شكراً لكم!",
      rating: 4,
      image: "https://images.unsplash.com/photo-1544005313-94ddf0286df2?ixlib=rb-4.0.3&auto=format&fit=crop&w=988&q=80"
    },
    {
      id: 3,
      name: "د. محمد الشهري",
      role: "طبيب باطنية",
      content: "كطبيب، أقدر كثيراً النظام المتطور لإدارة المواعيد وملفات المرضى. ساعدني ذلك على تنظيم عملي بشكل أفضل وتوفير وقت أكبر للاهتمام بالمرضى. منصة احترافية بكل المقاييس.",
      rating: 5,
      image: "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80"
    }
  ];
  
  // الحصول على حاوية التوصيات
  const testimonialsContainer = document.getElementById('testimonialsContainer');
  const testimonialDots = document.getElementById('testimonialDots');
  if (!testimonialsContainer || !testimonialDots) return;
  
  // إنشاء المؤشرات لكل توصية
  testimonials.forEach((testimonial, index) => {
    const dot = document.createElement('button');
    dot.className = `testimonial-dot rounded-pill mx-1 border-0 ${index === 0 ? 'active' : ''}`;
    dot.style.width = index === 0 ? '24px' : '10px';
    dot.style.height = '10px';
    dot.style.backgroundColor = index === 0 ? 'var(--medical-600)' : '#ddd';
    dot.dataset.index = index;
    
    dot.addEventListener('click', () => {
      showTestimonial(index);
    });
    
    testimonialDots.appendChild(dot);
  });
  
  // عرض التوصية الأولى
  showTestimonial(0);
  
  /**
   * عرض توصية معينة
   */
  function showTestimonial(index) {
    // تحديث المؤشرات
    const dots = testimonialDots.querySelectorAll('.testimonial-dot');
    dots.forEach((dot, i) => {
      dot.classList.toggle('active', i === index);
      dot.style.width = i === index ? '24px' : '10px';
      dot.style.backgroundColor = i === index ? 'var(--medical-600)' : '#ddd';
    });
    
    const testimonial = testimonials[index];
    
    // إنشاء HTML للتوصية
    const stars = Array(5).fill(0).map((_, i) => 
      `<i class="fas fa-star ${i < testimonial.rating ? 'text-warning' : 'text-muted'} me-1"></i>`
    ).join('');
    
    const testimonialHTML = `
      <div class="d-flex flex-column flex-md-row gap-4 align-items-center">
        <div class="rounded-circle border-4 border-white shadow-sm overflow-hidden" style="width: 120px; height: 120px;">
          <img src="${testimonial.image}" alt="${testimonial.name}" class="w-100 h-100 object-fit-cover img-blur-in" onload="this.classList.add('loaded')">
        </div>
        
        <div class="flex-1 text-center text-md-end">
          <div class="d-flex justify-content-center justify-content-md-end mb-3">
            ${stars}
          </div>
          
          <blockquote class="fs-5 text-muted mb-4 fst-italic">
            "${testimonial.content}"
          </blockquote>
          
          <div>
            <p class="fw-bold mb-0">${testimonial.name}</p>
            <p class="text-medical-600">${testimonial.role}</p>
          </div>
        </div>
      </div>
      
      <div class="position-absolute bottom-0 end-0 p-3 d-flex gap-2">
        <button class="testimonial-arrow prev rounded-circle bg-white shadow-sm border-0 d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
          <i class="fas fa-chevron-right"></i>
        </button>
        <button class="testimonial-arrow next rounded-circle bg-white shadow-sm border-0 d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
          <i class="fas fa-chevron-left"></i>
        </button>
      </div>
    `;
    
    testimonialsContainer.innerHTML = testimonialHTML;
    
    // إضافة معالجات الأحداث للأزرار
    testimonialsContainer.querySelector('.testimonial-arrow.prev').addEventListener('click', () => {
      const prevIndex = (index - 1 + testimonials.length) % testimonials.length;
      showTestimonial(prevIndex);
    });
    
    testimonialsContainer.querySelector('.testimonial-arrow.next').addEventListener('click', () => {
      const nextIndex = (index + 1) % testimonials.length;
      showTestimonial(nextIndex);
    });
  }
}

/**
 * معالجة تغيير مظهر شريط التنقل عند التمرير
 */
function handleNavbarScroll() {
  const navbar = document.querySelector('.navbar');
  if (!navbar) return;
  
  window.addEventListener('scroll', () => {
    if (window.scrollY > 20) {
      navbar.classList.add('bg-white', 'shadow-sm');
    } else {
      // إذا كنا في الصفحة الرئيسية، نزيل الخلفية عند العودة للأعلى
      if (window.location.pathname === '/' || window.location.pathname === '/index.html') {
        navbar.classList.remove('bg-white', 'shadow-sm');
      }
    }
  });
}