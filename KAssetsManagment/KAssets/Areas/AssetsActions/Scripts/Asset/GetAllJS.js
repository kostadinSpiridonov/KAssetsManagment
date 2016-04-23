$(function () {

    function readCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    //Show all assets, not only from your organisation
    $("#showAll").click(function () {
        var cook = readCookie("_lang")
        if ($(this).attr("value") == "Show all") {
            $(".other").removeClass("hidden");
            $(this).attr("value", "Hide others");
            if (cook == "en") {
                $(this).html("Hide others")
            }
            else if (cook == "bg") {
                $(this).html("Скрий другите")
            }
            CaseSearch();
        }
        else if ($(this).attr("value") == "Hide others") {
            $(".other").addClass("hidden");
            $(this).attr("value", "Show all");
            if (cook == "en") {
                $(this).html("Show all")
            }
            else if (cook == "bg") {
                $(this).html("Покажи другите")
            }
        }
    })

        //Search assets
    $("#searchValue").bind('input', function () {
        CaseSearch();
    });
    })

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