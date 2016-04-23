$(function () {
    //Clear slected values
    $("#clearFrom").click(function () {
        $("#From").val(null);
        $("#fromCode").val("");
    });

    $("#clearTo").click(function () {
        $("#To").val(null);
        $("#toCode").val("");
    });
})