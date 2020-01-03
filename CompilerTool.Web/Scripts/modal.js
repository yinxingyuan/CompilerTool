var MetaShare = MetaShare || {};

MetaShare.Modal = function () {
};

MetaShare.Modal.prototype.openModal = function (elementId) {
    $("#" + elementId).show().children().stop().slideDown();
    $("#" + elementId).children("div").draggable();
};

MetaShare.Modal.prototype.closeModal = function (elementId) {
    $("#" + elementId).children().stop().slideUp(function () {
        $("#" + elementId).hide();
    });
};





