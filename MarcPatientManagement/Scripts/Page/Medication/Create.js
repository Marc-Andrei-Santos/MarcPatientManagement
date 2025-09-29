$(function () 
{
    var form = $("#createForm");

    form.validate(
    {
        invalidHandler: function (event, validator) 
        {
            if (validator.numberOfInvalids()) 
            {
                showToast(window.Messages.FormErrors, "danger");
            }
        }
    });

    $("#saveBtn").click(function () 
    {
        var patient = $("#Patient").val().trim();
        var drug = $("#Drug").val().trim();
        var dosageVal = $("#Dosage").val().trim();
        var dosage = parseFloat(dosageVal);
        var date = new Date().toISOString().split('T')[0];

        // Check empty fields
        if (!patient || !drug || !dosageVal) 
        {
            showToast(window.Messages.AllFieldsRequired, "danger");

            if (!patient) 
            {
                $("#Patient").addClass("input-validation-error");
            }
            if (!drug) 
            {
                $("#Drug").addClass("input-validation-error");
            }
            if (!dosageVal) 
            {
                $("#Dosage").addClass("input-validation-error");
            }
            return;
        }

        // Patient validation
        var patientPattern = /^(?=.*\p{L})[\p{L}\p{M}\s'-]+$/u;
        if (!patientPattern.test(patient)) 
        {
            showToast(window.Messages.InvalidPatient, "danger");
            $("#Patient").addClass("input-validation-error");
            return;
        } 
        else if (patient.length > 50) 
        {
            showToast("Patient name cannot exceed 50 characters.", "danger");
            $("#Patient").addClass("input-validation-error");
            return;
        } 
        else 
        {
            $("#Patient").removeClass("input-validation-error");
        }

        // Drug validation
        var drugPattern = /^[\p{L}\p{N} ]+$/u;
        if (!drugPattern.test(drug)) 
        {
            showToast(window.Messages.InvalidDrug, "danger");
            $("#Drug").addClass("input-validation-error");
            return;
        } 
        else if (drug.length > 50) 
        {
            showToast("Drug name cannot exceed 50 characters.", "danger");
            $("#Drug").addClass("input-validation-error");
            return;
        } 
        else 
        {
            $("#Drug").removeClass("input-validation-error");
        }

        // Dosage validation
        var dosagePattern = /^\d{1,3}(\.\d{1,4})?$/;
        if (!dosagePattern.test(dosageVal) || dosage <= 0) 
        {
            showToast(window.Messages.InvalidDosage, "danger");
            $("#Dosage").addClass("input-validation-error");
            return;
        } 
        else 
        {
            $("#Dosage").removeClass("input-validation-error");
        }

        // Duplicate check (AJAX)
        $.post(window.checkDuplicateUrl, { patient: patient, drug: drug, dosage: dosage, date: date }, function (res) 
        {
            $("#Patient, #Drug, #Dosage").removeClass("input-validation-error");

            if (!res.isValid) 
            {
                const msg = res.message || "Error";
                showToast(msg, "danger");

                if (msg.includes(window.Messages.RecordAlreadyExists)) 
                {
                    $("#Patient, #Drug, #Dosage").addClass("input-validation-error");
                } 
                else if (msg.toLowerCase().includes("cannot add same drug")) 
                {
                    $("#Patient, #Drug").addClass("input-validation-error");
                }
                return;
            }

            $("#confirmModal").modal("show");
        });
    });

    $("#confirmSave").click(function () 
    {
        form.submit();
    });

    $("#cancelSave").click(function () 
    {
        $("#confirmModal").modal("hide");
    });

    $("#clearAllBtn").click(function () 
    {
        $("#createForm input[type='text'], #createForm input[type='number'], #createForm input[type='date']").val('');
        $("#createForm .input-validation-error").removeClass("input-validation-error");
        $("#PatientError, #DrugError, #DosageError").text('');
        $("#Dosage").focus();
    });

    $("#Patient, #Drug, #Dosage").on("input", function () 
    {
        $(this).removeClass("input-validation-error");
    });
});
