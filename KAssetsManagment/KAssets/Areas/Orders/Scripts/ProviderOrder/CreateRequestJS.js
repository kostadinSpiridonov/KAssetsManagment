
$(function () {
    //Clear provider value
    $(".clearProvider").click(function () {
        $("#Provider").val(null);
    });

    //get provider from server and put them in div for searching
    $.get("/Orders/ProviderOrder/ChooseProviders").done(function (data) {
        $("#providers").html(data);
        setEventsToProvider();
    });

    //Choose provider click
    $('.chooseProvider').click(
        function () {
            $("#providers").removeClass("hidden");
            $("#providers").dialog(
                {
                    width: 540,
                });

            $("#providers").on('dialogclose', function (event) {
                $("#searchValueProvider").val("");
                $("tr").removeClass("hidden");
            });

            SearcherProvider();
            return false;
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
            });
            return false;
        });

})

//Set events to provider
function setEventsToProvider() {
    $(".provider").click(function () {
        var id = $(this).attr("id").toString();

        $("#Provider").val(id);
        $("#providers").dialog('close');
    });
}

//Define search for provider
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
            case "Phone": {
                ShowOnlySearched("phone", searchValue)
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

//---------------------------------------------------------------------------------------------------------
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
                var price = $(this).parent().children().last().val();
                var id = $(this).attr("id");

                //Create div for item
                var div = '<div class="row itemRow"> <div class="col-md-3">' + brand + '</div> <div class="col-md-3">' + model + '</div> <div class="col-md-3">' +
                    organisation + '</div>' + '<div class="col-md-1"><input style="width:30px" value="1" type="number" min="1" class="count"  id="ItemsAndCount' +
                    flag + '__Count" name="ItemsAndCount[' + flag + '].Count"/></div>'
                    + ' <input id="ItemsAndCount_' +
                    flag + '__Id" name="ItemsAndCount[' + flag + '].Id" type="hidden" value="' + id + '"><div class="col-md-1"> <a class="btn btn-danger btn-sm remove" id="' + id +
                    'f">X</a></div> </div> <hr/>';
                flag++;
                $("#selectedItems").append(div);
            }
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

//------------------------------------------------------------------------------------------
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