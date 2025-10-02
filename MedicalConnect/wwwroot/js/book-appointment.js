// متغيرات عامة
let selectedDate = null;
let selectedTime = null;
let currentStep = 1;
let calendarInstance = null;

// تهيئة التقويم عند تحميل الصفحة
document.addEventListener('DOMContentLoaded', function () {
    initializeCalendar();
    initializeEventListeners();
});

// تهيئة التقويم
function initializeCalendar() {
    const calendarEl = document.getElementById('appointment-calendar');

    if (!calendarEl) return;

    // تحويل التواريخ المتاحة إلى صيغة Date objects
    const availableDatesObj = bookingData.availableDates.map(d => new Date(d));

    calendarInstance = flatpickr(calendarEl, {
        locale: 'ar',
        inline: true,
        minDate: 'today',
        dateFormat: 'Y-m-d',
        enable: availableDatesObj,
        onChange: function (selectedDates, dateStr, instance) {
            if (selectedDates.length > 0) {
                selectedDate = dateStr;
                loadAvailableTimeSlots(selectedDate);
            }
        },
        onDayCreate: function (dObj, dStr, fp, dayElem) {
            const date = dayElem.dateObj;
            const dateStr = flatpickr.formatDate(date, 'Y-m-d');

            // تحقق إذا كان التاريخ متاح
            if (!bookingData.availableDates.includes(dateStr)) {
                dayElem.classList.add('flatpickr-disabled');
            }
        }
    });
}

// جلب الأوقات المتاحة من قاعدة البيانات
async function loadAvailableTimeSlots(dateStr) {
    const availableTimesContainer = document.getElementById('available-times');

    // عرض loader
    availableTimesContainer.innerHTML = `
        <div class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">جاري التحميل...</span>
            </div>
            <p class="mt-3 text-muted">جاري جلب الأوقات المتاحة...</p>
        </div>
    `;

    try {
        const response = await fetch('/Appointments/GetAvailableTimeSlots', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                doctorId: bookingData.doctorId,
                date: dateStr
            })
        });

        const result = await response.json();

        if (result.success && result.timeSlots && result.timeSlots.length > 0) {
            displayTimeSlots(result.timeSlots);
        } else {
            availableTimesContainer.innerHTML = `
                <div class="text-center text-muted py-5">
                    <i class="fas fa-calendar-times fa-3x mb-3 text-danger opacity-50"></i>
                    <p class="mb-0">لا توجد أوقات متاحة في هذا التاريخ</p>
                </div>
            `;
        }
    } catch (error) {
        console.error('Error loading time slots:', error);
        showToast('حدث خطأ في جلب الأوقات المتاحة', 'error');
        availableTimesContainer.innerHTML = `
            <div class="text-center text-danger py-5">
                <i class="fas fa-exclamation-triangle fa-3x mb-3"></i>
                <p class="mb-0">حدث خطأ في جلب الأوقات المتاحة</p>
            </div>
        `;
    }
}

// عرض الأوقات المتاحة
function displayTimeSlots(timeSlots) {
    const availableTimesContainer = document.getElementById('available-times');

    // تقسيم الأوقات إلى صباحي ومسائي
    const morningSlots = timeSlots.filter(slot => slot.startTime.hours < 12);
    const afternoonSlots = timeSlots.filter(slot => slot.startTime.hours >= 12);

    let html = '';

    // أوقات المساء
    if (afternoonSlots.length > 0) {
        html += `
            <div class="mb-4">
                <h6 class="text-muted mb-3 fw-semibold">
                    <i class="fas fa-moon text-info me-2"></i>
                    الأوقات المسائية
                </h6>
                <div class="d-flex flex-wrap gap-2">
        `;

        afternoonSlots.forEach(slot => {
            html += `
                <button type="button" class="btn btn-outline-primary btn-time px-3 py-2 rounded-3" 
                        data-time="${slot.displayTime}" 
                        data-hours="${slot.startTime.hours}" 
                        data-minutes="${slot.startTime.minutes}">
                    ${slot.displayTime} ${slot.period}
                </button>
            `;
        });

        html += `
                </div>
            </div>
        `;
    }

    // إذا لم يكن هناك أوقات متاحة
    if (morningSlots.length === 0 && afternoonSlots.length === 0) {
        html = `
            <div class="text-center text-muted py-5">
                <i class="fas fa-calendar-times fa-3x mb-3 text-danger opacity-50"></i>
                <p class="mb-0">لا توجد أوقات متاحة في هذا التاريخ</p>
            </div>
        `;
    }

    availableTimesContainer.innerHTML = html;

    // إضافة event listeners لأزرار الأوقات
    document.querySelectorAll('.btn-time').forEach(btn => {
        btn.addEventListener('click', function () {
            selectTimeSlot(this);
        });
    });
}

// اختيار وقت
function selectTimeSlot(button) {
    // إزالة التحديد من جميع الأزرار
    document.querySelectorAll('.btn-time').forEach(btn => {
        btn.classList.remove('active');
        btn.classList.add('btn-outline-primary');
        btn.classList.remove('btn-primary');
    });

    // تحديد الزر المختار
    button.classList.add('active');
    button.classList.remove('btn-outline-primary');
    button.classList.add('btn-primary');

    // حفظ الوقت المختار
    const hours = button.getAttribute('data-hours');
    const minutes = button.getAttribute('data-minutes');
    selectedTime = `${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}:00`;

    // تفعيل زر الخطوة التالية
    document.getElementById('goto-step-2').disabled = false;
}

// تهيئة Event Listeners
function initializeEventListeners() {
    // زر الانتقال للخطوة الثانية
    document.getElementById('goto-step-2').addEventListener('click', function () {
        if (selectedDate && selectedTime) {
            goToStep(2);
        } else {
            showToast('يرجى اختيار التاريخ والوقت أولاً', 'warning');
        }
    });

    // زر العودة للخطوة الأولى
    document.getElementById('back-to-step-1').addEventListener('click', function () {
        goToStep(1);
    });

    // زر الانتقال للخطوة الثالثة
    document.getElementById('goto-step-3').addEventListener('click', function () {
        if (validateStep2()) {
            updateConfirmationDetails();
            goToStep(3);
        }
    });

    // زر العودة للخطوة الثانية
    document.getElementById('back-to-step-2').addEventListener('click', function () {
        goToStep(2);
    });

    // زر تأكيد الحجز
    document.getElementById('confirm-booking').addEventListener('click', function () {
        confirmBooking();
    });
}

// الانتقال بين الخطوات
function goToStep(step) {
    // إخفاء جميع الخطوات
    for (let i = 1; i <= 4; i++) {
        document.getElementById(`booking-step-${i}`).classList.add('d-none');
        document.getElementById(`step-${i}-indicator`)?.classList.remove('active');
    }

    // إظهار الخطوة المطلوبة
    document.getElementById(`booking-step-${step}`).classList.remove('d-none');

    // تحديث مؤشرات الخطوات
    for (let i = 1; i <= step; i++) {
        const indicator = document.getElementById(`step-${i}-indicator`);
        if (indicator) {
            indicator.classList.add('active');
            const icon = indicator.querySelector('.booking-step-icon');
            icon.classList.remove('bg-light', 'text-muted');
            icon.classList.add('bg-primary', 'text-white');
        }
    }

    currentStep = step;

    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// التحقق من صحة بيانات الخطوة الثانية
function validateStep2() {
    const reason = document.getElementById('appointmentReason').value;
    const phone = document.getElementById('patientPhone').value;

    if (!reason) {
        showToast('يرجى اختيار سبب الزيارة', 'warning');
        return false;
    }

    if (!phone) {
        showToast('يرجى إدخال رقم الهاتف', 'warning');
        return false;
    }

    return true;
}

// تحديث تفاصيل التأكيد
function updateConfirmationDetails() {
    // التاريخ
    const dateObj = new Date(selectedDate);
    const formattedDate = new Intl.DateTimeFormat('ar-EG', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(dateObj);
    document.getElementById('confirm-date').textContent = formattedDate;

    // الوقت
    const timeBtn = document.querySelector('.btn-time.active');
    if (timeBtn) {
        document.getElementById('confirm-time').textContent = timeBtn.textContent.trim();
    }

    // نوع الموعد
    const appointmentType = document.querySelector('input[name="AppointmentType"]:checked');
    const typeText = appointmentType.id === 'inPerson' ? 'زيارة في العيادة' : 'استشارة عن بعد';
    document.getElementById('confirm-type').textContent = typeText;

    // سبب الزيارة
    const reason = document.getElementById('appointmentReason');
    document.getElementById('confirm-reason').textContent = reason.options[reason.selectedIndex].text;
}

// تأكيد الحجز - إرسال البيانات لقاعدة البيانات
async function confirmBooking() {
    const termsCheck = document.getElementById('termsCheck');

    if (!termsCheck.checked) {
        showToast('يرجى الموافقة على الشروط والأحكام', 'warning');
        return;
    }

    // إظهار loader
    const confirmBtn = document.getElementById('confirm-booking');
    const spinner = document.getElementById('booking-spinner');
    confirmBtn.disabled = true;
    spinner.classList.remove('d-none');

    // جمع البيانات من النموذج
    const bookingDetails = {
        DoctorId: bookingData.doctorId,
        SelectedDate: selectedDate,
        SelectedTime: selectedTime,
        AppointmentType: document.querySelector('input[name="AppointmentType"]:checked').value,
        Reason: document.getElementById('appointmentReason').value,
        Notes: document.getElementById('notes').value,
        PaymentMethod: document.querySelector('input[name="PaymentMethod"]:checked').value
    };

    try {
        const response = await fetch('/Appointments/ConfirmBooking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(bookingDetails)
        });

        const result = await response.json();

        if (result.success) {
            // عرض صفحة التأكيد
            displaySuccessPage(result.bookingNumber);
            showToast('تم تأكيد الحجز بنجاح!', 'success');
        } else {
            showToast(result.message || 'حدث خطأ أثناء تأكيد الحجز', 'error');
            confirmBtn.disabled = false;
            spinner.classList.add('d-none');
        }
    } catch (error) {
        console.error('Error confirming booking:', error);
        showToast('حدث خطأ أثناء تأكيد الحجز', 'error');
        confirmBtn.disabled = false;
        spinner.classList.add('d-none');
    }
}

// عرض صفحة النجاح
function displaySuccessPage(bookingNumber) {
    goToStep(4);

    // تحديث تفاصيل الحجز في صفحة النجاح
    document.getElementById('booking-number').textContent = bookingNumber;

    const dateObj = new Date(selectedDate);
    const formattedDate = new Intl.DateTimeFormat('ar-EG', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(dateObj);
    document.getElementById('booking-date').textContent = formattedDate;

    const timeBtn = document.querySelector('.btn-time.active');
    if (timeBtn) {
        document.getElementById('booking-time').textContent = timeBtn.textContent.trim();
    }

    const appointmentType = document.querySelector('input[name="AppointmentType"]:checked');
    const typeText = appointmentType.id === 'inPerson' ? 'زيارة في العيادة' : 'استشارة عن بعد';
    document.getElementById('booking-type').textContent = typeText;
}

// عرض Toast notification
function showToast(message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');

    const bgClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';

    const icon = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-circle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    }[type] || 'fa-info-circle';

    const toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas ${icon} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    const toastElement = toastContainer.lastElementChild;
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: 5000
    });

    toast.show();

    // حذف التوست بعد الإخفاء
    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });
}ات الصباح
if (morningSlots.length > 0) {
    html += `
            <div class="mb-4">
                <h6 class="text-muted mb-3 fw-semibold">
                    <i class="fas fa-sun text-warning me-2"></i>
                    الأوقات الصباحية
                </h6>
                <div class="d-flex flex-wrap gap-2">
        `;

    morningSlots.forEach(slot => {
        html += `
                <button type="button" class="btn btn-outline-primary btn-time px-3 py-2 rounded-3" 
                        data-time="${slot.displayTime}" 
                        data-hours="${slot.startTime.hours}" 
                        data-minutes="${slot.startTime.minutes}">
                    ${slot.displayTime} ${slot.period}
                </button>
            `;
    });

    html += `
                </div>
            </div>
        `;
}

// أوق