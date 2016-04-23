$(function () {
    // This will make every element with the class "date-picker" into a DatePicker element
    $('.date-picker').datepicker( );
    var currentDate = new Date()
    var day = currentDate.getDate() + 1
    var month = currentDate.getMonth() + 1
    var year = currentDate.getFullYear()
    $('.date-picker').datepicker("setDate", new Date(year, month, day));

})

$(function () {

    if ($(".itm").size() == 0) {
        $("#SelectedProviderOrder").val(null);
    }
    //Change currency click> recaluclate the sum 
    $("#selectCurrency").change(function () {
        CalcTotal();
    });

    //---------------------------------------------------------------------------------------------------------
    //Get items from server and set events to them
    $.get("/Invoices/Invoice/ChooseItems").done(function (data) {
        $("#items").html(data);
        setEventsToAssets();
    });

    //---------------------------------------------------------------------------------------------------------
    //Choose item event -> close dialog and other
    $('.chooseItems').click(
        function () {
            $("#items").removeClass("hidden");
            $("#items").dialog(
                {
                    width: 500,
                });

            SearcherItem();

            $("#items").on('dialogclose', function (event) {
                $("#searchValue").val("");
                $("tr").removeClass("hidden");
            });
            return false;
        });
});

//Calculate the whole sum of all selected items
function CalcTotal() {
    var total = 0.00;
    var items = $(".itemRow");
    items.each(function (index, element) {
        var price = parseFloat($(this).children().last().prev().prev().html());
        var count = parseFloat($(this).children().last().prev().prev().prev().children().first().val());
        var fromCourseCode = $(this).children().last().prev().prev().children().last().prev().html();
        var toCourseId = $("#selectCurrency option:selected").val();

        $.ajax({
            type: "GET",
            url: "/HelpModule/Currency/GetCourseBetween",
            data: { first: fromCourseCode, second: toCourseId },
            success: function (data) {
                // total += (price * count * parseFloat(data))
                total = parseFloat(total) + (price * count * parseFloat(data))
                total = total.toFixed(2);
                $("#total").html(total + "");
            }
        })
    });

    total = total.toFixed(2);
    $("#total").html(total + "");
}

//Set events to items -> add item to main window
function setEventsToAssets() {
    $("#chooseBtn").click(function () {
        var items = $(".item");
        $("#selectedItems").html("");
        var flag = $(".itm").size();
        if (flag < 0) {
            flag = 0;
        }
        items.each(function (index, element) {
            if ($(this).is(':checked')) {
                var brand = $(this).parent().parent().children().first().html();
                var model = $(this).parent().parent().children().first().next().html();
                var price = $(this).parent().children().last().prev().prev().val();
                var currency = $(this).parent().children().last().prev().val();
                var course = $(this).parent().children().last().val();
                var id = $(this).attr("id");

                //Create div for item
                var div = '<div class="row itemRow"> <div class="col-md-3">' + brand + '</div> <div class="col-md-3">' + model + '</div>' + '<div class="col-md-3"><input style="width:30px" value="1" type="number" min="1" class="count"  id="Items_' +
                    flag + '_" name="Items[' + flag + ']"/></div>  <div class="col-md-2 price">' + price + '<span class="currencyCode"> ' + currency + '</span>' +
                    '<input type="hidden" value="' + course + '" />' + '</div> <div> <a class="btn btn-sm btn-danger remove" id="' + id + 'r" >X</a> </div>'
                    + ' <input class="itm" id="ItemIds[' +
                    flag + ']" name="ItemIds[' + flag + ']" type="hidden" value="' + id + '"> </div> <hr/>';
                flag++;
                $("#selectedItems").append(div);
            }
        });
        CalcTotal();
        $(".count").bind('input', function () {
            CalcTotal();
        });

        $(".remove").click(function () {
            var unrealId = $(this).attr("id");
            var realId = unrealId.substring(0, unrealId.length - 1);
            $("#" + realId).prop("checked", false)
            $("#chooseBtn").trigger('click');
        });

        $("#items").dialog('close');

    });

}

//Initialize search for item
function SearcherItem() {

    $("#searchValue").bind('input', function () {
        CaseSearch();
    });

    function CaseSearch() {
        var searchBy = $("#searchBy option:selected").val();
        var searchValue = $("#searchValue").val();
        switch (searchBy) {
            case "Brand": {
                ShowOnlySearched("brand", searchValue)
                break;
            };
            case "Model": {
                ShowOnlySearched("model", searchValue)
                break;
            };
            case "Price": {
                ShowOnlySearched("price", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearched(className, searchValue) {

        var elements = $("tr");
        elements.each(function (index, element) {

            var item = $(this).children("." + className).first();
            var classSearch = "." + className;
            var value = $(this).children(classSearch).prop('innerHTML');

            if (typeof (value) == "string") {

                if (value.indexOf(searchValue) == -1) {
                    item.parent().addClass("hidden");
                }

                if (value.indexOf(searchValue) > -1 || searchValue == "") {
                    item.parent().removeClass("hidden");
                }
            }

        });
    }
}

$(function () {

    //Set provider for choosing to div
    $.get("/Invoices/Invoice/ChooseProvider").done(function (data) {
        $("#providers").html(data);
        setEventsToProviders();
    });



    //Choose provider click
    $('.chooseProvider').click(
        function () {
            $("#providers").removeClass("hidden");
            $("#providers").dialog(
                {
                    width: 500,
                });

            //Clear search value
            $("#providers").on('dialogclose', function (event) {
                $("#searchValueProvider").val("");
                $("tr").removeClass("hidden");
            });

            SearcherProvider();
            return false;
        });
});

//Set events select provider
function setEventsToProviders() {
    $(".provider").click(function () {
        var id = $(this).attr("id").toString();

        var name = $(this).parent().parent().children().first().next().html().trim();
        var email = $(this).parent().parent().children().first().next().next().html().trim();
        var address = $(this).parent().children().last().prev().val();
        var bulstat = $(this).parent().children().last().val();
        $("#SelectedProvider").val(id);
        $("#ProviderEmail").val(email);
        $("#ProviderName").val(name);
        $("#ProviderAddress").val(address);
        $("#ProviderBulstat").val(bulstat);
        $("#providers").dialog('close');

    });
}

//Define provider search
function SearcherProvider() {

    $("#searchValueProvider").bind('input', function () {
        CaseSearch();
    });

    function CaseSearch() {
        var searchBy = $("#searchByProvider option:selected").val();
        var searchValue = $("#searchValueProvider").val();
        switch (searchBy) {
            case "Id": {
                ShowOnlySearched("id", searchValue)
                break;
            };
            case "Name": {
                ShowOnlySearched("name", searchValue)
                break;
            };
            case "Email": {
                ShowOnlySearched("email", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearched(className, searchValue) {

        var elements = $("tr");
        elements.each(function (index, element) {

            var item = $(this).children("." + className).first();
            var classSearch = "." + className;
            var value = $(this).children(classSearch).prop('innerHTML');

            if (typeof (value) == "string") {

                if (value.indexOf(searchValue) == -1) {
                    item.parent().addClass("hidden");
                }

                if (value.indexOf(searchValue) > -1 || searchValue == "") {
                    item.parent().removeClass("hidden");
                }
            }

        });
    }
}

$(function () {

    //Clear provider order value
    $("#clearPO").click(function () {
        $("#SelectedProviderOrder").val(null);
        $("#poItems").html(null);
        CalcTotal();
    });

    //----------------------------------
    //Set provider orders for choosing to div
    $.get("/Invoices/Invoice/ChooseProviderOrder").done(function (data) {
        $("#providerOrders").html(data);
        setEventsToProviderOrders();
    });


    //Choose provider order click event
    $('.chooseProviderOrder').click(
        function () {
            $("#providerOrders").removeClass("hidden");
            $("#providerOrders").dialog(
                {
                    width: 800,
                });

            //Clear provider order values
            $("#providerOrders").on('dialogclose', function (event) {
                $("#searchValueOrder").val("");
                $("tr").removeClass("hidden");
            });

            SearcherProviderOrders();
            return false;
        });
});

//Calculate the whole sum of all selected items
function CalcTotal() {
    var total = 0.00;
    var items = $(".itemRow");
    items.each(function (index, element) {
        var price = parseFloat($(this).children().last().prev().prev().html());
        var count = parseFloat($(this).children().last().prev().prev().prev().children().first().val());
        var fromCourseCode = $(this).children().last().prev().prev().children().last().prev().html();
        var toCourseId = $("#selectCurrency option:selected").val();

        $.ajax({
            type: "GET",
            url: "/HelpModule/Currency/GetCourseBetween",
            data: { first: fromCourseCode, second: toCourseId },
            success: function (data) {

                total = parseFloat(total) + parseFloat(price * count * parseFloat(data));
                total = total.toFixed(2);
                $("#total").html(total.toString());
            }
        })

    });

    total = total.toFixed(2);
    $("#total").html(total.toString());
}

//Choose provider order add items
function setEventsToProviderOrders() {
    $(".order").click(function () {


        var id = $(this).attr("id").toString();
        $("#SelectedProviderOrder").val(id);

        var items = $(this).parent().children().last().children(".orderItem");
        var flag = $(".itm").size();
        if (flag < 0) {
            flag = 0;
        }
        $("#poItems").html("");
        items.each(function (index, element) {
            var brand = $(this).children().first().html();
            var model = $(this).children().first().next().html();
            var price = $(this).children().last().children().first().next().val();
            var currency = $(this).children().last().children().last().prev().prev().val();
            var course = $(this).children().last().children().last().prev().val();
            var quantity = $(this).children().last().children().last().val();
            var id = $(this).children().last().children().first().attr("id");

            //Create div for item
            var div = '<div class="row itemRow"> <div class="col-md-3">' + brand + '</div> <div class="col-md-3">' + model + '</div>' + '<div class="col-md-3"><input style="width:30px" value="' + quantity + '" type="number" min="1" class="" disabled  id="tem_' +
                      flag + '_" name="tem' + flag + ']"/></div>  <div class="col-md-3 price">' + price + '<span class="currencyCode"> ' + currency + '</span>' +
                      '<input type="hidden" value="' + course + '" />' + '</div><div></div>'
                      + ' <input class="" id="tems' +
                      flag + '_" name="temId[' + flag + ']" type="hidden" value="' + id + '"> </div> <hr/>';
            flag++;
            $("#poItems").append(div);
        });
        CalcTotal();
        $(".count").bind('input', function () {
            CalcTotal();
        });



        $("#providerOrders").dialog('close');

        var providerId = $(this).attr("providerId");
        $("#" + providerId).trigger('click');
    });
}

//Define PO search
function SearcherProviderOrders() {

    $("#searchValueOrder").bind('input', function () {
        CaseSearch();
    });

    function CaseSearch() {
        var searchBy = $("#searchByOrder option:selected").val();
        var searchValue = $("#searchValueOrder").val();
        switch (searchBy) {
            case "Id": {
                ShowOnlySearched("id", searchValue)
                break;
            };
            case "From": {
                ShowOnlySearched("name", searchValue)
                break;
            };
            case "Provider": {
                ShowOnlySearched("email", searchValue)
                break;
            };
            case "Date of send": {
                ShowOnlySearched("date", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearched(className, searchValue) {

        var elements = $("tr");
        elements.each(function (index, element) {

            var item = $(this).children("." + className).first();
            var classSearch = "." + className;
            var value = $(this).children(classSearch).prop('innerHTML');

            if (typeof (value) == "string") {

                if (value.indexOf(searchValue) == -1) {
                    item.parent().addClass("hidden");
                }

                if (value.indexOf(searchValue) > -1 || searchValue == "") {
                    item.parent().removeClass("hidden");
                }
            }

        });
    }
}


