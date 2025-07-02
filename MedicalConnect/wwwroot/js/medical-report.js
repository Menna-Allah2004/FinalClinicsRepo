document.addEventListener('DOMContentLoaded', function () {
    // معالجة زر الطباعة
    const printReportBtn = document.getElementById('printReport');
    if (printReportBtn) {
        printReportBtn.addEventListener('click', function () {
            window.print();
        });
    }

    // معالجة زر تحميل PDF
    const downloadReportBtn = document.getElementById('downloadReport');
    if (downloadReportBtn) {
        downloadReportBtn.addEventListener('click', function () {
            // في التطبيق الحقيقي، هنا ستكون هناك منطق لتحويل التقرير إلى PDF
            // نستخدم محاكاة بسيطة للعرض
            simulateDownloadPDF();
        });
    }

    // محاكاة تحميل ملف PDF
    function simulateDownloadPDF() {
        // إظهار رسالة للمستخدم
        alert('جاري تحضير ملف PDF للتحميل...');

        // محاكاة تحميل الملف بعد بضع ثوان
        setTimeout(function () {
            alert('تم تحميل ملف التقرير الطبي بنجاح.');
        }, 2000);
    }

    // إضافة التأثيرات البصرية والمعلومات الديناميكية إلى التقرير

    // استرجاع بيانات التقرير من الخادم (محاكاة)
    function fetchReportData() {
        // في التطبيق الحقيقي، هنا ستكون هناك طلب API لاسترجاع بيانات التقرير

        // بيانات التقرير المحاكاة
        const reportData = {
            reportNumber: 'MR-2023-001548',
            doctorName: 'د. خالد عبدالرحمن',
            doctorSpecialty: 'أخصائي الباطنة والقلب',
            medicalLicense: 'MOMC-12345',
            visitDate: '15 أبريل 2023',
            visitTime: '10:30 صباحاً',
            patientName: 'أحمد محمد العمري',
            patientFileNumber: 'PAT-10056874',
            patientAge: '42 سنة',
            patientGender: 'ذكر',
            vitalSigns: {
                bloodPressure: '120/80 mmHg',
                heartRate: '76 bpm',
                temperature: '37.1 °C',
                respirationRate: '18 rpm'
            },
            chiefComplaint: 'ألم خفيف في منطقة الصدر، مع شعور بالإرهاق والتعب المستمر في الأيام الماضية.',
            medicalHistory: 'المريض يعاني من ارتفاع ضغط الدم منذ 5 سنوات، ويتناول دواء Lisinopril بانتظام. لديه تاريخ عائلي لأمراض القلب (والده توفي بسبب نوبة قلبية في سن 60).',
            clinicalExamination: 'المريض بحالة عامة جيدة. لا توجد علامات تشير إلى قصور القلب. أصوات القلب طبيعية بدون نفخات. الفحص الرئوي طبيعي. لا يوجد تورم في الأطراف السفلية.',
            diagnosis: [
                'ارتفاع ضغط الدم (I10)',
                'الذبحة الصدرية المستقرة (I20.9)',
                'اضطراب القلق (F41.9)'
            ],
            requiredTests: [
                'تخطيط القلب الكهربائي (ECG)',
                'فحص الدم الشامل (CBC)',
                'وظائف الكلى والكبد',
                'اختبار الإجهاد (Stress Test)'
            ],
            medications: [
                { name: 'Lisinopril', dose: '10 mg', frequency: 'مرة واحدة يومياً', duration: 'شهر', notes: 'يؤخذ في الصباح' },
                { name: 'Aspirin', dose: '81 mg', frequency: 'مرة واحدة يومياً', duration: 'شهر', notes: 'يؤخذ بعد الطعام' },
                { name: 'Atorvastatin', dose: '20 mg', frequency: 'مرة واحدة يومياً', duration: 'شهر', notes: 'يؤخذ قبل النوم' }
            ],
            lifestyleRecommendations: [
                'تقليل استهلاك الملح إلى أقل من 5 غرام يومياً.',
                'ممارسة النشاط البدني المعتدل لمدة 30 دقيقة على الأقل، 5 أيام في الأسبوع.',
                'التوقف عن التدخين.',
                'تقليل تناول الكافيين.',
                'اتباع نظام غذائي متوازن غني بالخضروات والفواكه والحبوب الكاملة.'
            ],
            nextVisitDate: 'بعد أسبوعين من تاريخ هذه الزيارة',
            followUpNotes: 'إحضار نتائج جميع الفحوصات المطلوبة في الزيارة القادمة. مراقبة ضغط الدم مرتين يومياً وتسجيل القراءات.'
        };

        return reportData;
    }

    // عرض بيانات التقرير في الصفحة
    function displayReportData() {
        const reportData = fetchReportData();

        // تعيين معلومات التقرير والطبيب
        document.getElementById('reportNumber').textContent = reportData.reportNumber;
        document.getElementById('doctorName').textContent = reportData.doctorName;
        document.getElementById('doctorSpecialty').textContent = reportData.doctorSpecialty;
        document.getElementById('medicalLicense').textContent = 'رقم الترخيص: ' + reportData.medicalLicense;
        document.getElementById('visitDate').textContent = reportData.visitDate;
        document.getElementById('visitTime').textContent = reportData.visitTime;

        // تعيين معلومات المريض
        document.getElementById('patientName').textContent = reportData.patientName;
        document.getElementById('patientFileNumber').textContent = reportData.patientFileNumber;
        document.getElementById('patientAge').textContent = reportData.patientAge;
        document.getElementById('patientGender').textContent = reportData.patientGender;

        // تعيين المؤشرات الحيوية
        document.getElementById('bloodPressure').textContent = reportData.vitalSigns.bloodPressure;
        document.getElementById('heartRate').textContent = reportData.vitalSigns.heartRate;
        document.getElementById('temperature').textContent = reportData.vitalSigns.temperature;
        document.getElementById('respirationRate').textContent = reportData.vitalSigns.respirationRate;

        // تعيين المعلومات السريرية
        document.getElementById('chiefComplaint').textContent = reportData.chiefComplaint;
        document.getElementById('medicalHistory').textContent = reportData.medicalHistory;
        document.getElementById('clinicalExamination').textContent = reportData.clinicalExamination;

        // تعيين التشخيص والفحوصات المطلوبة
        const diagnosisList = document.getElementById('diagnosisList');
        diagnosisList.innerHTML = '';
        reportData.diagnosis.forEach(diagnosis => {
            const li = document.createElement('li');
            li.className = 'mb-2';
            li.textContent = diagnosis;
            diagnosisList.appendChild(li);
        });

        const requiredTestsList = document.getElementById('requiredTestsList');
        requiredTestsList.innerHTML = '';
        reportData.requiredTests.forEach(test => {
            const li = document.createElement('li');
            li.className = 'mb-2';
            li.textContent = test;
            requiredTestsList.appendChild(li);
        });

        // تعيين الأدوية الموصوفة
        const medicationsList = document.getElementById('medicationsList');
        medicationsList.innerHTML = '';
        reportData.medications.forEach(medication => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${medication.name}</td>
                <td>${medication.dose}</td>
                <td>${medication.frequency}</td>
                <td>${medication.duration}</td>
                <td>${medication.notes}</td>
            `;
            medicationsList.appendChild(tr);
        });

        // تعيين التوصيات ومعلومات المتابعة
        const lifestyleRecommendationsList = document.getElementById('lifestyleRecommendationsList');
        lifestyleRecommendationsList.innerHTML = '';
        reportData.lifestyleRecommendations.forEach(recommendation => {
            const li = document.createElement('li');
            li.className = 'mb-2';
            li.textContent = recommendation;
            lifestyleRecommendationsList.appendChild(li);
        });

        document.getElementById('nextVisitDate').textContent = reportData.nextVisitDate;
        document.getElementById('followUpNotes').textContent = reportData.followUpNotes;
    }

    // تحميل بيانات التقرير عند تحميل الصفحة
    displayReportData();
});