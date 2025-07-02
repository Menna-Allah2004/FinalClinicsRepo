/**
 * Medical Connect - Book Appointment JavaScript
 */

document.addEventListener('DOMContentLoaded', function () {
  // الحصول على معرف الطبيب من الرابط
  const urlParams = new URLSearchParams(window.location.search);
  const doctorId = urlParams.get('id') || 1; // استخدام الطبيب الأول كافتراضي

  // تحميل بيانات الطبيب
  loadDoctorInfo(doctorId);

  // تهيئة التقويم
  initializeCalendar();

  // تهيئة خطوات الحجز
  setupBookingSteps();
});

/**
 * تحميل معلومات الطبيب
 */
function loadDoctorInfo(doctorId) {
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
      sessionDuration: "20-30 دقيقة"
    }
  ];

  // البحث عن الطبيب بالمعرف
  const doctor = doctors.find(d => d.id === parseInt(doctorId)) || doctors[0];

  // تعبئة بيانات الطبيب في الصفحة
  document.getElementById('doctor-name').textContent = doctor.name;
  document.getElementById('doctor-specialty').textContent = doctor.specialty;
  document.getElementById('doctor-rating').textContent = doctor.rating;
  document.getElementById('doctor-reviews').textContent = doctor.reviews;
  document.getElementById('doctor-location').textContent = doctor.location;
  document.getElementById('doctor-fee').textContent = `${doctor.consultationFee} شيكل`;
  document.getElementById('doctor-session-duration').textContent = doctor.sessionDuration;
  document.getElementById('doctor-image').src = doctor.image;
  document.getElementById('doctor-image').alt = doctor.name;

  // تخزين بيانات الطبيب للاستخدام في خطوات لاحقة
  window.selectedDoctor = doctor;

  // تحديث قسم التأكيد بمعلومات الطبيب
  document.getElementById('confirm-doctor').textContent = doctor.name;
  document.getElementById('confirm-specialty').textContent = doctor.specialty;
  document.getElementById('confirm-fee').textContent = `${doctor.consultationFee} شيكل`;
}

/**
 * تهيئة التقويم
 */
function initializeCalendar() {
  const calendarEl = document.getElementById('appointment-calendar');
  if (!calendarEl) return;

  // تهيئة Flatpickr مع دعم اللغة العربية
  flatpickr(calendarEl, {
    inline: true,
    locale: 'ar',
    dateFormat: 'Y-m-d',
    defaultDate: new Date(),
    minDate: 'today',
    showMonths: 1,
    disableMobile: true,
    monthSelectorType: 'static',
    onChange: function (selectedDates, dateStr, instance) {
      console.log('التاريخ المحدد:', dateStr);

      // حفظ التاريخ المحدد
      window.selectedDate = dateStr;

      // تفعيل أزرار الأوقات
      enableTimeButtons();

      // إلغاء تفعيل زر الخطوة التالية حتى يتم اختيار وقت
      document.getElementById('goto-step-2').disabled = true;
    }
  });

  // إعداد أزرار اختيار الوقت
  setupTimeButtons();
}

/**
 * إعداد أزرار اختيار الوقت
 */
function setupTimeButtons() {
  // أوقات الصباح
  const morningTimes = document.getElementById('morning-times');

  if (morningTimes) {
    const timeButtons = morningTimes.querySelectorAll('button');

    timeButtons.forEach(button => {
      button.addEventListener('click', function () {
        // إزالة الفئة النشطة من جميع الأزرار
        document.querySelectorAll('.appointment-times button').forEach(btn => {
          btn.classList.remove('btn-primary');
          btn.classList.add('btn-outline-primary');
        });

        // إضافة الفئة النشطة للزر المحدد
        this.classList.remove('btn-outline-primary');
        this.classList.add('btn-primary');

        // حفظ الوقت المحدد
        window.selectedTime = this.dataset.time;

        // تفعيل زر الخطوة التالية
        document.getElementById('goto-step-2').disabled = false;
      });
    });
  }

  // أوقات المساء
  const afternoonTimes = document.getElementById('afternoon-times');

  if (afternoonTimes) {
    const timeButtons = afternoonTimes.querySelectorAll('button');

    timeButtons.forEach(button => {
      button.addEventListener('click', function () {
        // إزالة الفئة النشطة من جميع الأزرار
        document.querySelectorAll('.appointment-times button').forEach(btn => {
          btn.classList.remove('btn-primary');
          btn.classList.add('btn-outline-primary');
        });

        // إضافة الفئة النشطة للزر المحدد
        this.classList.remove('btn-outline-primary');
        this.classList.add('btn-primary');

        // حفظ الوقت المحدد
        window.selectedTime = this.dataset.time;

        // تفعيل زر الخطوة التالية
        document.getElementById('goto-step-2').disabled = false;
      });
    });
  }
}

/**
 * تفعيل أزرار الأوقات
 */
function enableTimeButtons() {
  // هنا يمكن إضافة منطق لتفعيل/تعطيل أوقات معينة حسب التاريخ
  document.querySelectorAll('.appointment-times button').forEach(btn => {
    btn.disabled = false;
  });
}

/**
 * إعداد خطوات الحجز
 */
function setupBookingSteps() {
  // معرفات الأزرار
  const gotoStep2Btn = document.getElementById('goto-step-2');
  const backToStep1Btn = document.getElementById('back-to-step-1');
  const gotoStep3Btn = document.getElementById('goto-step-3');
  const backToStep2Btn = document.getElementById('back-to-step-2');
  const confirmBookingBtn = document.getElementById('confirm-booking');

  // معرفات محتوى الخطوات
  const step1Content = document.getElementById('booking-step-1');
  const step2Content = document.getElementById('booking-step-2');
  const step3Content = document.getElementById('booking-step-3');
  const step4Content = document.getElementById('booking-step-4');

  // معرفات مؤشرات الخطوات
  const bookingSteps = document.querySelectorAll('.booking-step');

  // الانتقال من الخطوة 1 إلى الخطوة 2
  if (gotoStep2Btn) {
    gotoStep2Btn.addEventListener('click', function () {
      step1Content.classList.add('d-none');
      step2Content.classList.remove('d-none');

      // تحديث مؤشرات الخطوات
      bookingSteps[0].classList.remove('active');
      bookingSteps[1].classList.add('active');
    });
  }

  // الرجوع من الخطوة 2 إلى الخطوة 1
  if (backToStep1Btn) {
    backToStep1Btn.addEventListener('click', function () {
      step2Content.classList.add('d-none');
      step1Content.classList.remove('d-none');

      // تحديث مؤشرات الخطوات
      bookingSteps[1].classList.remove('active');
      bookingSteps[0].classList.add('active');
    });
  }

  // الانتقال من الخطوة 2 إلى الخطوة 3
  if (gotoStep3Btn) {
    gotoStep3Btn.addEventListener('click', function () {
      // التحقق من صحة النموذج
      const form = document.getElementById('booking-details-form');
      if (form.checkValidity()) {
        // تحديث معلومات التأكيد
        updateConfirmationInfo();

        step2Content.classList.add('d-none');
        step3Content.classList.remove('d-none');

        // تحديث مؤشرات الخطوات
        bookingSteps[1].classList.remove('active');
        bookingSteps[2].classList.add('active');
      } else {
        // تفعيل التحقق من صحة النموذج
        form.classList.add('was-validated');
      }
    });
  }

  // الرجوع من الخطوة 3 إلى الخطوة 2
  if (backToStep2Btn) {
    backToStep2Btn.addEventListener('click', function () {
      step3Content.classList.add('d-none');
      step2Content.classList.remove('d-none');

      // تحديث مؤشرات الخطوات
      bookingSteps[2].classList.remove('active');
      bookingSteps[1].classList.add('active');
    });
  }

  // تأكيد الحجز والانتقال إلى الخطوة 4
  if (confirmBookingBtn) {
    confirmBookingBtn.addEventListener('click', function () {
      // التحقق من الموافقة على الشروط
      const termsCheck = document.getElementById('termsCheck');

      if (!termsCheck.checked) {
        window.medicalConnect.showToast('تنبيه', 'يجب الموافقة على الشروط والأحكام', 'error');
        return;
      }

      // إظهار حالة التحميل
      confirmBookingBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> جاري التأكيد...';
      confirmBookingBtn.disabled = true;

      // محاكاة طلب API
      setTimeout(function () {
        step3Content.classList.add('d-none');
        step4Content.classList.remove('d-none');

        // تحديث معلومات التأكيد النهائي
        document.getElementById('booking-number').textContent = 'BK-' + Math.floor(10000 + Math.random() * 90000);
        document.getElementById('booking-date').textContent = formatDate(window.selectedDate);
        document.getElementById('booking-time').textContent = formatTime(window.selectedTime);
        document.getElementById('booking-type').textContent = document.querySelector('input[name="appointmentType"]:checked').value === 'online' ? 'استشارة عن بعد' : 'زيارة في العيادة';
      }, 1500);
    });
  }
}

/**
 * تحديث معلومات التأكيد
 */
function updateConfirmationInfo() {
  if (!window.selectedDate || !window.selectedTime || !window.selectedDoctor) return;

  // تحديث التاريخ والوقت
  document.getElementById('confirm-date').textContent = formatDate(window.selectedDate);
  document.getElementById('confirm-time').textContent = formatTime(window.selectedTime);

  // تحديث نوع الموعد
  const appointmentType = document.querySelector('input[name="appointmentType"]:checked').value;
  document.getElementById('confirm-type').textContent = appointmentType === 'online' ? 'استشارة عن بعد' : 'زيارة في العيادة';

  // تحديث سبب الزيارة
  const reasonSelect = document.getElementById('appointmentReason');
  const reasonValue = reasonSelect.value;
  const reasonText = reasonSelect.options[reasonSelect.selectedIndex].text;
  document.getElementById('confirm-reason').textContent = reasonText;
}

/**
 * تنسيق التاريخ
 */
function formatDate(dateStr) {
  if (!dateStr) return '';

  // تحويل التاريخ إلى صيغة عربية
  const date = new Date(dateStr);
  const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };

  return date.toLocaleDateString('ar-SA', options);
}

/**
 * تنسيق الوقت
 */
function formatTime(timeStr) {
  if (!timeStr) return '';

  // تحويل الوقت إلى صيغة مناسبة
  const [hours, minutes] = timeStr.split(':');
  const hour = parseInt(hours);

  if (hour < 12) {
    return `${timeStr} صباحاً`;
  } else if (hour === 12) {
    return `${timeStr} ظهراً`;
  } else {
    return `${timeStr} مساءً`;
  }
}