document.addEventListener('DOMContentLoaded', function () {
    // تهيئة مكونات Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // معالجة البحث في المواعيد
    const searchInput = document.getElementById('searchAppointments');
    const searchButton = document.getElementById('searchAppointmentsBtn');

    if (searchButton && searchInput) {
        searchButton.addEventListener('click', function () {
            searchAppointments(searchInput.value);
        });

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                searchAppointments(searchInput.value);
            }
        });
    }

    // وظيفة البحث في المواعيد
    function searchAppointments(query) {
        if (!query.trim()) {
            // إعادة عرض جميع المواعيد إذا كان البحث فارغًا
            document.querySelectorAll('tbody tr').forEach(row => {
                row.style.display = '';
            });
            return;
        }

        query = query.toLowerCase();
        let resultsFound = 0;

        // البحث في جميع الصفوف في التبويب النشط
        const activeTabContent = document.querySelector('.tab-pane.active');
        const rows = activeTabContent.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const doctorName = row.querySelector('a.fw-medium').textContent.toLowerCase();
            const specialty = row.querySelector('td:nth-child(2)').textContent.toLowerCase();
            const date = row.querySelector('td:nth-child(3)').textContent.toLowerCase();
            const visitType = row.querySelector('.badge').textContent.toLowerCase();

            if (doctorName.includes(query) || specialty.includes(query) ||
                date.includes(query) || visitType.includes(query)) {
                row.style.display = '';
                resultsFound++;
            } else {
                row.style.display = 'none';
            }
        });

        if (resultsFound === 0) {
            const noResultsRow = document.createElement('tr');
            noResultsRow.id = 'noResultsRow';
            noResultsRow.innerHTML = `
                <td colspan="7" class="text-center py-4">
                    <div class="text-muted">
                        <i class="fas fa-search fa-3x mb-3"></i>
                        <p>لم يتم العثور على نتائج لـ "${query}"</p>
                        <button class="btn btn-sm btn-outline-primary mt-2" id="clearSearchBtn">
                            <i class="fas fa-times me-2"></i>مسح البحث
                        </button>
                    </div>
                </td>
            `;

            // إزالة صف "لا نتائج" السابق إذا كان موجودًا
            const existingNoResults = activeTabContent.querySelector('#noResultsRow');
            if (existingNoResults) {
                existingNoResults.remove();
            }

            // إضافة صف "لا نتائج"
            activeTabContent.querySelector('tbody').appendChild(noResultsRow);

            // إضافة معالج الحدث لزر مسح البحث
            document.getElementById('clearSearchBtn').addEventListener('click', function () {
                searchInput.value = '';
                // إعادة عرض جميع المواعيد
                resetSearch();
            });
        } else {
            // إزالة صف "لا نتائج" إذا كان موجودًا
            const existingNoResults = activeTabContent.querySelector('#noResultsRow');
            if (existingNoResults) {
                existingNoResults.remove();
            }
        }

        // تحديث تعليق العرض
        updateShowingCount(resultsFound);
    }

    // تحديث عدد العناصر المعروضة
    function updateShowingCount(count) {
        const showingCount = document.getElementById('showingCount');
        const totalCount = document.getElementById('totalCount');

        if (showingCount && totalCount) {
            if (count === 0 || count === parseInt(totalCount.textContent)) {
                showingCount.textContent = `1-${totalCount.textContent}`;
            } else {
                showingCount.textContent = `1-${count}`;
            }
        }
    }

    // إعادة ضبط البحث
    function resetSearch() {
        // إعادة عرض جميع الصفوف
        document.querySelectorAll('tbody tr').forEach(row => {
            row.style.display = '';
        });

        // إزالة صف "لا نتائج" إذا كان موجودًا
        document.querySelectorAll('#noResultsRow').forEach(row => {
            row.remove();
        });

        // إعادة تعيين تعليق العرض
        const totalCount = document.getElementById('totalCount');
        if (totalCount) {
            updateShowingCount(parseInt(totalCount.textContent));
        }
    }

    // معالجة مودال إلغاء الموعد
    const cancelAppointmentModal = document.getElementById('cancelAppointmentModal');
    if (cancelAppointmentModal) {
        cancelAppointmentModal.addEventListener('show.bs.modal', function (event) {
            // زر الذي تم النقر عليه
            const button = event.relatedTarget;

            // استخراج معلومات الطبيب من الصف الخاص بالموعد
            const row = button.closest('tr');
            const doctorName = row.querySelector('a.fw-medium').textContent;

            // تحديث اسم الطبيب في المودال
            document.getElementById('cancelDoctorName').textContent = doctorName;
        });
    }

    // معالجة زر تأكيد الإلغاء
    const confirmCancelBtn = document.getElementById('confirmCancelBtn');
    const cancellationReason = document.getElementById('cancellationReason');
    const otherReasonContainer = document.getElementById('otherReasonContainer');
    const otherReason = document.getElementById('otherReason');

    if (cancellationReason) {
        cancellationReason.addEventListener('change', function () {
            if (this.value === 'other') {
                otherReasonContainer.style.display = 'block';
            } else {
                otherReasonContainer.style.display = 'none';
            }
        });
    }

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

    // محاكاة إلغاء الموعد
    function simulateCancelAppointment() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لإلغاء الموعد

        // إظهار رسالة نجاح
        const modal = bootstrap.Modal.getInstance(document.getElementById('cancelAppointmentModal'));
        modal.hide();

        setTimeout(() => {
            alert('تم إلغاء الموعد بنجاح. ستصلك رسالة تأكيد على بريدك الإلكتروني.');
            // إعادة تحميل الصفحة لعرض التغييرات
            window.location.reload();
        }, 500);
    }

    // مراقبة أحداث التبويبات للحفاظ على حالة البحث
    document.querySelectorAll('button[data-bs-toggle="tab"]').forEach(function (tabButton) {
        tabButton.addEventListener('shown.bs.tab', function (e) {
            // إذا كان هناك بحث نشط، تطبيقه على التبويب الجديد
            if (searchInput.value.trim() !== '') {
                searchAppointments(searchInput.value);
            }
        });
    });

    // تحديث إحصائيات المواعيد (محاكاة)
    updateAppointmentStatistics();

    function updateAppointmentStatistics() {
        // في التطبيق الحقيقي، هذه البيانات ستأتي من الخادم
        const statistics = {
            total: 12,
            upcoming: 3,
            completed: 8,
            cancelled: 1
        };

        // تحديث العدادات
        document.getElementById('appointmentCount').textContent = statistics.total;
        document.getElementById('upcomingCount').textContent = statistics.upcoming;
        document.getElementById('completedCount').textContent = statistics.completed;
        document.getElementById('cancelledCount').textContent = statistics.cancelled;

        // تحديث إجمالي العدد في شريط الترويسة
        document.getElementById('totalCount').textContent = statistics.total;
    }
});