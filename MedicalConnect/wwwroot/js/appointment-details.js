document.addEventListener('DOMContentLoaded', function () {
    // التعامل مع زر إلغاء الموعد
    const cancelAppointmentBtn = document.getElementById('cancelAppointmentBtn');
    const confirmCancelBtn = document.getElementById('confirmCancelBtn');
    const cancellationReason = document.getElementById('cancellationReason');
    const otherReasonContainer = document.getElementById('otherReasonContainer');
    const otherReason = document.getElementById('otherReason');
    const cancelDoctorName = document.getElementById('cancelDoctorName');
    const doctorName = document.getElementById('doctorName');

    // عرض اسم الطبيب في مودال الإلغاء
    if (cancelDoctorName && doctorName) {
        cancelDoctorName.textContent = doctorName.textContent;
    }

    // عرض حقل "سبب آخر" عند اختيار "سبب آخر"
    if (cancellationReason) {
        cancellationReason.addEventListener('change', function () {
            if (this.value === 'other') {
                otherReasonContainer.style.display = 'block';
            } else {
                otherReasonContainer.style.display = 'none';
            }
        });
    }

    // معالجة زر تأكيد الإلغاء
    if (confirmCancelBtn) {
        confirmCancelBtn.addEventListener('click', function () {
            if (!cancellationReason.value) {
                alert('يرجى اختيار سبب الإلغاء');
                return;
            }

            if (cancellationReason.value === 'other' && !otherReason.value.trim()) {
                alert('يرجى ذكر سبب الإلغاء');
                return;
            }

            // إرسال طلب إلغاء الموعد (هنا يتم استبداله بالمحاكاة)
            simulateCancelAppointment();
        });
    }

    // التعامل مع زر إعادة جدولة الموعد
    const rescheduleAppointmentBtn = document.getElementById('rescheduleAppointmentBtn');
    const confirmRescheduleBtn = document.getElementById('confirmRescheduleBtn');
    const newAppointmentDate = document.getElementById('newAppointmentDate');
    const timeSlots = document.querySelectorAll('input[name="timeSlot"]');
    const rescheduleDoctorName = document.getElementById('rescheduleDoctorName');

    // عرض اسم الطبيب في مودال إعادة الجدولة
    if (rescheduleDoctorName && doctorName) {
        rescheduleDoctorName.textContent = doctorName.textContent;
    }

    // تعيين التاريخ الأدنى لليوم التالي
    if (newAppointmentDate) {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        newAppointmentDate.min = tomorrow.toISOString().split('T')[0];
    }

    // معالجة زر تأكيد إعادة الجدولة
    if (confirmRescheduleBtn) {
        confirmRescheduleBtn.addEventListener('click', function () {
            if (!newAppointmentDate.value) {
                alert('يرجى اختيار تاريخ للموعد الجديد');
                return;
            }

            let selectedTime = false;
            for (let i = 0; i < timeSlots.length; i++) {
                if (timeSlots[i].checked) {
                    selectedTime = true;
                    break;
                }
            }

            if (!selectedTime) {
                alert('يرجى اختيار وقت للموعد الجديد');
                return;
            }

            // إرسال طلب إعادة جدولة الموعد (هنا يتم استبداله بالمحاكاة)
            simulateRescheduleAppointment();
        });
    }

    // محاكاة إلغاء الموعد
    function simulateCancelAppointment() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لإلغاء الموعد

        // إظهار رسالة نجاح
        const modal = bootstrap.Modal.getInstance(document.getElementById('cancelAppointmentModal'));
        modal.hide();

        setTimeout(() => {
            alert('تم إلغاء الموعد بنجاح. ستصلك رسالة تأكيد على بريدك الإلكتروني.');
            window.location.href = 'appointments.html';
        }, 500);
    }

    // محاكاة إعادة جدولة الموعد
    function simulateRescheduleAppointment() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لإعادة جدولة الموعد

        // إظهار رسالة نجاح
        const modal = bootstrap.Modal.getInstance(document.getElementById('rescheduleAppointmentModal'));
        modal.hide();

        setTimeout(() => {
            alert('تمت إعادة جدولة الموعد بنجاح. ستصلك رسالة تأكيد على بريدك الإلكتروني.');
            window.location.href = 'appointments.html';
        }, 500);
    }
});