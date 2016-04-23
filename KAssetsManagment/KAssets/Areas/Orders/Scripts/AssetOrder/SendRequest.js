$(function () {

    //Get user' organisation currency
    $.get("/HelpModule/Currency/GetBaseCurrency").done(function (data) {
        $("#notation").val(data.Notation);
    });

    //Calculate the whole sum of all selected assets
    function CalcTotal() {
        var total = 0.00;
        var assets = $(".assetRow");
        assets.each(function (index, element) {
            var price = parseFloat($(this).children().last().prev().prev().html());
            var course = parseFloat($(this).children().last().prev().prev().children("input").first().val().toString().replace(",", "."));
            total += parseFloat(price * course);
        });

        total = total.toFixed(2);
        $("#total").html(total.toString() + " " + $("#notation").val());
    }

    //---------------------------------------------------------------------------------------------------------
    //Get assets from server and set events to them
    $.get("/Orders/AssetOrder/ChooseAssets").done(function (data) {
        $("#assets").html(data);
        setEventsToAssets();
    });

    //---------------------------------------------------------------------------------------------------------
    //Choose assets event -> close dialog and other
    $('.chooseAssets').click(
        function () {
            $("#assets").removeClass("hidden");
            $("#assets").dialog(
                {
                    width: 500,
                });

            SearcherAsset();

            $("#assets").on('dialogclose', function (event) {
                $("#searchValue").val("");
                $("tr").removeClass("hidden");
                CalcTotal();
            });
            return false;
        });

    //---------------------------------------------------------------------------------------------------------
    //Set events to items -> add asset to main window
    function setEventsToAssets() {
        $("#chooseBtn").click(function () {
            var assets = $(".asset");
            $("#selectedAssets").html("");
            var flag = 0;
            assets.each(function (index, element) {
                if ($(this).is(':checked')) {
                    var inventoryNumber = $(this).parent().parent().children().first().html();
                    var brand = $(this).parent().parent().children().first().next().html();
                    var model = $(this).parent().parent().children().first().next().next().html();
                    var site = $(this).parent().parent().children().first().next().next().next().html();
                    var price = $(this).next().val();
                    var currency = $(this).next().next().val();
                    var currencyCourse = $(this).next().next().next().val();
                    var id = $(this).attr("id");

                    //Create div for asset
                    var div = '<div class="row assetRow"> <div class="col-md-2">' + inventoryNumber + '</div> <div class="col-md-3">' + brand + '</div> <div class="col-md-2">' +
                        model + '</div>' + '<div class="col-md-2">' + site + '</div>  <div class="col-md-2 price">' + price + " " + currency + '<input type="hidden" value="' + currencyCourse + '"/>' + '</div>'
                        + ' <input id="Assets' +
                        flag + '_" name="Assets[' + flag + ']" type="hidden" value="' + id + '"><div class="col-md-1"> <a class="btn btn-danger btn-sm remove" id="' + id + 'f">X</a></div> </div> <hr/>';
                    flag++;
                    $("#selectedAssets").append(div);
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
            $("#assets").dialog('close');

        });
    }

    //------------------------------------------------------------------------------------------
    //Initialize search for item
    function SearcherAsset() {

        $("#searchValue").bind('input', function () {
            CaseSearch();
        });

        function CaseSearch() {
            var searchBy = $("#searchBy option:selected").val();
            var searchValue = $("#searchValue").val();
            switch (searchBy) {
                case "Inventory number": {
                    ShowOnlySearched("inventoryNumber", searchValue)
                    break;
                };
                case "Brand": {
                    ShowOnlySearched("brand", searchValue)
                    break;
                };
                case "Model": {
                    ShowOnlySearched("model", searchValue)
                    break;
                };
                case "Site name": {
                    ShowOnlySearched("site", searchValue)
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


});
