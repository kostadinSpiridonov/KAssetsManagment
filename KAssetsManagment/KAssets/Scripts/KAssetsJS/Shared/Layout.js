$(function (){
    //Set menu arrow click
    $(".collapsed").click(function () {
        if ($(this).children().last().children().last().hasClass("glyphicon-arrow-down")) {
            $(this).children().last().children().last().removeClass("glyphicon-arrow-down");
            $(this).children().last().children().last().addClass("glyphicon-arrow-up");
        }
        else {
            $(this).children().last().children().last().addClass("glyphicon-arrow-down");
            $(this).children().last().children().last().removeClass("glyphicon-arrow-up");
        }
    });

 

    Resize();

    //Resize after folding and unfolding
    $(".collapsed").mouseup(function () {
        function explode() {
            Resize();
        }
        setTimeout(explode, 200);

    });


    //Resize main div against menu
    $(".mainBody").css("min-height", $(".leftNavbar").css("height"));



    //Set selected menu become blue
    var url = this.location.pathname.toString();
    $("a[href*='" + url + "']").parent().addClass('active');
    console.log(url);
    if ($("a[href*='" + url + "']").parent().parent().hasClass("sub-menu")) {
        if ($("a[href*='" + url + "']").parent().parent().parent().hasClass("sub-menu")) {

            $("a[href='" + url + "']").parent().parent().prev().click();
            $("a[href='" + url + "']").parent().css('background-color', '#428bca');
            $("a[href='" + url + "']").css('color', 'white');

            $("a[href='" + url + "']").parent().parent().parent().prev().click();
        }
        else {
            $("a[href='" + url + "']").parent().parent().prev().click();
            $("a[href='" + url + "']").parent().css('background-color', '#428bca');
            $("a[href='" + url + "']").css('color', 'white');
        }
    }

    //Get new events for user
    $.ajax(
        {
            url: "/Event/GetNewEventsForUser",
            type: "GET",
            success: function (data) {
                if (data.length == 0) {
                    $(".countLBR").addClass('hidden');
                }
                else {
                    $(".countLBR").removeClass('hidden');
                }
                $(".countLBR").html(data.length);

                var lngt = 5;
                if (data.length < lngt) {
                    lngt = data.length
                }
                //Add events to panel
                for (var i = 0; i < lngt; i++) {
                    $(".menu").append('<li><a  class="eventBarLink" name="' + data[i].RelocationUrl + '" id="' + data[i].Id + '"><i class="fa fa-users text-aqua"></i>' + data[i].Content + '</a></li>');
                }
                //Event click-> relocate to event url -> set event seen
                $(".eventBarLink").click(function () {
                    var yrl = $(this).attr("name");
                    $.ajax({
                        url: "/Event/SetSeen",
                        type: "POST",
                        data: {
                            id: $(this).attr("id")
                        },
                        success: function (data) {
                            if (yrl == "null") {

                                window.location.href = "/Home";
                            }
                            else {
                                window.location.href = yrl;
                            }
                        }

                    });
                });
            }
        })

    //Set count of things
    $.ajax(
        {
            url: "/Home/CountNewThings",
            type: "GET",
            success: function (data) {
                if (data.ItemOrderRequestsForApproving != 0) {
                    $("#reqForAprItemOrd").html(data.ItemOrderRequestsForApproving);
                }

                if (data.ItemOrderApprovedRequests != 0) {
                    $("#approvedReqItemOrd").html(data.ItemOrderApprovedRequests);
                }

                if (data.ItemOrderRequestsForFinishing != 0) {
                    $("#reqForFinishItemOrd").html(data.ItemOrderRequestsForFinishing);
                }

                if (data.AssetOrderRequestsForApproving != 0) {
                    $("#reqForAprAssetOrd").html(data.AssetOrderRequestsForApproving);
                }

                if (data.AssetOrderApprovedRequests != 0) {
                    $("#apprReqAssetOrd").html(data.AssetOrderApprovedRequests);
                }

                if (data.AssetOrderRequestsForFinishing != 0) {
                    $("#reqForFinAssetOrd").html(data.AssetOrderRequestsForFinishing);
                }

                if (data.ProviderOrderRequestsForApproving != 0) {
                    $("#reqForApprProvOrd").html(data.ProviderOrderRequestsForApproving);
                }

                if (data.ProviderOrderApprovedRequests != 0) {
                    $("#apprReqProvOrd").html(data.ProviderOrderApprovedRequests);
                }

                if (data.ScrappingRequests != 0) {
                    $("#scrReq").html(data.ScrappingRequests);
                }

                if (data.RelocationRequestsForApproving != 0) {
                    $("#reqForApprRel").html(data.RelocationRequestsForApproving);
                }

                if (data.RelocationsForIssue != 0) {
                    $("#reqIssRel").html(data.RelocationsForIssue);
                }

                if (data.RelocationForIssueAll != 0) {
                    $("#reqIssAllRel").html(data.RelocationForIssueAll);
                }

                if (data.RelocationReceive != 0) {
                    $("#reqRecRel").html(data.RelocationReceive);
                }

                if (data.RelocationReceiveAll != 0) {
                    $("#reqRecAllRel").html(data.RelocationReceiveAll);
                }

                if (data.RenovationRequestsForApproving != 0) {
                    $("#reqForApprRen").html(data.RenovationRequestsForApproving);
                }

                if (data.RenovationApprovedRequests != 0) {
                    $("#apprReqRen").html(data.RenovationApprovedRequests);
                }

                if (data.RenovationAssetForRenovating != 0) {
                    $("#assetForRen").html(data.RenovationAssetForRenovating);
                }

                if (data.RenovationReturnedAssets != 0) {
                    $("#retAssetsRen").html(data.RenovationReturnedAssets);
                }

                if (data.InvoicesForApproving != 0) {
                    $("#invForAppr").html(data.InvoicesForApproving);
                }

                if (data.InvoicesForPaid != 0) {
                    $("#invForPaid").html(data.InvoicesForPaid);
                }

                if (data.AccidentsForAnswering != 0) {
                    $("#accForAnswer").html(data.AccidentsForAnswering);
                }

                if (data.AccidnetsAnswers != 0) {
                    $("#accAnswers").html(data.AccidnetsAnswers);
                }
            }
        })
})

//Resize menu
function Resize() {
    if ($(".ulSdb").height() > jQuery(window).height() - 50) {
        $(".leftNavbar").css("height", "100%");
        $(".leftNavbar").css("max-height", parseInt(jQuery(window).height() - 50));
    }
}
