document.addEventListener('DOMContentLoaded', function () {
    // معالجة البحث في الأسئلة الشائعة
    const searchInput = document.getElementById('searchFaq');
    const searchButton = document.getElementById('searchFaqButton');

    if (searchButton && searchInput) {
        searchButton.addEventListener('click', function () {
            searchFAQs(searchInput.value);
        });

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                searchFAQs(searchInput.value);
            }
        });
    }

    // وظيفة البحث في الأسئلة الشائعة
    function searchFAQs(query) {
        if (!query.trim()) {
            return;
        }

        query = query.toLowerCase();
        let resultsFound = 0;

        // البحث في جميع الأقسام
        document.querySelectorAll('.accordion-item').forEach(item => {
            const questionText = item.querySelector('.accordion-button').textContent.toLowerCase();
            const answerText = item.querySelector('.accordion-body').textContent.toLowerCase();

            if (questionText.includes(query) || answerText.includes(query)) {
                // إظهار العنصر وفتحه
                item.style.display = '';

                // فتح الأكورديون إذا كان مغلقًا
                const accordionButton = item.querySelector('.accordion-button');
                const accordionCollapse = item.querySelector('.accordion-collapse');

                if (accordionButton.classList.contains('collapsed')) {
                    accordionButton.classList.remove('collapsed');
                    accordionCollapse.classList.add('show');
                }

                // التبديل إلى التبويب المناسب
                const tabId = item.closest('.tab-pane').id;
                document.querySelector(`[data-bs-target="#${tabId}"]`).click();

                resultsFound++;

                // تظليل كلمات البحث
                highlightSearchTerms(item, query);
            } else {
                // إخفاء العنصر إذا لم يتطابق
                item.style.display = 'none';
            }
        });

        if (resultsFound === 0) {
            alert(`لم يتم العثور على نتائج لـ "${query}". حاول باستخدام كلمات أخرى.`);

            // إظهار جميع العناصر مرة أخرى
            resetSearch();
        }
    }

    // تظليل كلمات البحث
    function highlightSearchTerms(item, query) {
        // إزالة أي تظليل سابق
        item.querySelectorAll('mark').forEach(mark => {
            const parent = mark.parentNode;
            parent.replaceChild(document.createTextNode(mark.textContent), mark);
            parent.normalize();
        });

        // تظليل النص في السؤال
        const questionButton = item.querySelector('.accordion-button');
        const questionText = questionButton.textContent;
        const newQuestionText = questionText.replace(
            new RegExp(query, 'gi'),
            match => `<mark class="bg-warning text-dark">${match}</mark>`
        );
        questionButton.innerHTML = newQuestionText;

        // تظليل النص في الإجابة
        const answerBody = item.querySelector('.accordion-body');
        const paragraphs = answerBody.querySelectorAll('p, li');

        paragraphs.forEach(p => {
            const text = p.textContent;
            const newText = text.replace(
                new RegExp(query, 'gi'),
                match => `<mark class="bg-warning text-dark">${match}</mark>`
            );
            p.innerHTML = newText;
        });
    }

    // إعادة ضبط البحث وإظهار جميع العناصر
    function resetSearch() {
        // إعادة عرض جميع العناصر
        document.querySelectorAll('.accordion-item').forEach(item => {
            item.style.display = '';

            // إزالة أي تظليل
            item.querySelectorAll('mark').forEach(mark => {
                const parent = mark.parentNode;
                parent.replaceChild(document.createTextNode(mark.textContent), mark);
                parent.normalize();
            });
        });

        // العودة إلى التبويب الأول
        document.querySelector('[data-bs-target="#general"]').click();
    }

    // إضافة زر لمسح البحث
    const searchContainer = searchInput.parentElement;
    const clearButton = document.createElement('button');
    clearButton.className = 'btn btn-outline-secondary';
    clearButton.innerHTML = '<i class="fas fa-times"></i>';
    clearButton.type = 'button';
    clearButton.addEventListener('click', function () {
        searchInput.value = '';
        resetSearch();
    });

    // إضافة الزر بعد زر البحث
    searchButton.after(clearButton);

    // تعامل مع التبديل بين التبويبات للحفاظ على ارتفاع الصفحة
    document.querySelectorAll('[data-bs-toggle="pill"]').forEach(tabButton => {
        tabButton.addEventListener('shown.bs.tab', function (e) {
            // الحفاظ على موضع التمرير عند تبديل التبويبات
            window.scrollTo(0, window.scrollY);
        });
    });
});