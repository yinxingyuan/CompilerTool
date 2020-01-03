function setPageIndex(currentPage, pageTotal) {
    var element = document.getElementById('pageIndexInputId');
    var page;

    if (element != undefined) {
        page = element.value;
    }

    if (page == '') {
        return;
    }

    if (page == currentPage) {
        return;
    }

    if (page > pageTotal || page < 1) {
        return;
    }

    document.getElementById('pageSizeAndPageToForm').submit();

}
;

function setJumpPagerLinkWithGo(currentPage, pageTotal, goelementId,perPageelementId, element) {
    var elementGo = goelementId.value;
    var elementPerPage = perPageelementId.value;
    var goValue;
    var linkEnd = $(element).attr("href");

    if (elementGo != "") {
        if (elementGo<1) {
            goValue = 1;
        } else if (elementGo > pageTotal) {
            goValue = pageTotal;
        } else {
            goValue = elementGo;
        }
    } else {
        goValue = currentPage;
    }

    if (/\?/.test($(element).attr("href")))
    {
        linkEnd += "&pageIndex=" + goValue + "&pageSize=" + elementPerPage;
    }
    else
    {
        linkEnd += "?pageIndex=" + goValue + "&pageSize=" + elementPerPage;
    }

    $(element).attr("href", linkEnd);
};

function setSearchPageSize(pageSize) {
    var searchPageSize = $(":input[type='hidden'][name='Pager.PageSize']");
    if (searchPageSize != undefined) {
        $(searchPageSize).val(pageSize);
    }
}

function setJumpPagerLinkWithPerPage(currentPage, pageTotal, perPageelementId, element) {
    var elementPerPage = perPageelementId.value;
    var linkEnd = $(element).attr("href");
    
    if (/\?/.test($(element).attr("href"))) {
        linkEnd += "&pageIndex=1&pageSize=" + elementPerPage;
    }
    else {
        linkEnd += "?pageIndex=1&pageSize=" + elementPerPage;
    }
    $(element).attr("href", linkEnd);
    setSearchPageSize(elementPerPage);
}
