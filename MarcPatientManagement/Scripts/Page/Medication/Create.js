$(function () {

    function showToast(message) {
        var toast = $('<div class="alert alert-danger" style="position:fixed; top:20px; left:50%; transform:translateX(-50%); min-width:300px; z-index:9999;">' + message + '</div>');
        $("body").append(toast);
        setTimeout(function () {
            toast.fadeOut("slow", function () { $(this).remove(); });
        }, 3000);
    }

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
            showToast("Invalid Patient name.");
            $("#Patient").addClass("input-validation-error");
            return;
        } else {
            $("#Patient").removeClass("input-validation-error");
        }

        var drugPattern = /^[A-Za-z0-9]+(\s[A-Za-z0-9]+)*$/;
        if (!drugPattern.test(drug) || drug.length > 50) {
            showToast("Invalid Drug name.");
            $("#Drug").addClass("input-validation-error");
            return;
        } else {
            $("#Drug").removeClass("input-validation-error");
        }

        var dosagePattern = /^\d+(\.\d{1,4})?$/;
        if (isNaN(dosage) || dosage <= 0 || !dosagePattern.test(dosage.toString())) {
            showToast("Invalid Dosage.");
            $("#Dosage").addClass("input-validation-error");
            return;
        } else {
            $("#Dosage").removeClass("input-validation-error");
        }

        $.post(window.checkDuplicateUrl,
            { patient: patient, drug: drug, date: date },
            function (res) {
                if (res.isDuplicate) {
                    showToast(res.message);
                    $("#Drug").addClass("input-validation-error");
                } else {
                    $("#Drug").removeClass("input-validation-error");
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
