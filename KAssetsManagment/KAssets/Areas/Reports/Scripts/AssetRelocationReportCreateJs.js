$(function () {
    //Set assets for choosing to div
    $.get("/Reports/AssetRelocationsReport/ChooseAsset").done(function (data) {
        $("#assets").html(data);
        setEventsToAssets();
    });

    //Select asset -> open window and clear search values
    $('.chooseAsset').click(
        function () {
            $("#assets").removeClass("hidden");
            $("#assets").dialog(
                {
                    width: 500,
                });

            $("#assets").on('dialogclose', function (event) {
                $("#searchValue").val("");
                $("tr").removeClass("hidden");
            });

            SearcherAsset();
            return false;
        });


    //Set event to asset click
    function setEventsToAssets() {
        $(".asset").click(function () {
            var id = $(this).attr("id").toString();

            $("#SelectedAsset").val(id);
            $("#assets").dialog('close');

            $("#assetDetails").html(' <img src="/Content/31.gif" alt="Loading..." />');

            $.get("/AssetsActions/Asset/DetailsPartial/" + id).done(function (data) {
                $("#assetDetails").html(data);
                $("#fromSite").val($("dt:contains('To site')").next().html().toString().trim());
                $("#fromUser").val($("dt:contains('User')").next().html().toString().trim());
            });

            var locationCode = $(this).parent().children().last().val();
            $("#fromLocation").val(locationCode);

        });
    }

    //Define search for asset
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
