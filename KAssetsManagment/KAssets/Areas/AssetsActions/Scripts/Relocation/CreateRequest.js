
//Define search for locations
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
            case "Latitude": {
                ShowOnlySearchedLocation("latitude", searchValue)
                break;
            };
            case "Longitude": {
                ShowOnlySearchedLocation("longitude", searchValue)
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

//Define search for users
function SearcherUser() {
    $("#searchValueUser").bind('input', function () {
        CaseSearchUser();
    });

    function CaseSearchUser() {
        var searchBy = $("#searchByUser option:selected").val();
        var searchValue = $("#searchValueUser").val();
        switch (searchBy) {
            case "Id": {
                ShowOnlySearchedUser("id", searchValue)
                break;
            };
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

//Set events to location choosing
function setEventsToLocations() {
    $(".location").click(function () {
        var id = $(this).attr("id").toString();

        $("#ToLocation").val(id);
        $("#locations").dialog('close');

        $(".chooseUser").addClass("hidden");


    });
}

//Define search for sites
function SearcherSite() {
    $("#searchValueSite").bind('input', function () {
        CaseSearchSite();
    });

    function CaseSearchSite() {
        var searchBy = $("#searchBySite option:selected").val();
        var searchValue = $("#searchValueSite").val();
        switch (searchBy) {
            case "Name": {
                ShowOnlySearchedSite("name", searchValue)
                break;
            };
        }
    }

    function ShowOnlySearchedSite(className, searchValue) {

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

//Set events to user choose
function setEventsToUsers() {
    $(".user").click(function () {
        var id = $(this).attr("id").toString();

        $("#ToUser").val(id);
        $("#users").dialog('close');


        $(".chooseLocation").addClass("hidden");

        if ($("#ToSite").val() == "") {
            $(".chooseSite").addClass("hidden");
        }

        var userName = $(this).parent().parent().children().last().prev().html().toString().trim();
        $("#userName").val(userName);

        $("#userName").change();

        //Set user site is selected
        $.get("/AssetsActions/RelocationAsset/GetUserSiteId/" + id).done(function (data) {
            $("#ToSite").val(data);
            var siteName = $("#" + data).parent().parent().children().first().html().toString().trim();
            $("#siteName").val(siteName);
        }).fail(function () {

            $("#ToSite").val(null);
            $("#siteName").val("");
        });

        //Set user location is selected
        $.get("/AssetsActions/RelocationAsset/GetUserLocationId/" + id).done(function (data) {
            $("#ToLocation").val(data);
        }).fail(function () {
            $("#ToLocation").val(null);
        });
    });
}

//Site choose se events
function setEventsToSites() {
    $(".site").click(function () {
        var id = $(this).attr("id").toString();

        $("#ToSite").val(id);
        $("#siteName").val($(this).parent().prev().html().toString().trim());
        $("#sites").dialog('close');

        $.get("/AssetsActions/RelocationAsset/ChooseUsersFromSite/" + id).done(function (data) {
            $("#users").html(data);
            setEventsToUsers();
        });
    });


}

//Set event to asset click
function setEventsToAssets() {
    $(".asset").click(function () {
        var id = $(this).attr("id").toString();

        $("#SelectedAsset").val(id);
        $("#assets").dialog('close');

        $("#assetDetails").html(' <img src="/Content/31.gif" alt="Loading..." />');

        $.get("/AssetsActions/Asset/DetailsPartial/" + id).done(function (data) {
            $("#assetDetails").html(data);
            $("#fromSite").val($(".toSiteJS").next().html().toString().trim());
            $("#fromUser").val($(".userJS").next().html().toString().trim());
        });

        var locationCode = $(this).parent().children().last().val();
        $("#fromLocation").val(locationCode);

        //Set users for choosing to div
        $.get("/AssetsActions/RelocationAsset/ChooseUsers/" + id).done(function (data) {
            $("#users").html(data);
            setEventsToUsers();
        });

        //Set location for choosing to div
        $.get("/AssetsActions/RelocationAsset/ChooseLocation/" + id).done(function (data) {
            $("#locations").html(data);
            setEventsToLocations();
        });

        //Set sites for choosing to div
        $.get("/AssetsActions/RelocationAsset/GetAllPartial/" + id).done(function (data) {
            $("#sites").html(data);
            setEventsToSites();
        });

    });
}

$(function () {

    //Set assets for choosing to div
    $.get("/AssetsActions/RelocationAsset/ChooseAsset").done(function (data) {
        $("#assets").html(data);
        setEventsToAssets();
    });

    //Set sites for choosing to div
    $.get("/AssetsActions/RelocationAsset/GetAllPartial").done(function (data) {
        $("#sites").html(data);
        setEventsToSites();
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




    //Choose site -> open window and clear search values
    $('.chooseSite').click(
   function () {
       $("#sites").removeClass("hidden");
       $("#sites").dialog(
           {
               width: 500
           });

       $("#sites").on('dialogclose', function (event) {
           $("#searchValueSite").val("");
           $("tr").removeClass("hidden");
       });
       SearcherSite();
       return false;
   });



    //Set users for choosing to div
    $.get("/AssetsActions/RelocationAsset/ChooseUsers").done(function (data) {
        $("#users").html(data);
        setEventsToUsers();
    });

    //Choose user -> open window and clear search values
    $('.chooseUser').click(
        function () {
            $("#users").removeClass("hidden");
            $("#users").dialog(
                {
                    width: 500,
                });

            $("#users").on('dialogclose', function (event) {
                $("#searchValueUser").val("");
                $("tr").removeClass("hidden");
            });

            SearcherUser();
            return false;
        });

    //Set location for choosing to div
    $.get("/AssetsActions/RelocationAsset/ChooseLocation").done(function (data) {
        $("#locations").html(data);
        setEventsToLocations();
    });

    //Choose location -> open window and clear search values
    $('.chooseLocation').click(
        function () {
            $("#locations").removeClass("hidden");
            $("#locations").dialog(
                {
                    width: 760,
                });

            $("#locations").on('dialogclose', function (event) {
                $("#searchValueLocations").val("");
                $("tr").removeClass("hidden");
            });

            SearcherLocation();
            return false;
        });


    //Clear selected site value
    $("#clearSite").click(function () {
        $("#siteName").val(null);
        $("#ToSite").val(null);
        $(".chooseUser").removeClass("hidden");
        $(".chooseLocation").removeClass("hidden");
        //Set all user for choosing, not only users from site
        $.get("/AssetsActions/RelocationAsset/ChooseUsers").done(function (data) {
            $("#users").html(data);
            setEventsToUsers();
        });
    });

    //Clear selected user value
    $("#clearUser").click(function () {
        $("#userName").val(null);
        $("#ToUser").val(null);
        $(".chooseLocation").removeClass("hidden");
        $("#chSite").removeClass("hidden");
    });

    //Clear selected location value
    $("#clearLocation").click(function () {
        $("#ToLocation").val(null);
        $(".chooseUser").removeClass("hidden");
    });
});