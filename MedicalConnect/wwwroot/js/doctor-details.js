/**
 * Medical Connect - Doctor Details JavaScript
 */

document.addEventListener('DOMContentLoaded', function () {
  // الحصول على معرف الطبيب من الرابط
  const urlParams = new URLSearchParams(window.location.search);
  const doctorId = urlParams.get('id') || 1; // استخدام الطبيب الأول كافتراضي

  // تحميل بيانات الطبيب
  loadDoctorDetails(doctorId);

  // تحميل تقييمات الطبيب
  loadDoctorReviews(doctorId);
});

/**
 * تحميل بيانات الطبيب
 */
function loadDoctorDetails(doctorId) {
  // بيانات الأطباء
  const doctors = [
    {
      id: 1,
      name: "د. عبدالرحمن الأحمد",
      specialty: "طب القلب",
      rating: 4.9,
      reviews: 124,
      location: "عيادة عبدالرحمن - خانيونس",
      image: "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1470&q=80",
      available: true,
      experience: 15,
      consultationFee: 30,
      waitTime: "15-20 دقيقة",
      bio: "طبيب قلب استشاري مع أكثر من 15 عامًا من الخبرة في تشخيص وعلاج أمراض القلب والأوعية الدموية. متخصص في قسطرة القلب والتصوير بالموجات فوق الصوتية. حاصل على شهادة الزمالة في أمراض القلب من الكلية الملكية للأطباء في بريطانيا. يؤمن بأهمية الرعاية الطبية الشاملة والوقاية من أمراض القلب لتحسين نوعية حياة المرضى.",
      specialties: [
        "تشخيص وعلاج أمراض القلب",
        "قسطرة القلب التشخيصية والعلاجية",
        "تصوير القلب بالموجات فوق الصوتية",
        "علاج ارتفاع ضغط الدم"
      ],
      languages: ["العربية", "الإنجليزية"],
      education: [
        {
          degree: "زمالة أمراض القلب",
          institution: "جامعة الأزهر - غزة",
          period: "2014 - 2022"
        },
        {
          degree: "البورد العربي في الباطنة",
          institution: "المجلس العربي للاختصاصات الصحية",
          period: "2008 - 2011"
        },
        {
          degree: "بكالوريوس الطب والجراحة",
          institution: "الجامعة التطبيقية - غزة",
          period: "2001 - 2007"
        }
      ]
    }
  ];

  // البحث عن الطبيب بالمعرف
  const doctor = doctors.find(d => d.id === parseInt(doctorId)) || doctors[0];

  // تعبئة بيانات الطبيب في الصفحة
  // معلومات رأس الصفحة
  document.getElementById('doctorNameBreadcrumb').textContent = doctor.name;
  document.title = `Medical Connect - ${doctor.name}`;

  // معلومات بطاقة الطبيب
  document.getElementById('doctorName').textContent = doctor.name;
  document.getElementById('doctorSpecialty').textContent = doctor.specialty;
  document.getElementById('doctorRating').textContent = doctor.rating;
  document.getElementById('doctorReviews').textContent = doctor.reviews;
  document.getElementById('doctorLocation').textContent = doctor.location;
  document.getElementById('doctorExperience').textContent = `${doctor.experience} سنة`;
  document.getElementById('doctorFee').textContent = `${doctor.consultationFee} شيكل`;
  document.getElementById('doctorWaitTime').textContent = doctor.waitTime;

  // صورة الطبيب
  document.getElementById('doctorImage').src = doctor.image;
  document.getElementById('doctorImage').alt = doctor.name;

  // نجوم التقييم
  const ratingStars = document.getElementById('doctorRatingStars');
  if (ratingStars) {
    let starsHTML = '';
    const fullStars = Math.floor(doctor.rating);
    const hasHalfStar = doctor.rating % 1 >= 0.5;

    for (let i = 0; i < 5; i++) {
      if (i < fullStars) {
        starsHTML += '<i class="fas fa-star text-warning"></i>';
      } else if (i === fullStars && hasHalfStar) {
        starsHTML += '<i class="fas fa-star-half-alt text-warning"></i>';
      } else {
        starsHTML += '<i class="far fa-star text-warning"></i>';
      }
    }

    ratingStars.innerHTML = starsHTML;
  }

  // الأوقات المتاحة
  if (doctor.available) {
    document.getElementById('doctorAvailabilitySection').classList.remove('d-none');
  } else {
    document.getElementById('doctorAvailabilitySection').classList.add('d-none');
  }

  // المعلومات التفصيلية
  document.getElementById('doctorBio').textContent = doctor.bio;

  // التخصصات
  const specialtiesList = document.getElementById('doctorSpecialties');
  if (specialtiesList) {
    let specialtiesHTML = '';

    doctor.specialties.forEach(specialty => {
      specialtiesHTML += `
        <li class="list-group-item ps-0 border-0 d-flex align-items-center">
          <i class="fas fa-check-circle text-success me-2"></i>
          ${specialty}
        </li>
      `;
    });

    specialtiesList.innerHTML = specialtiesHTML;
  }

  // اللغات
  const languagesList = document.getElementById('doctorLanguages');
  if (languagesList) {
    let languagesHTML = '';

    doctor.languages.forEach(language => {
      languagesHTML += `
        <li class="list-group-item ps-0 border-0 d-flex align-items-center">
          <i class="fas fa-check-circle text-success me-2"></i>
          ${language}
        </li>
      `;
    });

    languagesList.innerHTML = languagesHTML;
  }

  // التعليم والشهادات
  const educationList = document.getElementById('doctorEducation');
  if (educationList) {
    let educationHTML = '';

    doctor.education.forEach(edu => {
      educationHTML += `
        <div class="timeline-item mb-4">
          <div class="d-flex">
            <div class="timeline-marker me-3">
              <div class="rounded-circle bg-medical-50 d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                <i class="fas fa-graduation-cap text-medical-600"></i>
              </div>
            </div>
            <div>
              <h4 class="fs-6 fw-semibold mb-1">${edu.degree}</h4>
              <p class="text-muted mb-1">${edu.institution}</p>
              <p class="small text-muted">${edu.period}</p>
            </div>
          </div>
        </div>
      `;
    });

    educationList.innerHTML = educationHTML;
  }

  // تحديث قسم التقييمات
  document.getElementById('reviewsRating').textContent = doctor.rating;
  document.getElementById('reviewsCount').textContent = doctor.reviews;
}

/**
 * تحميل تقييمات الطبيب
 */
function loadDoctorReviews(doctorId) {
  // بيانات التقييمات
  const reviews = [
    {
      id: 1,
      name: "أحمد السعيد",
      date: "12/01/2025",
      rating: 5,
      comment: "دكتور ممتاز وذو خبرة عالية. شرح لي حالتي بالتفصيل وأعطاني العلاج المناسب. العيادة مريحة ومنظمة، والانتظار كان قصيراً. أنصح به بشدة.",
      avatar: "https://randomuser.me/api/portraits/men/32.jpg"
    },
    {
      id: 2,
      name: "منى الحربي",
      date: "05/02/2025",
      rating: 4,
      comment: "تجربة إيجابية مع الدكتور. متمكن ولديه خبرة واضحة في مجاله. الموعد كان متأخراً قليلاً عن الوقت المحدد لكن الخدمة كانت ممتازة بشكل عام.",
      avatar: "https://randomuser.me/api/portraits/women/44.jpg"
    },
    {
      id: 3,
      name: "محمد العتيبي",
      date: "22/03/2025",
      rating: 5,
      comment: "أفضل طبيب قلب زرته. متابعتي معه لأكثر من سنة، ودائماً متعاون ويشرح الحالة بوضوح. أوصي به بشدة.",
      avatar: "https://randomuser.me/api/portraits/men/11.jpg"
    }
  ];

  const reviewsList = document.getElementById('reviewsList');
  if (!reviewsList) return;

  let reviewsHTML = '';

  // عرض التقييمات الثلاثة الأولى فقط
  const visibleReviews = reviews.slice(0, 3);

  visibleReviews.forEach(review => {
    // إنشاء نجوم التقييم
    let starsHTML = '';
    for (let i = 0; i < 5; i++) {
      if (i < review.rating) {
        starsHTML += '<i class="fas fa-star text-warning"></i>';
      } else {
        starsHTML += '<i class="far fa-star text-warning"></i>';
      }
    }

    reviewsHTML += `
      <div class="review-item mb-4 pb-4 border-bottom">
        <div class="d-flex">
          <div class="me-3">
            <img src="${review.avatar}" alt="${review.name}" class="rounded-circle" width="50" height="50">
          </div>
          <div class="flex-grow-1">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h6 class="fw-semibold mb-0">${review.name}</h6>
              <span class="text-muted small">${review.date}</span>
            </div>
            <div class="mb-2">
              ${starsHTML}
            </div>
            <p class="text-muted mb-0">${review.comment}</p>
          </div>
        </div>
      </div>
    `;
  });

  reviewsList.innerHTML = reviewsHTML;

  // معالجة زر "عرض المزيد"
  const loadMoreBtn = document.getElementById('loadMoreReviews');
  if (loadMoreBtn) {
    loadMoreBtn.addEventListener('click', function () {
      // في هذا المثال يتم عرض رسالة فقط
      window.medicalConnect.showToast('جميع التقييمات', 'تم عرض جميع التقييمات المتاحة بالفعل', 'info');
    });
  }
}