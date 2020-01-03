var metaShare = metaShare || {};

metaShare.DateFormat = function () {
};


metaShare.DateFormat.formatValid = function (s) {
    return s < 10 ? '0' + s : s;
};

metaShare.DateFormat.getValidDate = function (dateLiteral) {
    if (dateLiteral == null) {
        return new Date();
    } else {
        if (metaShare.DateFormat.IsDataMin(dateLiteral)) {
            return new Date();
        }

        var dateLiteralReplace = dateLiteral.replace(/-/g, '/');
        var date = new Date(dateLiteralReplace);
        if (date == "Invalid Date") {
            return new Date();
        } else {
            return date;
        }
    }
};

metaShare.DateFormat.getDateFormat = function (dateLiteral) {
    var date = metaShare.DateFormat.getValidDate(dateLiteral);
    return date.getFullYear() +
        '-' +
        metaShare.DateFormat.formatValid(date.getMonth() + 1) +
        "-" +
        metaShare.DateFormat.formatValid(date.getDate());
};

metaShare.DateFormat.getDateTimeFormat = function (dateLiteral) {
    var date = metaShare.DateFormat.getValidDate(dateLiteral);

    return metaShare.DateFormat.getDateFormat(dateLiteral) +
        " " +
        metaShare.DateFormat.formatValid(date.getHours()) +
        ':' +
        metaShare.DateFormat.formatValid(date.getMinutes()) +
        ":" +
        metaShare.DateFormat.formatValid(date.getSeconds());
};

metaShare.DateFormat.format = function (element, value) {
    var datetimepickerDefaultParam = {
        value: metaShare.DateFormat.getDateTimeFormat(value),
        lang: 'ch',
        weekStart: 0,
        autoclose: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        timepicker: false,
        format: "Y-m-d"
    };

    element.datetimepicker(datetimepickerDefaultParam);

    if (datetimepickerDefaultParam.format != null && datetimepickerDefaultParam.format == "Y-m-d") {
        element.datetimepicker({ value: metaShare.DateFormat.getDateFormat(value) });
    }
};

metaShare.DateFormat.IsDataMin = function (dateLiteral) {
    if (dateLiteral == null) return true;
    var dataMinLiterals = new Array("0001-01-01", "0001/01/01", "01/01/0001", "01-01-0001", "1/1/0001", "1-1-0001");
    for (var i = 0; i < dataMinLiterals.length; i++) {
        if (dateLiteral.match(dataMinLiterals[i])) {
            return true;
        }
    }
    return false;
};