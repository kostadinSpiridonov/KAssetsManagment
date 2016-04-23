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

    var chooseOption = 0;

    //Set currency for choosing
    var orgId = $("#orgId").val();
    $.get("/HelpModule/ExchangeRate/ChooseCurrency/"+orgId).done(function (data) {
        $("#currencyD").html(data);
        setEventsToCurrency();

        if ($("#From").val() != "") {
            var id = $("#From").val();
            var currency = $("#" + id);
            $("#fromCode").val($.trim(currency.parent().prev().prev().html()));
        }

        if ($("#To").val() != "") {
            var id = $("#To").val();
            var currency = $("#" + id);
            $("#toCode").val($.trim(currency.parent().prev().prev().html()));
        }
    });

    $("#selectOrganisation").change(function () {
        var id = $(this).val();
        $.get("/HelpModule/ExchangeRate/ChooseCurrency/" + id).done(function (data) {
            $("#currencyD").html(data);
            setEventsToCurrency();

            if ($("#From").val() != "") {
                var id = $("#From").val();
                var currency = $("#" + id);
                $("#fromCode").val($.trim(currency.parent().prev().prev().html()));
            }

            if ($("#To").val() != "") {
                var id = $("#To").val();
                var currency = $("#" + id);
                $("#toCode").val($.trim(currency.parent().prev().prev().html()));
            }
        });

    });

    //Choose from currency click open window
    $('#chooseFrom').click(
        function () {
            chooseOption = 1;
            $("#currencyD").removeClass("hidden");
            $("#currencyD").dialog(
                {
                    width: 500,
                });

            $("#currencyD").on('dialogclose', function (event) {
                $("#search").val("");
                $("tr").removeClass("hidden");
            });

            SearcherCurrency();
            return false;
        });


    //Choose to currency click open window
    $('#chooseTo').click(
       function () {
           chooseOption = 2;
           $("#currencyD").removeClass("hidden");
           $("#currencyD").dialog(
               {
                   width: 500,
               });

           $("#currencyD").on('dialogclose', function (event) {
               $("#search").val("");
               $("tr").removeClass("hidden");
           });

           SearcherCurrency();
           return false;
       });

    //Set event click choose currency
    function setEventsToCurrency() {
        $(".currency").click(function () {

            var id = $(this).attr("id").toString();

            //if is for to
            if (chooseOption === 1) {
                $("#From").val(id);
                $("#fromCode").val($.trim($(this).parent().prev().prev().html()));
            }

            //if is for from
            if (chooseOption == 2) {
                $("#To").val(id);
                $("#toCode").val($.trim($(this).parent().prev().prev().html()));
            }
            $("#currencyD").dialog('close');
        });
    }

    //Define search
    function SearcherCurrency() {

        $("#searchValue").bind('input', function () {
            CaseSearch();
        });

        function CaseSearch() {
            var searchBy = $("#searchBy option:selected").val();
            var searchValue = $("#searchValue").val();
            switch (searchBy) {
                case "Id": {
                    ShowOnlySearched("id", searchValue)
                    break;
                };
                case "Code": {
                    ShowOnlySearched("code", searchValue)
                    break;
                };
                case "Description": {
                    ShowOnlySearched("description", searchValue)
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
})