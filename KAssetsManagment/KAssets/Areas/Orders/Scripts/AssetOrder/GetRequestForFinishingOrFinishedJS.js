$(function () {

    CalcTotal();

    //Calculate the whole sum of all selected items
    function CalcTotal() {
        var total = 0.00;
        var items = $(".itemRow");
        items.each(function (index, element) {
            var price = parseFloat($(this).children().last().prev().html());
            var course = parseFloat($(this).children().last().val().toString().replace(",", "."));
            console.log(price)
            total += parseFloat(price * course);
        });

        //Get currency of user' organisation
        $.get("/HelpModule/Currency/GetBaseCurrency").done(function (data) {
            total = total.toFixed(2);
            $("#total").html(total.toString() + " " + data.Notation);
        });

    }
})