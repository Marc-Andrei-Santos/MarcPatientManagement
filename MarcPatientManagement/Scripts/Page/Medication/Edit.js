$(function () {
    function showToast(message) {
        var toast = $('<div class="alert alert-danger" style="position:fixed; top:20px; left:50%; transform:translateX(-50%); min-width:300px; z-index:9999;">' + message + '</div>');
        $("body").append(toast);
        setTimeout(function () {
            toast.fadeOut("slow", function () { $(this).remove(); });
        }, 3000);
    }

    $("#updateBtn").click(function () {
        var form = $("#editForm");
        if (form.valid()) {
            var patient = $("#Patient").val();
            var drug = $("#Drug").val();
            var date = $("#ModifiedDate").val();
            var id = $("#Id").val();
            var dosage = $("#Dosage").val();

            $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");

            if (!patient || !/^[A-Za-z\s\-']+$/.test(patient) || patient !== patient.trim()) {
                showToast(window.Messages.InvalidPatient);
                $("#Patient").addClass("input-validation-error");
                return;
            }
            if (!drug || !/^[A-Za-z0-9\s]+$/.test(drug) || drug !== drug.trim()) {
                showToast(window.Messages.InvalidDrug);
                $("#Drug").addClass("input-validation-error");
                return;
            }
            if (parseFloat(dosage) <= 0 || isNaN(dosage)) {
                showToast(window.Messages.InvalidDosage);
                $("#Dosage").addClass("input-validation-error");
                return;
            }

            $.post(window.checkDuplicateUrl,
                { patient: patient, drug: drug, date: date, id: id, dosage: dosage },
                function (res) {
                    console.log("CheckDuplicate Response:", res);

                    $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");

                    if (!res.isValid) {
                        const msg = res.message;
                        showToast(msg);

                        if (msg === window.Messages.DuplicateRecord) {
                            $("#Patient, #Drug").addClass("input-validation-error");
                        } else if (msg === window.Messages.RecordAlreadyExists) {
                            $("#Patient, #Drug, #Dosage").addClass("input-validation-error");
                        } else if (msg === window.Messages.NoChanges) {
                            $("#Patient, #Drug, #Dosage").addClass("input-validation-error");
                        }
                        return;
                    }
                    $("#confirmUpdateModal").modal("show");

                });
        }
    });

    $("#clearAllBtn").click(function () {
        $("#editForm input[type='text'], #editForm input[type='number'], #editForm input[type='date']").val('');
        $("#editForm .input-validation-error").removeClass("input-validation-error");
    });

    $("#confirmUpdate").click(function () {
        $("#editForm").submit();
    });

    $("#cancelUpdate").click(function () {
        $("#confirmUpdateModal").modal("hide");
    });

    $("#Drug, #Dosage, #Patient").on("input", function () {
        $(this).removeClass("input-validation-error");
    });
});
