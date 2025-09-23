$(document).ready(function () {
    const $table = $(".custom-table tbody");
    const $rows = $table.find("tr");
    const $patientInput = $("input[name='patientFilter']");
    const $drugInput = $("input[name='drugFilter']");
    const $dosageInput = $("input[name='dosageFilter']");
    const $dateInput = $("input[name='modifiedDateFilter']");
    const $resetBtn = $("#resetBtn");

    function toggleResetButton() {
        if ($patientInput.val() || $drugInput.val() || $dosageInput.val() || $dateInput.val()) {
            $resetBtn.addClass("show");
        } else {
            $resetBtn.removeClass("show");
        }
    }
    function normalizeSpaces(text) {
        return text.replace(/\s+/g, ' ').trim();
    }

    function flexibleMatch(input, target) {
        if (!input) return true;
        const words = input.split(/\s+/).filter(Boolean);
        target = target.replace(/\s+/g, ''); 
        return words.every(word => {
            const w = word.replace(/\s+/g, '');
            return w && target.includes(w);
        });
    }

    function filterTable() {
        let patientVal = $patientInput.val().toLowerCase();
        let drugVal = $drugInput.val().toLowerCase();
        const dosageVal = $dosageInput.val().trim();
        const dateVal = $dateInput.val().trim();

        $rows.each(function () {
            const $row = $(this);
            let patientText = $row.find("td:eq(4)").text().toLowerCase();
            let drugText = $row.find("td:eq(3)").text().toLowerCase();
            const dosageText = $row.find("td:eq(2)").text().trim();
            const dateText = $row.find("td:eq(1)").text().trim();

            const patientFilter = flexibleMatch(patientVal, patientText);
            const drugFilter = flexibleMatch(drugVal, drugText);
            const dosageMatch = dosageVal ? dosageText.replace(/\s+/g, '').includes(dosageVal.replace(/\s+/g, '')) : true;
            const dateMatch = dateVal ? dateText === dateVal : true;

            if (patientFilter && drugFilter && dosageMatch && dateMatch) {
                $row.show();
            } else {
                $row.hide();
            }
        });
    }


    $patientInput.on("input", function () {
        toggleResetButton();
        filterTable();
    });

    $drugInput.on("input", function () {
        toggleResetButton();
        filterTable();
    });

    $dosageInput.on("input", function () {
        this.value = this.value.replace(/[^\d\s.]/g, "");
        toggleResetButton();
        filterTable();
    });

    $dateInput.on("change input", function () {
        toggleResetButton();
        filterTable();
    });

    $resetBtn.on("click", function () {
        $patientInput.val("");
        $drugInput.val("");
        $dosageInput.val("");
        $dateInput.val("");
        $rows.show();
        toggleResetButton();
    });
});