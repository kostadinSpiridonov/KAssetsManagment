
$(function () {

    //Add offer panel show or hide
    $(".addOffersBtn").click(function () {
        if ($(".addOffers").hasClass("hidden")) {
            $(".addOffers").removeClass("hidden");
        }
        else {
            $(".addOffers").addClass("hidden");
        }
        $(".addItems").addClass("hidden")
    });

    //Add items panel show or hide
    $(".addItemsBtn").click(function () {
        if ($(".addItems").hasClass("hidden")) {
            $(".addItems").removeClass("hidden")
        }
        else {
            $(".addItems").addClass("hidden")
        }
        $(".addOffers").addClass("hidden");
    });

    //Add form for offer
    $(".addForm").click(function () {
        var count = $(".baseInfo").size();
        count--;
        var old = '[0].';
        count++;
        var nov = '[' + count.toString() + '].';
        var newForm = $(".baseInfo").clone();
        var htmlL = newForm[0].outerHTML.toString();
        var replacedHTML = replaceAll(old, nov, htmlL);
        $(".allForms").append(replacedHTML);
        if (count % 2 !== 0) {
            $(".allForms").append('<div class="col-md-12"><hr/></div>');
        }
    })

    //Remove form for item
    $(".removeForm").click(function () {
        var count = $(".baseInfo").size();
        if (count > 1) {
            if ($(".baseInfo").last().next().hasClass("col-md-12")) {
                $(".baseInfo").last().next().remove();
            }
            $(".baseInfo").last().remove();
        }
    })

    //Remove form for item
    $(".removeFormItem").click(function () {
        var count = $(".baseInfoItem").size();
        if (count > 1) {
            if ($(".baseInfoItem").last().next().hasClass("col-md-12")) {
                $(".baseInfoItem").last().next().remove();
            }
            $(".baseInfoItem").last().remove();

        }
    })

    //Add form for item
    $(".addFormItem").click(function () {

        var count = $(".baseInfoItem").size();
        count--;
        var old = '[0].';
        count++;
        var nov = '[' + count.toString() + '].';
        var newForm = $(".baseInfoItem").clone();
        var htmlL = newForm[0].outerHTML.toString();
        var replacedHTML = replaceAll(old, nov, htmlL);
        $(".allFormsItem").append(replacedHTML);
        if (count % 2 !== 0) {
            $(".allFormsItem").append('<div class="col-md-12"><hr/></div>');
        }

        var id = $('input[name="[' + count + '].DateOfManufacture"]').attr("id") + "dm" + count;
        var next = $('input[name="[' + count + '].DateOfManufacture"]').next();
        $("input[name='[" + count + "].DateOfManufacture']").remove();
        $('<input id="' + id + '"class="form-control" data-val-date="The field Date of manufacture must be a date." data-val-required="The Date of manufacture field is required." name="['
            + count + '].DateOfManufacture" type="datetime">').insertBefore(next)

        $('input[name="[' + count + '].DateOfManufacture"]').datepicker( );
        var currentDate = new Date()
        var day = currentDate.getDate()
        var month = currentDate.getMonth() + 1
        var year = currentDate.getFullYear()
        $('input[name="[' + count + '].DateOfManufacture"]').datepicker("setDate", new Date(year, month, day));



    })
})

function replaceAll(find, replace, str) {
    while (str.indexOf(find) > -1) {
        str = str.replace(find, replace);
    }
    return str;
}