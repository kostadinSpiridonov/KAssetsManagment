$(function () {

    CalcTotal();

    //Change coutn of assets calculate 
    $(".count").click(function () {
        CalcTotal();
    });

    //Calculate the whole sum of all selected items
    function CalcTotal() {
        var total = 0.00;
        var items = $(".itemRow");
        items.each(function (index, element) {
            if ($(this).children().last().prev().prev().children().first().is(':checked')) {
                var price = parseFloat($(this).children().last().prev().prev().prev().html());
                var course = parseFloat($(this).children().last().val().toString().replace(",", "."));
                total += parseFloat(price * course);
            }
        });

        //Get the currency of user' organisation
        $.get("/HelpModule//Currency/GetBaseCurrency").done(function (data) {
            total = total.toFixed(2);
            $("#total").html(total.toString() + " " + data.Notation);
        });

    }
})