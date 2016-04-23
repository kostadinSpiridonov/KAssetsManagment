$(function () {


    //Set assets for choosing to div
    $.get("/AssetsActions/RenovationAsset/ChooseAsset").done(function (data) {
        $("#assets").html(data);
        setEventsToAssets();
    });

    //Choose asset -> open windows and clear search values on closing
    $('.chooseAsset').click(
        function () {
            $("#assets").removeClass("hidden");
            $("#assets").dialog(
                {
                    width: 500
                });
            $("#assets").on('dialogclose', function (event) {
                $("#searchValue").val("");
                $("tr").removeClass("hidden");
            });

            Searcher();
            return false;
        });
});

    //Set events on assets click
    function setEventsToAssets() {
        $(".asset").click(function () {
            var id = $(this).attr("id").toString();

            $("#SelectedAsset").val(id);
            $("#assets").dialog('close');

            $("#assetDetails").html(' <img src="/Content/31.gif" alt="Loading..." />');

            $.get("/AssetsActions/Asset/DetailsPartial/" + id).done(function (data) {
                $("#assetDetails").html(data);
                $("#fromSite").val($("dt:contains('To site')").next().html().toString().trim());
            });

        });
    }

//Define search for assets
function Searcher() {
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