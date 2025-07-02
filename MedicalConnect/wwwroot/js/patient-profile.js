/**
 * Medical Connect - Patient Profile JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
  // تهيئة التقويم
  initializeCalendar();
  
  // تحميل المواعيد
  loadAppointments();
  
  // تحميل السجلات الطبية
  loadMedicalRecords();
  
  // إعداد تحرير الملف الشخصي
  setupProfileEdit();
});

/**
 * تهيئة التقويم
 */
function initializeCalendar() {
  const calendarEl = document.getElementById('medical-calendar');
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
    onChange: function(selectedDates, dateStr, instance) {
      console.log('التاريخ المحدد:', dateStr);
    }
  });
}

/**
 * تحميل بيانات المواعيد
 */
function loadAppointments() {
  // بيانات المواعيد
  const appointments = [
    { 
      id: 1, 
      doctor: "د. علي أحمد", 
      specialty: "طب عام", 
      date: "05/06/2023", 
      time: "10:30 ص", 
      status: "مكتمل" 
    },
    { 
      id: 2, 
      doctor: "د. سارة خالد", 
      specialty: "أمراض جلدية", 
      date: "15/06/2023", 
      time: "12:00 م", 
      status: "ملغي" 
    },
    { 
      id: 3, 
      doctor: "د. محمد عبد الله", 
      specialty: "أمراض قلب", 
      date: "25/06/2023", 
      time: "09:00 ص", 
      status: "قادم" 
    },
  ];
  
  // الحصول على حاوية المواعيد
  const appointmentsList = document.getElementById('appointmentsList');
  if (!appointmentsList) return;
  
  // إنشاء HTML لكل موعد
  let appointmentsHTML = '';
  
  if (appointments.length === 0) {
    appointmentsHTML = `
      <div class="text-center py-5">
        <div class="text-muted mb-3">
          <i class="fas fa-calendar-times fa-3x"></i>
        </div>
        <h5 class="fw-bold mb-2">لا توجد مواعيد</h5>
        <p class="text-muted mb-4">ليس لديك أي مواعيد محجوزة حالياً</p>
        <a href="doctors.html" class="btn btn-primary px-4 py-2 rounded-3">
          حجز موعد جديد
        </a>
      </div>
    `;
  } else {
    appointments.forEach(appointment => {
      let statusClass, statusIcon;
      
      switch (appointment.status) {
        case "مكتمل":
          statusClass = "success";
          statusIcon = "check-circle";
          break;
        case "ملغي":
          statusClass = "danger";
          statusIcon = "times-circle";
          break;
        case "قادم":
          statusClass = "primary";
          statusIcon = "clock";
          break;
        default:
          statusClass = "secondary";
          statusIcon = "circle";
      }
      
      appointmentsHTML += `
        <div class="card mb-3 border-0 shadow-sm hover-card">
          <div class="card-body p-3">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <h5 class="card-title h6 fw-bold mb-1">${appointment.doctor}</h5>
                <p class="text-muted small mb-2">${appointment.specialty}</p>
              </div>
              <span class="badge bg-${statusClass} bg-opacity-10 text-${statusClass} px-3 py-2 rounded-pill">
                <i class="fas fa-${statusIcon} me-1"></i>
                ${appointment.status}
              </span>
            </div>
            
            <div class="d-flex align-items-center text-muted small mb-3">
              <i class="fas fa-calendar-day me-2"></i>
              <span class="me-3">${appointment.date}</span>
              <i class="fas fa-clock me-2"></i>
              <span>${appointment.time}</span>
            </div>
            
            <div class="d-flex ${appointment.status === "قادم" ? "justify-content-between" : "justify-content-end"}">
              ${appointment.status === "قادم" ? `
                <button class="btn btn-sm btn-outline-danger rounded-3">
                  <i class="fas fa-times me-1"></i>
                  إلغاء الموعد
                </button>
              ` : ''}
              <a href="appointment-details.html?id=${appointment.id}" class="btn btn-sm btn-primary rounded-3">
                <i class="fas fa-info-circle me-1"></i>
                تفاصيل الموعد
              </a>
            </div>
          </div>
        </div>
      `;
    });
  }
  
  appointmentsList.innerHTML = appointmentsHTML;
}

/**
 * تحميل بيانات السجلات الطبية
 */
function loadMedicalRecords() {
  // بيانات السجلات الطبية
  const medicalRecords = [
    { 
      id: 1, 
      title: "فحص دوري", 
      doctor: "د. علي أحمد", 
      date: "05/01/2023", 
      description: "فحص عام، نتائج طبيعية" 
    },
    { 
      id: 2, 
      title: "علاج التهاب", 
      doctor: "د. سارة خالد", 
      date: "10/02/2023", 
      description: "التهاب جلدي، وصف مضاد حيوي" 
    },
    { 
      id: 3, 
      title: "فحص قلب", 
      doctor: "د. محمد عبد الله", 
      date: "15/03/2023", 
      description: "تخطيط قلب، نتائج طبيعية" 
    },
  ];
  
  // الحصول على حاوية السجلات الطبية
  const medicalRecordsList = document.getElementById('medicalRecordsList');
  if (!medicalRecordsList) return;
  
  // إنشاء HTML لكل سجل طبي
  let medicalRecordsHTML = '';
  
  if (medicalRecords.length === 0) {
    medicalRecordsHTML = `
      <div class="text-center py-5">
        <div class="text-muted mb-3">
          <i class="fas fa-file-medical fa-3x"></i>
        </div>
        <h5 class="fw-bold mb-2">لا توجد سجلات طبية</h5>
        <p class="text-muted">لم يتم إضافة أي سجلات طبية بعد</p>
      </div>
    `;
  } else {
    medicalRecords.forEach(record => {
      medicalRecordsHTML += `
        <div class="card mb-3 border-0 shadow-sm hover-card">
          <div class="card-body p-3">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <h5 class="card-title h6 fw-bold mb-1">${record.title}</h5>
                <p class="text-muted small mb-2">${record.doctor}</p>
              </div>
              <span class="text-muted small">${record.date}</span>
            </div>
            
            <p class="mb-3">${record.description}</p>
            
            <div class="d-flex justify-content-end">
              <a href="medical-record-details.html?id=${record.id}" class="btn btn-sm btn-primary rounded-3">
                <i class="fas fa-file-medical me-1"></i>
                عرض التقرير الكامل
              </a>
            </div>
          </div>
        </div>
      `;
    });
  }
  
  medicalRecordsList.innerHTML = medicalRecordsHTML;
}

/**
 * إعداد وظائف تحرير الملف الشخصي
 */
function setupProfileEdit() {
  const editProfileBtn = document.getElementById('editProfileBtn');
  const cancelEditBtn = document.getElementById('cancelEditBtn');
  const editProfileForm = document.getElementById('editProfileForm');
  const viewProfileMode = document.getElementById('viewProfileMode');
  const editProfileMode = document.getElementById('editProfileMode');
  
  if (!editProfileBtn || !cancelEditBtn || !editProfileForm || !viewProfileMode || !editProfileMode) return;
  
  // بيانات المريض الأولية
  const patientData = {
    name: "أحمد محمد",
    email: "ahmed@example.com",
    phone: "0501234567",
    birthDate: "01/05/1985",
    // bloodType: "A+",
    // allergies: "لا يوجد",
    // chronicDiseases: "ضغط الدم المرتفع"
  };
  
  // وضع البيانات في العناصر المعروضة
  document.getElementById('displayName').textContent = patientData.name;
  document.getElementById('displayEmail').textContent = patientData.email;
  document.getElementById('displayPhone').textContent = patientData.phone;
  document.getElementById('displayBirthDate').textContent = patientData.birthDate;
  document.getElementById('displayBloodType').textContent = patientData.bloodType;
  document.getElementById('displayAllergies').textContent = patientData.allergies;
  document.getElementById('displayChronicDiseases').textContent = patientData.chronicDiseases;
  
  // وضع البيانات في نموذج التحرير
  document.getElementById('editName').value = patientData.name;
  document.getElementById('editEmail').value = patientData.email;
  document.getElementById('editPhone').value = patientData.phone;
  document.getElementById('editBirthDate').value = patientData.birthDate;
  document.getElementById('editBloodType').value = patientData.bloodType;
  document.getElementById('editAllergies').value = patientData.allergies;
  document.getElementById('editChronicDiseases').value = patientData.chronicDiseases;
  
  // التبديل إلى وضع التحرير
  editProfileBtn.addEventListener('click', function() {
    viewProfileMode.classList.add('d-none');
    editProfileMode.classList.remove('d-none');
  });
  
  // إلغاء التحرير
  cancelEditBtn.addEventListener('click', function() {
    viewProfileMode.classList.remove('d-none');
    editProfileMode.classList.add('d-none');
  });
  
  // حفظ التغييرات
  editProfileForm.addEventListener('submit', function(e) {
    e.preventDefault();
    
    // جمع البيانات من النموذج
    const updatedData = {
      name: document.getElementById('editName').value,
      email: document.getElementById('editEmail').value,
      phone: document.getElementById('editPhone').value,
      birthDate: document.getElementById('editBirthDate').value,
      bloodType: document.getElementById('editBloodType').value,
      allergies: document.getElementById('editAllergies').value,
      chronicDiseases: document.getElementById('editChronicDiseases').value
    };
    
    // تحديث البيانات المعروضة
    document.getElementById('displayName').textContent = updatedData.name;
    document.getElementById('displayEmail').textContent = updatedData.email;
    document.getElementById('displayPhone').textContent = updatedData.phone;
    document.getElementById('displayBirthDate').textContent = updatedData.birthDate;
    // document.getElementById('displayBloodType').textContent = updatedData.bloodType;
    // document.getElementById('displayAllergies').textContent = updatedData.allergies;
    // document.getElementById('displayChronicDiseases').textContent = updatedData.chronicDiseases;
    
    // عرض إشعار نجاح التحديث
    window.medicalConnect.showToast('تم تحديث البيانات', 'تم تحديث بيانات الملف الشخصي بنجاح', 'success');
    
    // العودة إلى وضع العرض
    viewProfileMode.classList.remove('d-none');
    editProfileMode.classList.add('d-none');
  });
}