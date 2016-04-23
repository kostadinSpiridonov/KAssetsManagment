$(function () {
    //Set sites from choosing to div
    $.get("/AssetsActions/Asset/ChooseSites").done(function (data) {
        $("#sites").html(data);
        setEventsToSites();
    });

    //Set choosite site click
    $('.chooseSite').click(
       function () {
           $("#sites").removeClass("hidden");
           $("#sites").dialog(
               {
                   width: 500
               });

           //Set default values to seracher
           $("#sites").on('dialogclose', function (event) {
               $("#searchValueSite").val("");
               $("tr").removeClass("hidden");
           });
           SearcherSite();

           return false;
       });


});

//Set events to added sites
function setEventsToSites() {
    $(".site").click(function () {
        var id = $(this).attr("id").toString();

        $("#siteId").val(id);
        $("#siteName").val($(this).parent().prev().html().toString().trim());
        $("#sites").dialog('close');

        //When choose site, add users from site to div
        $.get("/AssetsActions/RelocationAsset/ChooseUsersFromSite/" + id).done(function (data) {
            $("#users").html(data);
            setEventsToUsers();
        });


        //Get organisation currencies
        $.get("/AssetsActions/Asset/OrganisationCurrencies/" + id).done(function (data) {
            var mySelect = $('#selectCurrency');
            mySelect.find("*").remove();
            $.each(data, function (i) {
                mySelect.append(
                    $('<option></option>').val(data[i].Id).html(data[i].Code)
                );
            });
        });
    });
}


$(function () {
    //Add users for choosing to div
    $.get("/AssetsActions/RelocationAsset/ChooseUsers").done(function (data) {
        $("#users").html(data);
        setEventsToUsers();
    });

    //Add choose user click
    $('.chooseUser').click(
        function () {
            $("#users").removeClass("hidden");
            $("#users").dialog(
                {
                    width: 500,
                });

            //Clear serach values
            $("#users").on('dialogclose', function (event) {
                $("#searchValueUser").val("");
                $("tr").removeClass("hidden");
            });

            SearcherUser();
            return false;
        });
})

//Set events to user
function setEventsToUsers() {
    $(".user").click(function () {
        var id = $(this).attr("id").toString();

        $("#userId").val(id);
        $("#users").dialog('close');


        $(".chooseLocation").addClass("hidden");

        if (typeof $("#siteId").val() == 'undefined') {
            $(".chooseSite").addClass("hidden");
        }

        var userName = $(this).parent().parent().children().last().prev().html().toString().trim();

        //When choose user, set user site seleced
        $.get("/AssetsActions/RelocationAsset/GetUserSiteId/" + id).done(function (data) {
            $("#siteId").val(data);
            var siteName = $("#" + data).parent().parent().children().first().html().toString().trim();
            $("#siteName").val(siteName);

            //Get organisation currencies
            $.get("/AssetsActions/Asset/OrganisationCurrencies/" + data).done(function (data1) {
                var mySelect = $('#selectCurrency');
                mySelect.find("*").remove();
                $.each(data1, function (i) {
                    mySelect.append(
                        $('<option></option>').val(data1[i].Id).html(data1[i].Code)
                    );
                });
            });

        }).fail(function () {

            $("#siteId").val(null);
            $("#siteName").val("");
        });

        //When choose user, set user location is selected
        $.get("/AssetsActions/RelocationAsset/GetUserLocationId/" + id).done(function (data) {
            $("#locationId").val(data);
        }).fail(function () {
            $("#locationId").val(null);
        });
    });
}

//Create searcher in pop up window
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

$(function () {

    //Set location for choosing to divs
    $.get("/AssetsActions/RelocationAsset/ChooseLocation").done(function (data) {
        $("#locations").html(data);
        setEventsToLocations();
    });

    //Set location click event
    $('.chooseLocation').click(
        function () {
            $("#locations").removeClass("hidden");
            $("#locations").dialog(
                {
                    width: 765,
                });

            //Clear search values
            $("#locations").on('dialogclose', function (event) {
                $("#searchValueLocations").val("");
                $("tr").removeClass("hidden");
            });

            SearcherLocation();
            return false;
        });
})

//Choose location event
function setEventsToLocations() {
    $(".location").click(function () {
        var id = $(this).attr("id").toString();

        $("#locationId").val(id);
        $("#locations").dialog('close');

        $(".chooseUser").addClass("hidden");


    });
}

$(function () {
    //Clear site 
    $("#clearSite").click(function () {
        $("#siteName").val(null);
        $("#siteId").val(null);
        $(".chooseUser").removeClass("hidden");
        $(".chooseLocation").removeClass("hidden");

        //Set all users for choosing, not only from site
        $.get("/AssetsActions/RelocationAsset/ChooseUsers").done(function (data) {
            $("#users").html(data);
            setEventsToUsers();
        });
    });

    $("#clearUser").click(function () {
        $("#userId").val(null);
        $(".chooseLocation").removeClass("hidden");
        $(".chooseSite").removeClass("hidden");
    });

    $("#clearLocation").click(function () {
        $("#locationId").val(null);
        $(".chooseUser").removeClass("hidden");
    });
})

//Search user
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

//Search location
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