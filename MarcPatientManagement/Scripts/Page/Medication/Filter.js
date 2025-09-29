$(document).ready(function () 
{
    const $table = $(".custom-table tbody");
    const $rows = $table.find("tr");

    const $inputs = 
    {
        patient: $("input[name='patientFilter']"),
        drug: $("input[name='drugFilter']"),
        dosage: $("input[name='dosageFilter']"),
        date: $("input[name='modifiedDateFilter']")
    };

    const $resetBtn = $("#resetBtn");

    function toggleResetButton() 
    {
        const hasValue = Object.values($inputs).some($el => $el.val());
        $resetBtn.toggleClass("show", hasValue);
    }

    function flexibleMatch(input, target) 
    {
        if (!input) return true;

        const words = input.split(/\s+/).filter(Boolean);
        target = target.replace(/\s+/g, "");

        return words.every(word => target.includes(word.replace(/\s+/g, "")));
    }

    function filterTable() 
    {
        const patientVal = $inputs.patient.val().toLowerCase();
        const drugVal = $inputs.drug.val().toLowerCase();
        const dosageVal = $inputs.dosage.val().trim();
        const dateVal = $inputs.date.val().trim();

        $rows.each(function () 
        {
            const $row = $(this);

            const patientText = $row.find("td:eq(4)").text().toLowerCase();
            const drugText = $row.find("td:eq(3)").text().toLowerCase();
            const dosageText = $row.find("td:eq(2)").text().trim();
            const dateText = $row.find("td:eq(1)").text().trim();

            const patientMatch = flexibleMatch(patientVal, patientText);
            const drugMatch = flexibleMatch(drugVal, drugText);
            const dosageMatch = dosageVal
                ? dosageText.replace(/\s+/g, "").includes(dosageVal.replace(/\s+/g, ""))
                : true;
            const dateMatch = dateVal ? dateText === dateVal : true;

            $row.toggle(patientMatch && drugMatch && dosageMatch && dateMatch);
        });
    }

    function handleInput() 
    {
        toggleResetButton();
        filterTable();
    }

    $inputs.patient.on("input", handleInput);
    $inputs.drug.on("input", handleInput);

    $inputs.dosage.on("input", function () 
    {
        this.value = this.value.replace(/[^\d\s.]/g, ""); 
        handleInput();
    });

    $inputs.date.on("change input", handleInput);
    $resetBtn.on("click", function () 
    {
        Object.values($inputs).forEach($el => $el.val(""));
        $rows.show();
        toggleResetButton();
    });
});
