$(function (){
    //Define search for provider
    $("#searchValue").bind('input', function () {
        CaseSearch();
    });

    function CaseSearch() {
        var searchBy = $("#searchBy option:selected").val();
        var searchValue = $("#searchValue").val();
        switch (searchBy) {
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
            var value = "";
            if (className == "name") {
                value = $(this).children(classSearch).children().first().prop('innerHTML');
            }
            else {
                value = $(this).children(classSearch).prop('innerHTML');
            }

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
})