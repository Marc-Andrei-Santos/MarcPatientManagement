$(function () {
    var form = $("#createForm");

    form.validate({
        invalidHandler: function (event, validator) {
            validator.numberOfInvalids() && showToast(window.Messages.FormErrors || "Please correct the errors in the form.", "danger");
        }
    });

    $("#saveBtn").click(function () {
        if (form.valid()) {
            var patient = $("#Patient").val().trim();
            var drug = $("#Drug").val().trim();
            var dosage = parseFloat($("#Dosage").val());
            var date = new Date().toISOString().split('T')[0];

            var patientPattern = /^(?=.*\p{L})[\p{L}\p{M}\s'-]+$/u;
            if (!patientPattern.test(patient) || patient.startsWith(" ") || patient.endsWith(" ")) {
                showToast(window.Messages.InvalidPatient || "Invalid patient name", "danger");
                $("#Patient").addClass("input-validation-error");
                return;
            } else {
                $("#Patient").removeClass("input-validation-error");
            }

            var drugPattern = /^[A-Za-z0-9]+(\s[A-Za-z0-9]+)*$/;
            if (!drugPattern.test(drug) || drug.length > 50) {
                showToast(window.Messages.InvalidDrug || "Invalid drug name", "danger");
                $("#Drug").addClass("input-validation-error");
                return;
            } else {
                $("#Drug").removeClass("input-validation-error");
            }

            var dosagePattern = /^\d+(\.\d{1,4})?$/;
            if (isNaN(dosage) || dosage <= 0 || !dosagePattern.test(dosage.toString())) {
                showToast(window.Messages.InvalidDosage || "Invalid dosage", "danger");
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
            showToast(window.Messages.AllFieldsRequired || "All fields are required", "danger");
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
