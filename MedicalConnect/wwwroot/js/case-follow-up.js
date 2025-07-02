document.addEventListener('DOMContentLoaded', function () {
    // إعداد الرسم البياني لضغط الدم
    const bloodPressureChartCtx = document.getElementById('bloodPressureChart').getContext('2d');

    // بيانات محاكاة لضغط الدم
    const dates = ['4/9', '4/10', '4/11', '4/12', '4/13', '4/14', '4/15'];
    const systolicData = [145, 138, 132, 128, 125, 122, 120];
    const diastolicData = [95, 90, 88, 85, 83, 82, 80];

    // إنشاء الرسم البياني
    const bloodPressureChart = new Chart(bloodPressureChartCtx, {
        type: 'line',
        data: {
            labels: dates,
            datasets: [
                {
                    label: 'الانقباضي (Systolic)',
                    data: systolicData,
                    borderColor: '#1976D2',
                    backgroundColor: 'rgba(25, 118, 210, 0.1)',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.4
                },
                {
                    label: 'الانبساطي (Diastolic)',
                    data: diastolicData,
                    borderColor: '#42A5F5',
                    backgroundColor: 'rgba(66, 165, 245, 0.1)',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.4
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += context.parsed.y + ' mmHg';
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    min: 70,
                    max: 150,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    },
                    ticks: {
                        stepSize: 20
                    }
                }
            },
            elements: {
                point: {
                    radius: 3,
                    hoverRadius: 5
                }
            }
        }
    });

    // تحديث النطاق الزمني للرسم البياني عند تغيير الاختيار
    const chartTimeRange = document.getElementById('chartTimeRange');
    if (chartTimeRange) {
        chartTimeRange.addEventListener('change', function () {
            let newDates = [];
            let newSystolicData = [];
            let newDiastolicData = [];

            // محاكاة تحديث البيانات بناءً على النطاق الزمني المحدد
            switch (this.value) {
                case 'آخر 7 أيام':
                    newDates = ['4/9', '4/10', '4/11', '4/12', '4/13', '4/14', '4/15'];
                    newSystolicData = [145, 138, 132, 128, 125, 122, 120];
                    newDiastolicData = [95, 90, 88, 85, 83, 82, 80];
                    break;
                case 'آخر 30 يوم':
                    // محاكاة بيانات لـ 30 يومًا (نعرض 8 نقاط فقط للتبسيط)
                    newDates = ['3/15', '3/20', '3/25', '3/30', '4/5', '4/10', '4/15'];
                    newSystolicData = [150, 148, 142, 140, 135, 130, 120];
                    newDiastolicData = [98, 96, 94, 92, 88, 85, 80];
                    break;
                case 'آخر 90 يوم':
                    // محاكاة بيانات لـ 90 يومًا (نعرض 8 نقاط فقط للتبسيط)
                    newDates = ['1/15', '2/1', '2/15', '3/1', '3/15', '4/1', '4/15'];
                    newSystolicData = [160, 155, 150, 145, 140, 130, 120];
                    newDiastolicData = [100, 98, 95, 92, 90, 85, 80];
                    break;
            }

            // تحديث بيانات الرسم البياني
            bloodPressureChart.data.labels = newDates;
            bloodPressureChart.data.datasets[0].data = newSystolicData;
            bloodPressureChart.data.datasets[1].data = newDiastolicData;
            bloodPressureChart.update();
        });
    }

    // معالجة مودال إضافة بيانات صحية
    const healthDataType = document.getElementById('healthDataType');
    const bloodPressureFields = document.getElementById('bloodPressureFields');
    const otherMeasurementField = document.getElementById('otherMeasurementField');
    const measurementUnit = document.getElementById('measurementUnit');

    if (healthDataType) {
        healthDataType.addEventListener('change', function () {
            // إظهار الحقول المناسبة بناءً على نوع البيانات المحدد
            switch (this.value) {
                case 'blood_pressure':
                    bloodPressureFields.style.display = 'block';
                    otherMeasurementField.style.display = 'none';
                    break;
                case 'heart_rate':
                    bloodPressureFields.style.display = 'none';
                    otherMeasurementField.style.display = 'block';
                    measurementUnit.textContent = 'نبضة/دقيقة';
                    break;
                case 'temperature':
                    bloodPressureFields.style.display = 'none';
                    otherMeasurementField.style.display = 'block';
                    measurementUnit.textContent = '°C';
                    break;
                case 'oxygen_level':
                    bloodPressureFields.style.display = 'none';
                    otherMeasurementField.style.display = 'block';
                    measurementUnit.textContent = '%';
                    break;
                case 'weight':
                    bloodPressureFields.style.display = 'none';
                    otherMeasurementField.style.display = 'block';
                    measurementUnit.textContent = 'كجم';
                    break;
                default:
                    bloodPressureFields.style.display = 'none';
                    otherMeasurementField.style.display = 'none';
            }
        });
    }

    // معالجة زر حفظ البيانات الصحية
    const saveHealthDataBtn = document.getElementById('saveHealthDataBtn');
    if (saveHealthDataBtn) {
        saveHealthDataBtn.addEventListener('click', function () {
            // التحقق من صحة النموذج
            if (!validateHealthDataForm()) {
                return;
            }

            // إرسال البيانات (محاكاة)
            simulateSaveHealthData();
        });
    }

    // التحقق من صحة نموذج البيانات الصحية
    function validateHealthDataForm() {
        const dataType = healthDataType.value;

        if (!dataType) {
            alert('يرجى اختيار نوع البيانات');
            return false;
        }

        if (dataType === 'blood_pressure') {
            const systolic = document.getElementById('systolic').value;
            const diastolic = document.getElementById('diastolic').value;

            if (!systolic || !diastolic) {
                alert('يرجى إدخال قيم ضغط الدم');
                return false;
            }

            if (parseInt(systolic) < 70 || parseInt(systolic) > 250) {
                alert('قيمة الضغط الانقباضي غير صالحة');
                return false;
            }

            if (parseInt(diastolic) < 40 || parseInt(diastolic) > 150) {
                alert('قيمة الضغط الانبساطي غير صالحة');
                return false;
            }
        } else {
            const measurementValue = document.getElementById('measurementValue').value;

            if (!measurementValue) {
                alert('يرجى إدخال قيمة القياس');
                return false;
            }

            // التحقق من صحة القيم حسب نوع البيانات
            switch (dataType) {
                case 'heart_rate':
                    if (parseInt(measurementValue) < 30 || parseInt(measurementValue) > 220) {
                        alert('قيمة معدل ضربات القلب غير صالحة');
                        return false;
                    }
                    break;
                case 'temperature':
                    if (parseFloat(measurementValue) < 35 || parseFloat(measurementValue) > 42) {
                        alert('قيمة درجة الحرارة غير صالحة');
                        return false;
                    }
                    break;
                case 'oxygen_level':
                    if (parseInt(measurementValue) < 70 || parseInt(measurementValue) > 100) {
                        alert('قيمة مستوى الأكسجين غير صالحة');
                        return false;
                    }
                    break;
            }
        }

        const measurementTime = document.getElementById('measurementTime').value;
        if (!measurementTime) {
            alert('يرجى إدخال وقت القياس');
            return false;
        }

        return true;
    }

    // محاكاة حفظ البيانات الصحية
    function simulateSaveHealthData() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لحفظ البيانات

        // إظهار رسالة تحميل
        saveHealthDataBtn.disabled = true;
        saveHealthDataBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span> جاري الحفظ...';

        setTimeout(() => {
            // إخفاء المودال وإظهار رسالة نجاح
            const modal = bootstrap.Modal.getInstance(document.getElementById('addHealthDataModal'));
            modal.hide();

            // إعادة تعيين الزر
            saveHealthDataBtn.disabled = false;
            saveHealthDataBtn.textContent = 'حفظ البيانات';

            // إعادة تعيين النموذج
            document.getElementById('healthDataForm').reset();
            bloodPressureFields.style.display = 'none';
            otherMeasurementField.style.display = 'none';

            // عرض رسالة نجاح
            alert('تم حفظ البيانات الصحية بنجاح!');

            // تحديث الصفحة لعرض البيانات الجديدة (في التطبيق الحقيقي يمكن تحديث العناصر مباشرة)
            // window.location.reload();
        }, 1500);
    }

    // معالجة مودال إضافة عرض
    const saveSymptomBtn = document.getElementById('saveSymptomBtn');
    if (saveSymptomBtn) {
        saveSymptomBtn.addEventListener('click', function () {
            // التحقق من صحة النموذج
            if (!validateSymptomForm()) {
                return;
            }

            // إرسال البيانات (محاكاة)
            simulateSaveSymptom();
        });
    }

    // التحقق من صحة نموذج الأعراض
    function validateSymptomForm() {
        const symptomName = document.getElementById('symptomName').value;
        const symptomTime = document.getElementById('symptomTime').value;
        const symptomDescription = document.getElementById('symptomDescription').value;

        if (!symptomName.trim()) {
            alert('يرجى إدخال اسم العرض');
            return false;
        }

        if (!symptomTime) {
            alert('يرجى إدخال وقت ظهور العرض');
            return false;
        }

        if (!symptomDescription.trim()) {
            alert('يرجى إدخال وصف العرض');
            return false;
        }

        return true;
    }

    // محاكاة حفظ الأعراض
    function simulateSaveSymptom() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لحفظ البيانات

        // إظهار رسالة تحميل
        saveSymptomBtn.disabled = true;
        saveSymptomBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span> جاري الحفظ...';

        setTimeout(() => {
            // إخفاء المودال وإظهار رسالة نجاح
            const modal = bootstrap.Modal.getInstance(document.getElementById('addSymptomModal'));
            modal.hide();

            // إعادة تعيين الزر
            saveSymptomBtn.disabled = false;
            saveSymptomBtn.textContent = 'حفظ العرض';

            // إعادة تعيين النموذج
            document.getElementById('symptomForm').reset();

            // عرض رسالة نجاح
            alert('تم حفظ العرض بنجاح!');

            // تحديث الصفحة لعرض البيانات الجديدة (في التطبيق الحقيقي يمكن تحديث العناصر مباشرة)
            // window.location.reload();
        }, 1500);
    }

    // معالجة مودال مراسلة الطبيب
    const sendMessageBtn = document.getElementById('sendMessageBtn');
    if (sendMessageBtn) {
        sendMessageBtn.addEventListener('click', function () {
            // التحقق من صحة النموذج
            if (!validateContactDoctorForm()) {
                return;
            }

            // إرسال الرسالة (محاكاة)
            simulateSendMessage();
        });
    }

    // التحقق من صحة نموذج مراسلة الطبيب
    function validateContactDoctorForm() {
        const doctorSelect = document.getElementById('doctorSelect').value;
        const messageSubject = document.getElementById('messageSubject').value;
        const messageBody = document.getElementById('messageBody').value;

        if (!doctorSelect) {
            alert('يرجى اختيار الطبيب');
            return false;
        }

        if (!messageSubject.trim()) {
            alert('يرجى إدخال موضوع الرسالة');
            return false;
        }

        if (!messageBody.trim()) {
            alert('يرجى إدخال نص الرسالة');
            return false;
        }

        return true;
    }

    // محاكاة إرسال رسالة للطبيب
    function simulateSendMessage() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لإرسال الرسالة

        // إظهار رسالة تحميل
        sendMessageBtn.disabled = true;
        sendMessageBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span> جاري الإرسال...';

        setTimeout(() => {
            // إخفاء المودال وإظهار رسالة نجاح
            const modal = bootstrap.Modal.getInstance(document.getElementById('contactDoctorModal'));
            modal.hide();

            // إعادة تعيين الزر
            sendMessageBtn.disabled = false;
            sendMessageBtn.textContent = 'إرسال الرسالة';

            // عرض رسالة نجاح
            alert('تم إرسال الرسالة بنجاح! سيتواصل معك الطبيب في أقرب وقت ممكن.');
        }, 2000);
    }

    // إضافة تأثيرات بصرية لسلسلة الزمنية للتعافي
    styleRecoveryTimeline();

    function styleRecoveryTimeline() {
        // تنسيق نقاط التعافي
        const recoveryPoints = document.querySelectorAll('.recovery-point');
        recoveryPoints.forEach(point => {
            // إضافة أنماط CSS للنقاط
            point.style.position = 'relative';
            point.style.zIndex = '1';

            // الحصول على أيقونة النقطة
            const icon = point.querySelector('.recovery-icon');
            if (icon) {
                // تنسيق الأيقونة
                icon.style.width = '40px';
                icon.style.height = '40px';
                icon.style.borderRadius = '50%';
                icon.style.display = 'inline-flex';
                icon.style.alignItems = 'center';
                icon.style.justifyContent = 'center';
                icon.style.boxShadow = '0 0 0 4px white';
            }
        });
    }
});