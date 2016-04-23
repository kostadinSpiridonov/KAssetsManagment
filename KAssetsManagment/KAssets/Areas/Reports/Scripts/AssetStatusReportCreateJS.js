$(function () {

    //Search assets
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
                ShowOnlySearched("siteName", searchValue)
                break;
            }
        }
    }

    function ShowOnlySearched(className, searchValue) {

        var elements = $("tr");
        elements.each(function (index, element) {

            var item = $(this).children("." + className).first();
            var classSearch = "." + className;
            var value = "";
            if (className == "inventoryNumber") {
                value = $(this).children(classSearch).children().first().prop('innerHTML');
            }
            else {
                value = $(this).children(classSearch).prop('innerHTML');
            }

            if (typeof (value) == "string") {
                if ($("#showAll").html() == "Show all") {

                    if (value.indexOf(searchValue) == -1) {
                        item.parent().addClass("hidden");
                    }

                    if (value.indexOf(searchValue) > -1 || searchValue == "") {
                        if (!item.parent().hasClass("other")) {
                            item.parent().removeClass("hidden");
                        }
                    }

                }
                else {
                    if (value.indexOf(searchValue) == -1) {
                        item.parent().addClass("hidden");
                    }

                    if (value.indexOf(searchValue) > -1 || searchValue == "") {
                        item.parent().removeClass("hidden");
                    }
                }
            }

        });
    }
})