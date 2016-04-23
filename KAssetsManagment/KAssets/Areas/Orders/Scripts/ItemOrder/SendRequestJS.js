$(function () {

    $(".clearUser").click(function () {
        $(".selectedUser").val(null);
        $(".chooseLocation").removeClass("hidden");
    });

    $(".clearLocation").click(function () {
        $(".selectedLocation").val(null);
    });

    //---------------------------------------------------------------------------------------------------
    //When user select a user from drop down automaticly the location of selected user is select in location drop down
    //and location drop down become disable
    $("#userDropDown").change(function () {

        var id = $("#userDropDown").val();

        if (id != "") {

            $.getJSON("/Account/GetUserLocationId/" + id, null, function (data) {
                $("#locationDropDown").val(data);
                $(".chooseLocation").addClass("hidden");
            });
        }
        else {
            $("#locationDropDown").val(null);
            $(".chooseLocation").removeClass("hidden");
        }
    });

    //---------------------------------------------------------------------------------------------------------
    //Get items from server and set events to them
    $.get("/Orders/ItemOrder/ChooseItems").done(function (data) {
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
                CalcTotal();
            });
            return false;
        });


    //---------------------------------------------------------------------------------------------------------
    //Get items from server and set events to them
    $.get("/Orders/ItemOrder/ChooseLocations").done(function (data) {
        $("#locations").html(data);
        setEventsToLocation();
    });

    //---------------------------------------------------------------------------------------------------------
    //Choose location event -> close dialog and other
    $('.chooseLocation').click(
        function () {
            $("#locations").removeClass("hidden");
            $("#locations").dialog(
                {
                    width: 800,
                });

            SearcherLocation();

            $("#locations").on('dialogclose', function (event) {
                $("#searchValueLocation").val("");
                $("tr").removeClass("hidden");
            });
            return false;
        });


    //---------------------------------------------------------------------------------------------------------
    //Get items from server and set events to them
    $.get("/Orders/ItemOrder/ChooseUser").done(function (data) {
        $("#users").html(data);
        setEventsToUser();
    });

    //---------------------------------------------------------------------------------------------------------
    //Choose location event -> close dialog and other
    $('.chooseUser').click(
        function () {
            $("#users").removeClass("hidden");
            $("#users").dialog(
                {
                    width: 500,
                });

            SearcherUser();

            $("#users").on('dialogclose', function (event) {
                $("#searchValueUser").val("");
                $("tr").removeClass("hidden");
            });
            return false;
        });

});

//Set events to locations -> add location to main window
function setEventsToUser() {
    $(".user").click(function () {
        $("#users").dialog('close');
        var id = $(this).attr("id");
        $(".selectedUser").val(id);
        $(".selectedUser").change();
    });
}

//Initialize search for user
function SearcherUser() {

    $("#searchValueUser").bind('input', function () {
        CaseSearchUser();
    });

    function CaseSearchUser() {
        var searchBy = $("#searchByUser option:selected").val();
        var searchValue = $("#searchValueUser").val();
        switch (searchBy) {
            case "Name": {
                ShowOnlySearchedUser("name", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearchedUser(className, searchValue) {

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

//Set events to locations -> add location to main window
function setEventsToLocation() {
    $(".location").click(function () {
        $("#locations").dialog('close');
        var id = $(this).attr("id");
        $(".selectedLocation").val(id);

    });
}

//Initialize search for location
function SearcherLocation() {

    $("#searchValueLocation").bind('input', function () {
        CaseSearchLocation();
    });

    function CaseSearchLocation() {
        var searchBy = $("#searchByLocation option:selected").val();
        var searchValue = $("#searchValueLocation").val();
        switch (searchBy) {
            case "Code": {
                ShowOnlySearchedLocation("code", searchValue)
                break;
            };
            case "Latitude": {
                ShowOnlySearchedLocation("latitude", searchValue)
                break;
            };
            case "Longitude": {
                ShowOnlySearchedLocation("longitude", searchValue)
                break;
            };
            case "Country": {
                ShowOnlySearchedLocation("country", searchValue)
                break;
            };
            case "Town": {
                ShowOnlySearchedLocation("town", searchValue)
                break;
            };
            case "Street": {
                ShowOnlySearchedLocation("street", searchValue)
                break;
            };
            case "Street number": {
                ShowOnlySearchedLocation("streetNumber", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearchedLocation(className, searchValue) {

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
            case "Organisation": {
                ShowOnlySearched("organisation", searchValue)
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

//Set events to items -> add item to main window
function setEventsToAssets() {
    $("#chooseBtn").click(function () {
        var items = $(".item");
        $("#selectedItems").html("");
        var flag = 0;
        items.each(function (index, element) {
            if ($(this).is(':checked')) {
                var brand = $(this).parent().parent().children().first().html();
                var model = $(this).parent().parent().children().first().next().html();
                var organisation = $(this).parent().parent().children().first().next().next().html();
                var price = $(this).parent().children().last().prev().prev().val();
                var currency = $(this).parent().children().last().prev().val();
                var course = $(this).parent().children().last().val();
                var id = $(this).attr("id");

                //Create div for item
                var div = '<div class="row itemRow"> <div class="col-md-3">' + brand + '</div> <div class="col-md-3">' + model + '</div> <div class="col-md-2">' +
                    organisation + '</div>' + '<div class="col-md-1"><input style="width:30px" value="1" type="number" min="1" class="count"  id="Items_' +
                    flag + '_" name="Items[' + flag + '].Count"/></div>  <div class="col-md-2 price">' + price + '<span> ' + currency + '</span>' +
                    '<input type="hidden" value="' + course + '" />' + '</div>'
                    + ' <input id="Items_' +
                    flag + '_" name="Items[' + flag + '].Id" type="hidden" value="' + id + '"> <div class="col-md-1"> <a class="btn btn-danger btn-sm remove" id="' + id + 'f">X</a></div> </div> <hr/>';
                flag++;
                $("#selectedItems").append(div);
            }
        });

        CalcTotal();
        $(".remove").click(function () {
            var unrealId = $(this).attr("id");
            var realId = unrealId.substring(0, unrealId.length - 1);
            $("#" + realId).prop("checked", false)
            $("#chooseBtn").trigger('click');
        });

        $(".count").bind('input', function () {
            CalcTotal();
        });
        $("#items").dialog('close');

    });
}

//Calculate the whole sum of all selected items
function CalcTotal() {
    var total = 0.00;
    var items = $(".itemRow");
    items.each(function (index, element) {
        var price = parseFloat($(this).children().last().prev().prev().html());
        var count = parseFloat($(this).children().last().prev().prev().prev().children().first().val());
        var course = parseFloat($(this).children().last().prev().prev().children().last().val().toString().replace(",", "."));
        total += parseFloat(price * count * course);

    });
    total = total.toFixed(2);
    $.get("/HelpModule/Currency/GetBaseCurrency").done(function (data) {
        $("#total").html(total.toString() + " " + data.Notation);
    });
}