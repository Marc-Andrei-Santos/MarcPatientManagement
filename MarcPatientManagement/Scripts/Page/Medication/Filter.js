document.addEventListener('DOMContentLoaded', function () {
    const patientInput = document.querySelector('input[name="patientFilter"]');
    const drugInput = document.querySelector('input[name="drugFilter"]');
    const dosageInput = document.querySelector('input[name="dosageFilter"]');
    const dateInput = document.querySelector('input[name="modifiedDateFilter"]');
    const tableBody = document.querySelector('tbody');
    const pageSizeSelect = document.getElementById('pageSizeSelect');
    const filterBtn = document.getElementById('filterBtn');
    const resetBtn = document.getElementById('resetBtn');
    const paginationList = document.getElementById('pagination');
    const showingInfo = document.getElementById('showingInfo');

    if (resetBtn) resetBtn.classList.remove('show');

    const rawRows = Array.from(tableBody.querySelectorAll('tr'));
    const items = rawRows.map(row => {
        const cells = Array.from(row.querySelectorAll('td'));
        if (!cells || cells.length === 0) return null;
        if (cells.length === 1 && cells[0].hasAttribute('colspan')) return null;
        return {
            html: row.innerHTML,
            patient: cells[0].textContent.trim().toLowerCase(),
            drug: cells[1].textContent.trim().toLowerCase(),
            dosage: cells[2].textContent.trim(),
            date: cells[3].textContent.trim()
        };
    }).filter(x => x !== null);

    let filteredItems = items.slice();
    let currentPage = 1;

    function updateShowing(start, end, total) {
        if (!showingInfo) return;
        if (total === 0) showingInfo.textContent = `Showing 0 to 0 of 0 entries`;
        else showingInfo.textContent = `Showing ${start} to ${end} of ${total} entries`;
    }

    function renderPagination(totalPages) {
        paginationList.innerHTML = '';
        const prevLi = document.createElement('li');
        prevLi.className = 'page-item ' + (currentPage === 1 ? 'disabled' : '');
        const prevA = document.createElement('a');
        prevA.className = 'page-link text-success fw-bold';
        prevA.href = '#';
        prevA.innerHTML = '<i class="fas fa-chevron-left"></i> Prev';
        prevA.addEventListener('click', function (e) {
            e.preventDefault();
            if (currentPage > 1) { currentPage--; renderPage(); }
        });
        prevLi.appendChild(prevA);
        paginationList.appendChild(prevLi);

        const maxToShow = 7;
        let startPage = Math.max(1, currentPage - Math.floor(maxToShow / 2));
        let endPage = Math.min(totalPages, startPage + maxToShow - 1);
        if (endPage - startPage + 1 < maxToShow) {
            startPage = Math.max(1, endPage - maxToShow + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            const li = document.createElement('li');
            li.className = 'page-item mx-1 ' + (i === currentPage ? 'active' : '');
            const a = document.createElement('a');
            a.className = 'page-link fw-bold ' + (i === currentPage ? 'bg-success text-white' : 'text-success');
            a.href = '#';
            a.textContent = i;
            a.style.borderRadius = '6px';
            a.style.minWidth = '36px';
            a.style.textAlign = 'center';
            a.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = i;
                renderPage();
            });
            li.appendChild(a);
            paginationList.appendChild(li);
        }

        const nextLi = document.createElement('li');
        nextLi.className = 'page-item ' + (currentPage === totalPages ? 'disabled' : '');
        const nextA = document.createElement('a');
        nextA.className = 'page-link text-success fw-bold';
        nextA.href = '#';
        nextA.innerHTML = 'Next <i class="fas fa-chevron-right"></i>';
        nextA.addEventListener('click', function (e) {
            e.preventDefault();
            if (currentPage < totalPages) { currentPage++; renderPage(); }
        });
        nextLi.appendChild(nextA);
        paginationList.appendChild(nextLi);
    }

    function renderPage() {
        const pageSize = parseInt(pageSizeSelect.value, 10) || 10;
        const total = filteredItems.length;
        const totalPages = Math.max(1, Math.ceil(total / pageSize));
        if (currentPage > totalPages) currentPage = totalPages;
        const startIndex = (currentPage - 1) * pageSize;
        const endIndex = Math.min(startIndex + pageSize, total);
        tableBody.innerHTML = '';
        if (total === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted py-3">No records found.</td></tr>';
            updateShowing(0, 0, 0);
        } else {
            const fragment = document.createDocumentFragment();
            filteredItems.slice(startIndex, endIndex).forEach(item => {
                const tr = document.createElement('tr');
                tr.innerHTML = item.html;
                fragment.appendChild(tr);
            });
            tableBody.appendChild(fragment);
            updateShowing(startIndex + 1, endIndex, total);
        }
        renderPagination(totalPages);
    }

    function applyFilter() {
        const p = patientInput ? patientInput.value.trim().toLowerCase() : '';
        const d = drugInput ? drugInput.value.trim().toLowerCase() : '';
        const dos = dosageInput ? dosageInput.value.trim() : '';
        const date = dateInput ? dateInput.value : '';
        filteredItems = items.filter(it => {
            if (p && !it.patient.includes(p)) return false;
            if (d && !it.drug.includes(d)) return false;
            if (dos && !it.dosage.includes(dos)) return false;
            if (date && it.date !== date) return false;
            return true;
        });
        currentPage = 1;
        renderPage();
    }

    function checkResetVisibility() {
        const hasValue =
            (patientInput && patientInput.value.trim() !== '') ||
            (drugInput && drugInput.value.trim() !== '') ||
            (dosageInput && dosageInput.value.trim() !== '') ||
            (dateInput && dateInput.value.trim() !== '');
        if (hasValue) {
            resetBtn.classList.add('show');
        } else {
            resetBtn.classList.remove('show');
        }
    }

    if (filterBtn) filterBtn.addEventListener('click', function (e) { e.preventDefault(); applyFilter(); checkResetVisibility(); });
    if (resetBtn) resetBtn.addEventListener('click', function (e) {
        e.preventDefault();
        if (patientInput) patientInput.value = '';
        if (drugInput) drugInput.value = '';
        if (dosageInput) dosageInput.value = '';
        if (dateInput) dateInput.value = '';
        if (pageSizeSelect) pageSizeSelect.value = '10';
        filteredItems = items.slice();
        currentPage = 1;
        renderPage();
        checkResetVisibility();
    });
    if (pageSizeSelect) pageSizeSelect.addEventListener('change', function () { currentPage = 1; renderPage(); });

    [patientInput, drugInput, dosageInput, dateInput].forEach(inp => {
        if (!inp) return;
        inp.addEventListener('input', checkResetVisibility);
        inp.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                applyFilter();
                checkResetVisibility();
            }
        });
    });

    renderPage();
    checkResetVisibility();
});
