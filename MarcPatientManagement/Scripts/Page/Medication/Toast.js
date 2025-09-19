function showToast(message, type = "danger")

{
    var alertClass = "alert-info text-dark";

    switch (type.toLowerCase())

    {
        case "success":
            alertClass = "alert-success text-center toast-success-dark";
            break;
        case "warning":
            alertClass = "alert-warning text-dark text-center";
            break;
        case "danger":
            alertClass = "alert-danger text-center";
            break;
    }

    var toast = $(
        '<div class="alert ' + alertClass + ' alert-dismissible fade show" ' +
        'style="position:fixed; top:20px; left:50%; transform:translateX(-50%); min-width:300px; z-index:9999;">' +
        message +
        '</div>'
    );

    $("body").append(toast);

    setTimeout(function ()

    {
        toast.fadeOut("slow", function ()
        {
            $(this).remove();
        });
    }, 2600);
}

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
