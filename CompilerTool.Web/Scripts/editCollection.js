var MetaShare = MetaShare || {};

MetaShare.EditCollection = function (collectionContentElementId, addTemplateElementId, editContentElementId, editItemsElementsIdPrefix, editContentElementsIdPrefix) {
    this.CollectionElementId = collectionContentElementId;
    this.AddTemplateElementId = addTemplateElementId;
    this.EditContentElementId = editContentElementId;
    this.ItemsElementsIdPrefix = editItemsElementsIdPrefix;
    this.EditElementsIdPrefix = editContentElementsIdPrefix;
    this.CurrentEditIndex = null;
    this.IsAdd = false;
    if (this.ItemsElementsIdPrefix == null) {

    }
    if (this.EditElementsIdPrefix == null) {
         
    }
    this.initialize();
};

MetaShare.EditCollection.prototype.initialize = function () {
    var thisOjb = this;
    var editElements = $("#" + thisOjb.AddTemplateElementId).find("input[type=text],input[type=hidden],input[type=password],input[type=radio],input[type=checkbox],select,textarea");
    for (var i = 0; i < editElements.length; i++) {
        var editElement = $(editElements[i]);
        if (editElement.attr("name").match(/\[{}\]/g)) {
            editElement.attr("id", editElement.attr("name").replace(/\[{}\]/g, "_{}_").replace(".", "_"));
        }
    }
};

MetaShare.EditCollection.prototype.addItem = function () {
    var thisOjb = this;
    var index = $("#" + thisOjb.CollectionElementId).find("tbody").find(".data_tr").length;
    var attributeHtml = $("#" + thisOjb.AddTemplateElementId).html().replace(/{}/g, index);
    $("#" + thisOjb.CollectionElementId).find("tbody").append(attributeHtml);
    thisOjb.setIsAdd(true);
    MetaShare.Modal.prototype.openModal(thisOjb.EditContentElementId);
    thisOjb.loadItem(index);
    thisOjb.validate();
};

MetaShare.EditCollection.prototype.loadItem = function (index) {
    var thisOjb = this;
    thisOjb.setCurrentEditIndex(index);
    MetaShare.EditCollection.setBy(thisOjb.ItemsElementsIdPrefix + "_" + index + "_", thisOjb.EditElementsIdPrefix, thisOjb.EditContentElementId);
};

MetaShare.EditCollection.prototype.editItem = function (element) {
    var thisOjb = this;
    thisOjb.loadItem(thisOjb.getCurrentEditIndex(element));
    thisOjb.setIsAdd(false);
    MetaShare.Modal.prototype.openModal(thisOjb.EditContentElementId);
    thisOjb.validate();
};

MetaShare.EditCollection.prototype.viewItem = function (element) {
    var thisOjb = this;
    thisOjb.loadItem(thisOjb.getCurrentEditIndex(element));
    MetaShare.Modal.prototype.openModal(thisOjb.EditContentElementId);
};

MetaShare.EditCollection.prototype.getCurrentEditIndex = function (element) {
    var thisOjb = this;
    var dataTr = $("#" + thisOjb.CollectionElementId).find("tbody").find(".data_tr");
    return dataTr.index($(element).closest("tr"));
};

MetaShare.EditCollection.prototype.saveItem = function (element) {
    var thisOjb = this;
    if (thisOjb.validate(element) ===true) {
        MetaShare.EditCollection.setBy(thisOjb.EditElementsIdPrefix, thisOjb.ItemsElementsIdPrefix + "_" + thisOjb.CurrentEditIndex + "_", thisOjb.CollectionElementId);
        MetaShare.Modal.prototype.closeModal(thisOjb.EditContentElementId);
    }
};

MetaShare.EditCollection.prototype.cancelItem = function () {
    var thisOjb = this;
    if (thisOjb.IsAdd) {
        $("#" + thisOjb.CollectionElementId).find("tbody").find(".data_tr").last().remove();
    }
    MetaShare.Modal.prototype.closeModal(thisOjb.EditContentElementId);
};

MetaShare.EditCollection.prototype.removeItem = function (element) {
    var thisOjb = this;
    $(element).parents("tr").remove();
    var dataTr = $("#" + thisOjb.CollectionElementId).find("tbody").find(".data_tr");

    for (var i = 0; i < dataTr.length; i++) {
        var dataElements = dataTr.eq(i).find("*");
        for (var j = 0; j < dataElements.length; j++) {
            var elementName = dataElements.eq(j).attr("name");
            if (elementName != null) {
                dataElements.eq(j).attr("name", elementName.replace(/\[\d+\]/g, '[' + i + ']'));
            }
            var elementId = dataElements.eq(j).attr("id");
            if (elementId != null) {
                dataElements.eq(j).attr("id", elementId.replace(/\_\d+\_/g, '_' + i + '_'));
            }
        }
    }
};

MetaShare.EditCollection.prototype.setCurrentEditIndex = function (index) {
    var thisOjb = this;
    thisOjb.CurrentEditIndex = index;
};

MetaShare.EditCollection.prototype.setIsAdd = function(boolValue) {
    var thisOjb = this;
    thisOjb.IsAdd = boolValue;
};

MetaShare.EditCollection.setBy = function(sourceElementIdPrefix, targetElementIdPrefix, targetContentElementId) {
    var editElements = $("#" + targetContentElementId)
        .find("input[type=text],input[type=hidden],input[type=password],input[type=radio],input[type=checkbox],select,textarea,:contains('" + targetElementIdPrefix + "')");

    for (var i = 0; i < editElements.length; i++) {
        var targetElement = $(editElements[i]);
        var targetElementId = targetElement.attr("id");
        if (targetElementId != undefined) {
            var sourseElement = $("#" + targetElement.attr("id").replace(targetElementIdPrefix, sourceElementIdPrefix));
            if (sourseElement != null) {
                if (targetElement.is("input")) {
                    if (targetElement.attr("type") === "radio") {
                        if (sourseElement.attr("type") === "radio") {
                            if (sourseElement.is(':checked')) {
                                targetElement.prop("checked", true);
                            } else {
                                targetElement.prop("checked", false);
                            }
                            continue;
                        }
                    }
                    if (targetElement.attr("type") === "checkbox") {
                        if (sourseElement.attr("type") === "checkbox") {
                            if (sourseElement.is(':checked')) {
                                targetElement.prop("checked", true);
                            } else {
                                targetElement.prop("checked", false);
                            }
                            continue;
                        }
                    }
                    if (targetElement.attr("type") === "text" || targetElement.attr("type") === "hidden") {
                        if (sourseElement.is("select")) {
                            MetaShare.EditCollection.prototype.setBySelect(targetElement, sourseElement);
                            continue;
                        }
                    }
                } else if (targetElement.is("select")) {
                    var sourseValue = sourseElement.val();
                    if (sourseValue === "") continue;
                    MetaShare.EditCollection.prototype.setSelect(targetElement, sourseElement);
                    continue;
                } else if (targetElement.is("textarea") === "textarea") {
                    if (sourseElement.is("textarea")) {
                        targetElement.html(sourseElement.html()); continue;
                    } else {
                        targetElement.html(sourseElement.val()); continue;
                    }
                }
                targetElement.val(sourseElement.val());
            }
        }
    }
    $("#" + targetContentElementId).find("select").trigger("change");
};

MetaShare.EditCollection.prototype.setSelectText = function (selectId, changeElementId) {
    if ($("#" + selectId).val() === "0" || $("#" + selectId).val() === "") {
        $("#" + changeElementId).val("");
    } else {
        $("#" + changeElementId).val($("#" + selectId).find("option:selected").text());
    }
};


MetaShare.EditCollection.prototype.setSelect = function (targetElement, sourseElement) {
    var count = $(targetElement).get(0).options.length;
    for (var i = 0; i < count; i++) {
        if ($(targetElement).get(0).options[i].value == sourseElement.val() || $(targetElement).get(0).options[i].text == sourseElement.val()) {
            $(targetElement).get(0).options[i].selected = true;
            break;
        }
    }
};

MetaShare.EditCollection.prototype.setBySelect = function (targetElement, sourseElement) {
    targetElement.val(sourseElement.val());
    if (targetElement.attr("id").match(/_Id$/)) {
        var targetName = targetElement.attr("id").replace(/_Id$/, "_Name");
    } else {
        targetElement.parent().find("span").html(sourseElement.find("option:selected").text());
    }
};

MetaShare.EditCollection.prototype.validate = function () {
    var thisOjb = this;
    var editElements = $("#" + thisOjb.EditContentElementId).find("input[type=text],input[type=hidden],input[type=password],input[type=radio],input[type=checkbox],select,textarea");
    var validator = $("form").validate();
    var valid = true;
    for (var i = 0; i < editElements.length; i++) {
        var validState = validator.element($(editElements[i]));
        if (validState === false) {
            valid = false;
            break;
        }
    }
    return valid;
};