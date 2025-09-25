$(function ()

{
    var deleteForm = $("#deleteForm");

    $(".delete-btn").click(function () {
        var id = $(this).data("id");
        deleteForm.attr("action", "/Medication/Medication/DeleteConfirmed/" + id);
        $("#deleteModal").modal("show");
    });

    $("#cancelDeleteBtn").click(function () {
        $("#deleteModal").modal("hide");
    });
});
