/**
 * Medical Connect - Doctors Page JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
  // تحميل قائمة الأطباء
  loadDoctors();
  
  // تهيئة المرشحات
  initializeFilters();
});

/**
 * تهيئة مرشحات البحث
 */
function initializeFilters() {
  // زر تبديل عرض المرشحات المتقدمة
  const toggleAdvancedFilters = document.getElementById('toggleAdvancedFilters');
  const advancedFilters = document.getElementById('advancedFilters');
  
  if (toggleAdvancedFilters && advancedFilters) {
    toggleAdvancedFilters.addEventListener('click', function() {
      advancedFilters.classList.toggle('d-none');
    });
  }
  
  // حدث البحث
  const searchInput = document.getElementById('searchInput');
  const specialtyFilter = document.getElementById('specialtyFilter');
  const locationFilter = document.getElementById('locationFilter');
  const ratingFilter = document.getElementById('ratingFilter');
  const priceFilter = document.getElementById('priceFilter');
  const availableTodayFilter = document.getElementById('availableTodayFilter');
  const resetFilters = document.getElementById('resetFilters');
  
  // إضافة أحداث للمرشحات
  [searchInput, specialtyFilter, locationFilter, ratingFilter, priceFilter, availableTodayFilter].forEach(filter => {
    if (filter) {
      filter.addEventListener('change', filterDoctors);
      if (filter.type === 'text') {
        filter.addEventListener('keyup', filterDoctors);
      }
    }
  });
  
  // زر إعادة ضبط المرشحات
  if (resetFilters) {
    resetFilters.addEventListener('click', function() {
      if (searchInput) searchInput.value = '';
      if (specialtyFilter) specialtyFilter.value = 'جميع التخصصات';
      if (locationFilter) locationFilter.value = 'جميع المدن';
      if (ratingFilter) ratingFilter.value = '';
      if (priceFilter) priceFilter.value = '';
      if (availableTodayFilter) availableTodayFilter.checked = false;
      
      filterDoctors();
    });
  }
  
  // مرشح الترتيب
  const sortSelect = document.getElementById('sortSelect');
  if (sortSelect) {
    sortSelect.addEventListener('change', function() {
      filterDoctors();
    });
  }
}

/**
 * تحميل بيانات الأطباء
 */
function loadDoctors() {
  // بيانات الأطباء
  const doctors = [
    {
      id: 1,
      name: "د. عبدالرحمن الأحمد",
      specialty: "طب القلب",
      rating: 4.9,
      reviews: 124,
      location: "عيادة عبدالرحمن-خانيونس",
      image: "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true,
      experience: 15,
      consultationFee: 30,
      waitTime: "15-20 دقيقة"
    },
    {
      id: 2,
      name: "د. سارة العمري",
      specialty: "أمراض النساء والتوليد",
      rating: 4.8,
      reviews: 98,
      location: "مركز الرعاية الطبية - غزة",
      image: "https://images.unsplash.com/photo-1594824476967-48c8b964273f?ixlib=rb-4.0.3&auto=format&fit=crop&w=987&q=80",
      available: true,
      experience: 10,
      consultationFee: 25,
      waitTime: "10-15 دقيقة"
    },
    {
      id: 3,
      name: "د. محمد السعيد",
      specialty: "طب الأطفال",
      rating: 4.7,
      reviews: 87,
      location: "مستشفى الشفاء - غزة",
      image: "https://images.unsplash.com/photo-1622253692010-333f2da6031d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1964&q=80",
      available: false,
      experience: 12,
      consultationFee: 28,
      waitTime: "20-25 دقيقة"
    },
    {
      id: 4,
      name: "د. نورة الغامدي",
      specialty: "طب الأعصاب",
      rating: 4.9,
      reviews: 132,
      location: "المركز الطبي  - البريج",
      image: "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true,
      experience: 18,
      consultationFee: 35,
      waitTime: "15-20 دقيقة"
    },
    {
      id: 5,
      name: "د. أحمد الزهراني",
      specialty: "جراحة العظام",
      rating: 4.8,
      reviews: 76,
      location: "مستشفى السلام - غزة",
      image: "https://images.unsplash.com/photo-1537368910025-700350fe46c7?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true,
      experience: 14,
      consultationFee: 32,
      waitTime: "20-30 دقيقة"
    },
    {
      id: 6,
      name: "د. هدى السليم",
      specialty: "طب العيون",
      rating: 4.6,
      reviews: 94,
      location: "مركز البصر الطبي - النصيرات",
      image: "https://images.unsplash.com/photo-1527613426441-4da17471b66d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: false,
      experience: 9,
      consultationFee: 23,
      waitTime: "10-15 دقيقة"
    }
  ];
  
  // تخزين الأطباء في كائن النافذة للاستخدام في الترشيح
  window.medicalConnect = window.medicalConnect || {};
  window.medicalConnect.doctors = doctors;
  
  // عرض الأطباء
  filterDoctors();
}

/**
 * ترشيح وعرض الأطباء حسب المعايير
 */
function filterDoctors() {
  if (!window.medicalConnect || !window.medicalConnect.doctors) return;
  
  const doctors = window.medicalConnect.doctors;
  
  // الحصول على قيم المرشحات
  const searchInput = document.getElementById('searchInput')?.value?.trim()?.toLowerCase() || '';
  const specialtyFilter = document.getElementById('specialtyFilter')?.value || 'جميع التخصصات';
  const locationFilter = document.getElementById('locationFilter')?.value || 'جميع المدن';
  const ratingFilter = document.getElementById('ratingFilter')?.value || '';
  const priceFilter = document.getElementById('priceFilter')?.value || '';
  const availableTodayFilter = document.getElementById('availableTodayFilter')?.checked || false;
  
  // ترشيح الأطباء
  let filteredDoctors = doctors.filter(doctor => {
    // البحث بالنص
    const matchesSearch = searchInput === '' || 
                          doctor.name.toLowerCase().includes(searchInput) || 
                          doctor.specialty.toLowerCase().includes(searchInput);
    
    // التخصص
    const matchesSpecialty = specialtyFilter === 'جميع التخصصات' || 
                            doctor.specialty === specialtyFilter;
    
    // الموقع
    const matchesLocation = locationFilter === 'جميع المدن' || 
                           doctor.location.includes(locationFilter);
    
    // التقييم
    const matchesRating = ratingFilter === '' || 
                         doctor.rating >= parseFloat(ratingFilter);
    
    // السعر
    const matchesPrice = priceFilter === '' || 
                        doctor.consultationFee <= parseInt(priceFilter);
    
    // المتاح اليوم
    const matchesAvailability = !availableTodayFilter || 
                               doctor.available;
    
    return matchesSearch && matchesSpecialty && matchesLocation && 
           matchesRating && matchesPrice && matchesAvailability;
  });
  
  // الترتيب
  const sortSelect = document.getElementById('sortSelect')?.value || 'rating';
  
  switch (sortSelect) {
    case 'rating':
      filteredDoctors.sort((a, b) => b.rating - a.rating);
      break;
    case 'experience':
      filteredDoctors.sort((a, b) => b.experience - a.experience);
      break;
    case 'fee-asc':
      filteredDoctors.sort((a, b) => a.consultationFee - b.consultationFee);
      break;
    case 'fee-desc':
      filteredDoctors.sort((a, b) => b.consultationFee - a.consultationFee);
      break;
  }
  
  // عرض النتائج
  displayDoctors(filteredDoctors);
}

/**
 * عرض الأطباء في الصفحة
 */
function displayDoctors(doctors) {
  const doctorsList = document.getElementById('doctorsList');
  const doctorsCount = document.getElementById('doctorsCount');
  const noResults = document.getElementById('noResults');
  const pagination = document.getElementById('pagination');
  
  if (!doctorsList || !doctorsCount || !noResults || !pagination) return;
  
  // تحديث عدد الأطباء
  doctorsCount.textContent = doctors.length;
  
  // عرض/إخفاء رسالة عدم وجود نتائج
  if (doctors.length === 0) {
    doctorsList.innerHTML = '';
    noResults.classList.remove('d-none');
    pagination.classList.add('d-none');
    return;
  } else {
    noResults.classList.add('d-none');
    pagination.classList.remove('d-none');
  }
  
  // إنشاء HTML لكل طبيب
  let doctorsHTML = '';
  
  doctors.forEach(doctor => {
    doctorsHTML += `
      <div class="col-md-6 col-lg-4">
        <div class="card h-100 border-0 shadow-sm hover-card">
          <div class="position-relative">
            <img src="${doctor.image}" alt="${doctor.name}" class="card-img-top img-blur-in" style="height: 200px; object-fit: cover;" onload="this.classList.add('loaded')">
            ${doctor.available ? `
              <div class="position-absolute top-0 end-0 m-2 badge bg-success text-white px-2 py-1 rounded-pill d-flex align-items-center">
                <span class="bg-white rounded-circle d-inline-block me-1" style="width: 6px; height: 6px;"></span>
                متاح اليوم
              </div>
            ` : ''}
          </div>
          
          <div class="card-body p-3 text-end">
            <h5 class="card-title mb-1 h6 fw-bold">${doctor.name}</h5>
            <p class="card-text text-medical-600 small fw-medium mb-2">${doctor.specialty}</p>
            
            <div class="d-flex align-items-center text-muted small mb-3">
              <i class="fas fa-map-marker-alt me-1"></i>
              <span>${doctor.location}</span>
            </div>
            
            <div class="d-flex justify-content-between align-items-center mb-3">
              <div class="d-flex align-items-center">
                <i class="fas fa-star text-warning me-1"></i>
                <span class="fw-medium">${doctor.rating}</span>
                <span class="text-muted small ms-1">(${doctor.reviews})</span>
              </div>
              <div class="text-muted small">
                <span class="fw-medium">${doctor.experience}</span> سنة خبرة
              </div>
            </div>
            
            <div class="d-flex justify-content-between border-top border-bottom py-3 mb-3">
              <div>
                <p class="text-muted small mb-0">سعر الكشف</p>
                <p class="fw-medium mb-0">${doctor.consultationFee} شيكل</p>
              </div>
              <div>
                <p class="text-muted small mb-0">مدة الانتظار</p>
                <p class="fw-medium mb-0">${doctor.waitTime}</p>
              </div>
            </div>
            
            <div class="d-flex gap-2">
              <a href="book-appointment.html?id=${doctor.id}" class="btn btn-primary flex-grow-1 rounded-3">
                <i class="fas fa-calendar-check me-1"></i>
                حجز موعد
              </a>
              <a href="doctor-details.html?id=${doctor.id}" class="btn btn-light flex-grow-1 rounded-3">
                عرض الملف
              </a>
            </div>
          </div>
        </div>
      </div>
    `;
  });
  
  doctorsList.innerHTML = doctorsHTML;
}