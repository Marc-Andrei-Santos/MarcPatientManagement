$(function () {

    function showToast(message) {
        var toast = $('<div class="alert alert-danger" style="position:fixed; top:20px; left:50%; transform:translateX(-50%); min-width:300px; z-index:9999;">' + message + '</div>');
        $("body").append(toast);
        setTimeout(function () {
            toast.fadeOut("slow", function () { $(this).remove(); });
        }, 4000);
    }

    $("#updateBtn").click(function () {
        var form = $("#editForm");

        if (form.valid()) {
            var dosage = parseFloat($("#Dosage").val());
            if (isNaN(dosage) || dosage <= 0 || dosage > 999.9999) {
                showToast("Please enter a valid dosage.");
                $("#Dosage").addClass("input-validation-error");
                return;
            } else {
                $("#Dosage").removeClass("input-validation-error");
            }

            var patient = $("#Patient").val();
            var drug = $("#Drug").val();
            var date = $("#ModifiedDate").val();
            var id = $("#Id").val();

            $.post(window.checkDuplicateUrl,
                { patient: patient, drug: drug, date: date, id: id },
                function (res) {
                    if (res.isDuplicate) {
                        showToast(res.message);
                        $("#Drug").addClass("input-validation-error");
                    } else {
                        $("#Drug").removeClass("input-validation-error");
                        $("#confirmUpdateModal").modal("show");
                    }
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

    $("#Drug, #Dosage").on("input", function () {
        $(this).removeClass("input-validation-error");
    });
});
