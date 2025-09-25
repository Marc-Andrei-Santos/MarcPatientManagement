$(function () {
    var form = $("#editForm");

    form.validate({
        invalidHandler: function (event, validator) {
            validator.numberOfInvalids() && showToast(window.Messages.FormErrors, "danger");
        }
    });

    $("#updateBtn").click(function () {
        if (form.valid()) {
            var patient = $("#Patient").val().trim();
            var drug = $("#Drug").val().trim();
            var dosage = parseFloat($("#Dosage").val());
            var id = $("#Id").val();
            var date = new Date().toISOString().split('T')[0];

            var patientPattern = /^(?=.*\p{L})[\p{L}\p{M}\s'-]+$/u;
            if (!patientPattern.test(patient)) {
                showToast(window.Messages.InvalidPatient, "danger");
                $("#Patient").addClass("input-validation-error");
                return;
            } else {
                $("#Patient").removeClass("input-validation-error");
            }

            var drugPattern = /^(?=.*[A-Za-z0-9])[A-Za-z0-9\s'-]+$/;

            if (!drugPattern.test(drug) || drug.length > 50) {
                showToast(window.Messages.InvalidDrug, "danger");
                $("#Drug").addClass("input-validation-error");
                return;
            } else {
                $("#Drug").removeClass("input-validation-error");
            }

            var dosagePattern = /^\d+(\.\d{1,4})?$/;
            if (isNaN(dosage) || dosage <= 0 || !dosagePattern.test(dosage.toString())) {
                showToast(window.Messages.InvalidDosage, "danger");
                $("#Dosage").addClass("input-validation-error");
                return;
            } else {
                $("#Dosage").removeClass("input-validation-error");
            }

            $.post(window.checkDuplicateUrl, { patient, drug, date, id, dosage }, function (res) {
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

                $("#confirmUpdateModal").modal("show");
            });

        } else {
            showToast(window.Messages.AllFieldsRequired, "danger");
        }
    });

    $("#confirmUpdate").click(function () {
        form.submit();
    });

    $("#cancelUpdate").click(function () {
        $("#confirmUpdateModal").modal("hide");
    });

    $("#clearAllBtn").click(function () {
        $("#editForm input[type='text'], #editForm input[type='number'], #editForm input[type='date']").val('');
        $("#editForm .input-validation-error").removeClass("input-validation-error");
        $("#PatientError, #DrugError, #DosageError").text('');
    });

    $("#Patient, #Drug, #Dosage").on("input", function () {
        $(this).removeClass("input-validation-error");
    });
});