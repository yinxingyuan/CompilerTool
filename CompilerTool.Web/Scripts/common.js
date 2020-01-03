var metaShare = metaShare || {};

metaShare.basic = function () {
};

metaShare.basic.prototype.deleteTableColumn=function(element, id, deleteUrl) {
    $("#dialog-confirm").dialog({
        resizable: false,
        minHeight: 200,
        width: 500,
        modal: true,
        buttons: {
            "OK": function () {
                $.post(deleteUrl, { id: id }, function (result) {
                    if (result == true) {
                        window.location.reload();
                        //alert("Delete Success!");
                        //$(element).parent().parent().remove();
                    } else {
                        alert("Operation fail!");
                    }

                }, "json");
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });
}

metaShare.basic.prototype.clickLight = function (defaultActiveItemId) {

    var index = $(".m_title_box ol li").length;
    if (index < 2) {
        return;
    }

    $.each($(".m_title_box ol li:not(:last-child)"),
        function (i, eles) {
            var pathname = $(eles).attr("data-id");
            var onobj = $("#main-menu li[data-id='" + pathname + "']");
            if (index == 2) {
                onobj.addClass("active-menu");
            } else if (index == 3) {
                if (i == 0) {
                    onobj.addClass("active-menu");
                } else {
                    onobj.parent().addClass("in");
                    onobj.addClass("on");
                }
            } else {
                if (i == 0) {
                    onobj.addClass("active-menu");
                } else if (i == index - 2) {
                    onobj.parent().addClass("in");
                    onobj.addClass("on");
                } else {
                    onobj.parent().addClass("in");
                    onobj.addClass("active on");
                }
            }
        });
    
}

metaShare.basic.prototype.footerPosition = function () {
    var pageHeight = window.innerHeight;
    if (typeof pageHeight != "number") {
        if(document.compatMode == "number"){
            pageHeight = document.documentElement.clientHeight;
        }else{
            pageHeight = document.body.clientHeight;
        }
    } 
    $(".tt_body").css({ "min-height": pageHeight - 50 });
    $(".tt_body .tt_content_left").css({ "min-height": pageHeight - 115 });
}