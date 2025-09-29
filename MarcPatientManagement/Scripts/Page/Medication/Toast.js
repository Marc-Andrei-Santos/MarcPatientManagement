(function () 
{
    window.showToast = function (message, type) 
    {
        type = (type || "danger").toLowerCase();

        var alertClass = "alert-info text-dark fw-semibold"; 
        switch (type) 
        {
            case "success":
                alertClass = "alert-success text-center toast-success-dark fw-semibold";
                break;
            case "warning":
                alertClass = "alert-warning text-center toast-warning-dark fw-semibold";
                break;
            case "danger":
            default:
                alertClass = "alert-danger text-center fw-semibold";
                break;
        }

        var uniqueId = "toastMessage_" + Date.now();

        var $toast = $(
            '<div id="' + uniqueId + '" class="alert ' + alertClass + ' alert-dismissible fade show" ' +
            'style="position:fixed; top:20px; left:50%; transform:translateX(-50%); min-width:300px; z-index:9999;">' +
            message +
            '</div>'
        );



        $("body").append($toast);

        setTimeout(function () 
        {
            $toast.fadeOut("slow", function () 
            {
                $(this).remove();
            });
        }, 2600);
    };

    $(function () 
    {
        var serverToast = $("#toastMessage");
        if (serverToast.length) 
        {
            setTimeout(function () 
            {
                serverToast.fadeOut("slow", function () 
                {
                    $(this).remove();
                });
            }, 2600);
        }
    });
})();
