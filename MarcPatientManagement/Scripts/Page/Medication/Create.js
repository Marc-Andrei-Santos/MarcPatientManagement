$(function () {
    $("#saveBtn").click(function () {
        var form = $("#createForm");
        if (!form.valid()) {
            return;
        }

        var patient = $("#Patient").val();
        var drug = $("#Drug").val();
        var dosage = parseFloat($("#Dosage").val());
        var date = new Date().toISOString().split('T')[0];

        var patientPattern = /^(?=.*[A-Za-z])[A-Za-z\s'-]+$/;
        if (!patientPattern.test(patient) || patient.startsWith(" ") || patient.endsWith(" ")) {
            showToast(window.Messages.InvalidPatient, "danger");
            $("#Patient").addClass("input-validation-error");
            return;
        } else {
            $("#Patient").removeClass("input-validation-error");
        }

        var drugPattern = /^[A-Za-z0-9]+(\s[A-Za-z0-9]+)*$/;
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

        $.post(window.checkDuplicateUrl,
            { patient: patient, drug: drug, date: date, dosage: dosage },
            function (res) {
                if (res.isDuplicate) {
                    const msg = res.message;
                    showToast(msg, "danger");

                    $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");

                    if (msg === window.Messages.DuplicateRecord) {
                        $("#Patient, #Drug").addClass("input-validation-error");
                    } else if (msg === window.Messages.RecordAlreadyExists) {
                        $("#Patient, #Drug, #Dosage").addClass("input-validation-error"); 
                    } else if (msg === window.Messages.NoChanges) {
                        $("#Patient, #Drug, #Dosage").addClass("input-validation-error");
                    }
                } else {
                    $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");
                    $("#confirmModal").modal("show");
                }
            });

    });

    $("#confirmSave").click(function () {
        $("#createForm").submit();
    });

    $("#cancelSave").click(function () {
        $("#confirmModal").modal("hide");
    });

    $("#Patient, #Drug, #Dosage").on("input", function () {
        $(this).removeClass("input-validation-error");
    });
});
