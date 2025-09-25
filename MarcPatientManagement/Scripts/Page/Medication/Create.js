$(function () {
    var form = $("#createForm");

    form.validate({
        invalidHandler: function (event, validator) {
            // Gumamit ng window.Messages.FormErrors
            validator.numberOfInvalids() && showToast(window.Messages.FormErrors, "danger");
        }
    });

    $("#saveBtn").click(function () {
        if (form.valid()) {
            var patient = $("#Patient").val().trim();
            var drug = $("#Drug").val().trim();
            var dosage = parseFloat($("#Dosage").val());
            var date = new Date().toISOString().split('T')[0];

            var patientPattern = /^(?=.*\p{L})[\p{L}\p{M}\s'-]+$/u;
            if (!patientPattern.test(patient)) {
                // Gumamit ng window.Messages.InvalidPatient
                showToast(window.Messages.InvalidPatient, "danger");
                $("#Patient").addClass("input-validation-error");
                return;
            } else {
                $("#Patient").removeClass("input-validation-error");
            }

            var drugPattern = /^[A-Za-z0-9]+(\s+[A-Za-z0-9]+)*$/;
            if (!drugPattern.test(drug) || drug.length > 50) {
                // Gumamit ng window.Messages.InvalidDrug
                showToast(window.Messages.InvalidDrug, "danger");
                $("#Drug").addClass("input-validation-error");
                return;
            } else {
                $("#Drug").removeClass("input-validation-error");
            }

            var dosagePattern = /^\d+(\.\d{1,4})?$/;
            if (isNaN(dosage) || dosage <= 0 || !dosagePattern.test(dosage.toString())) {
                // Gumamit ng window.Messages.InvalidDosage
                showToast(window.Messages.InvalidDosage, "danger");
                $("#Dosage").addClass("input-validation-error");
                return;
            } else {
                $("#Dosage").removeClass("input-validation-error");
            }

            $.post(window.checkDuplicateUrl, { patient, drug, date, dosage }, function (res) {
                $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");

                if (!res.isValid) {
                    const msg = res.message || "Error";
                    showToast(msg, "danger");

                    if (msg.includes(window.Messages.RecordAlreadyExists)) {
                        $("#Patient, #Drug, #Dosage").addClass("input-validation-error");
                    } else if (msg.toLowerCase().includes("cannot add same drug")) {
                        $("#Patient, #Drug").addClass("input-validation-error");
                    }

                    return;
                }

                $("#confirmModal").modal("show");
            });

        } else {
            // Gumamit ng window.Messages.AllFieldsRequired
            showToast(window.Messages.AllFieldsRequired, "danger");
        }
    });

    $("#confirmSave").click(function () {
        form.submit();
    });

    $("#cancelSave").click(function () {
        $("#confirmModal").modal("hide");
    });

    $("#clearAllBtn").click(function () {
        $("#createForm input[type='text'], #createForm input[type='number'], #createForm input[type='date']").val('');
        $("#createForm .input-validation-error").removeClass("input-validation-error");
        $("#PatientError, #DrugError, #DosageError").text('');
    });

    $("#Patient, #Drug, #Dosage").on("input", function () {
        $(this).removeClass("input-validation-error");
    });
});