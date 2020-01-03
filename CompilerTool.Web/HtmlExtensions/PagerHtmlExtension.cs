using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MetaShare.Common.Core.Presentation;
using CompilerTool.Entities;
namespace CompilerTool.Web.HtmlExtensions
{
	    public static class PagerHtmlExtension
    {
        private const string FirstPageTag = "|<";
        private const string LastPageTag = ">|";
        private const string PreviousTag = "<";
        private const string NextTag = ">";
        private const int MaxDisplayPageCount = 5;

        public static MvcHtmlString RenderPager<T>(this HtmlHelper helper, TargetPager<T> dataList, string url, object routeValues = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (dataList == null)
            {
                dataList = new TargetPager<T> {PageTotal = 0};
            }


            if (dataList.PageTotal <= 0)
            {
                return new MvcHtmlString(string.Empty);
            }

            if (dataList.PageIndex > dataList.PageTotal)
            {
                dataList.PageIndex = 1;
            }

            int start = 1;
            int end = dataList.PageTotal;

            if (dataList.PageTotal > MaxDisplayPageCount)
            {
                start = dataList.PageIndex - 4;
                end = dataList.PageIndex + 5;

                if (dataList.PageIndex - 4 <= 1)
                {
                    start = 1;
                    end = MaxDisplayPageCount;
                }
                if (dataList.PageIndex + 4 >= dataList.PageTotal)
                {
                    start = dataList.PageTotal - MaxDisplayPageCount + 1;
                    end = dataList.PageTotal;
                }
            }

            TagBuilder pagerDiv = new TagBuilder("div");
            pagerDiv.MergeAttribute("style", "display:inline-block;");

            TagBuilder pageDivGo = new TagBuilder("div");
            pageDivGo.AddCssClass("togoperpage");

            TagBuilder unsortList = new TagBuilder("ul");
            unsortList.AddCssClass("pagination");

            unsortList.InnerHtml += RenderPageLi(dataList, FirstPageTag, url, routeValues);
            unsortList.InnerHtml += RenderPageLi(dataList, PreviousTag, url, routeValues);

            for (int i = start; i <= end; i++)
            {
                unsortList.InnerHtml += RenderPageLi(dataList, Convert.ToString(i), url, routeValues);
            }

            unsortList.InnerHtml += RenderPageLi(dataList, NextTag, url, routeValues);
            unsortList.InnerHtml += RenderPageLi(dataList, LastPageTag, url, routeValues);

            pageDivGo.InnerHtml = RenderSearchPageAndPageSizeDiv(dataList, url, routeValues);

            pagerDiv.InnerHtml = string.Concat(unsortList.ToString(), pageDivGo.ToString());

            return new MvcHtmlString(pagerDiv.ToString());
        }

        private static string RenderSearchPageAndPageSizeDiv<T>(TargetPager<T> dataList, string url, object routeValues = null)
        {
            TagBuilder form = new TagBuilder("form");
            form.GenerateId("pageSizeAndPageToForm");

            if (routeValues != null)
            {
                PropertyInfo[] attributes = routeValues.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in attributes)
                {
                    TagBuilder hiddenDiv = new TagBuilder("input");
                    hiddenDiv.MergeAttribute("type", "hidden");
                    hiddenDiv.MergeAttribute("name", propertyInfo.Name);
                    hiddenDiv.MergeAttribute("value", Convert.ToString(propertyInfo.GetValue(routeValues, null)));
                    form.InnerHtml += hiddenDiv.ToString();
                }
            }

            form.MergeAttribute("action", url);

            form.InnerHtml += RenderToPageDiv("go_to_page", dataList);
            form.InnerHtml += RenderSetSizeDiv(dataList, "per_page");
            return form.ToString();
        }

        private static string RenderSetSizeDiv<T>(TargetPager<T> dataList, string divClassName)
        {
            TagBuilder div = new TagBuilder("div");
            div.AddCssClass(divClassName);
            TagBuilder span = new TagBuilder("span");
            span.SetInnerText("Per page");
            div.InnerHtml += span.ToString();

            TagBuilder select = new TagBuilder("select");
            select.GenerateId("pager_pagesize_selectid");
            select.MergeAttribute("name", "pageSize");
            const string pageEvent = "javascript:document.getElementById('pageSizeAndPageToForm').submit();";
            select.MergeAttribute("onchange", pageEvent);

            TagBuilder option = new TagBuilder("option");
            option.SetInnerText("5");
            option.MergeAttribute("value", "5");

            TagBuilder option1 = new TagBuilder("option");
            option1.SetInnerText("10");
            option1.MergeAttribute("value", "10");

            TagBuilder option2 = new TagBuilder("option");
            option2.SetInnerText("15");
            option2.MergeAttribute("value", "15");

            TagBuilder option3 = new TagBuilder("option");
            option3.SetInnerText("20");
            option3.MergeAttribute("value", "20");

            if (dataList.PageSize == 20)
            {
                option3.MergeAttribute("selected", "selected");
            }
            else if (dataList.PageSize == 15)
            {
                option2.MergeAttribute("selected", "selected");
            }
            else if (dataList.PageSize == 10)
            {
                option1.MergeAttribute("selected", "selected");
            }
            else
            {
                option.MergeAttribute("selected", "selected");
            }

            select.InnerHtml += option;
            select.InnerHtml += option1;
            select.InnerHtml += option2;
            select.InnerHtml += option3;

            div.InnerHtml += select.ToString();
            return div.ToString();
        }

        private static string RenderToPageDiv<T>(string divClassName, TargetPager<T> dataList)
        {
            TagBuilder toPageDiv = new TagBuilder("div");
            toPageDiv.AddCssClass(divClassName);
            TagBuilder span = new TagBuilder("span");
            span.SetInnerText("To");
            toPageDiv.InnerHtml += span.ToString();

            TagBuilder textArea = new TagBuilder("input");
            textArea.MergeAttribute("name", "pageIndex");
            textArea.GenerateId("pageIndexInputId");
            textArea.MergeAttribute("type", "text");
            toPageDiv.InnerHtml += textArea.ToString();

            TagBuilder go = new TagBuilder("input");
            go.MergeAttribute("value", "Go");
            go.MergeAttribute("type", "button");

            string buttonEventString = "javascript:setPageIndex(" + dataList.PageIndex + "," + dataList.PageTotal + ");";

            go.MergeAttribute("onclick", buttonEventString);
            toPageDiv.InnerHtml += go.ToString();

            return toPageDiv.ToString();
        }

        private static string RenderPageLi<T>(TargetPager<T> dataList, string text, string url, object routeValues)
        {
            TagBuilder listItem = new TagBuilder("li");

            int pageNumber;

            if (text == PreviousTag)
            {
                pageNumber = dataList.PageIndex - 1;
            }
            else if (text == NextTag)
            {
                pageNumber = dataList.PageIndex + 1;
            }
            else if (text == FirstPageTag)
            {
                pageNumber = 1;
            }
            else if (text == LastPageTag)
            {
                pageNumber = dataList.PageTotal;
            }
            else
            {
                pageNumber = Convert.ToInt16(text, 10);
            }

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageNumber > dataList.PageTotal)
            {
                pageNumber = dataList.PageTotal;
            }


            string targetUrl = url;
            if (routeValues != null)
            {
                PropertyInfo[] attributes = routeValues.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in attributes)
                {
                    if (targetUrl.Contains("?"))
                    {
                        targetUrl += "&";
                    }
                    else
                    {
                        targetUrl += "?";
                    }

                    targetUrl += propertyInfo.Name + "=" + propertyInfo.GetValue(routeValues, null);
                }
            }

            if (targetUrl.Contains("?"))
            {
                targetUrl += "&pageIndex=" + pageNumber + "&pageSize=" + dataList.PageSize;
            }
            else
            {
                targetUrl += "?pageIndex=" + pageNumber + "&pageSize=" + dataList.PageSize;
            }

            TagBuilder link = new TagBuilder("a");

            if (pageNumber != dataList.PageIndex)
            {
                link.MergeAttribute("href", targetUrl);
            }
            link.SetInnerText(text);

            int currentIndex = 0;
            int.TryParse(text, out currentIndex);

            if (currentIndex == dataList.PageIndex)
            {
                listItem.AddCssClass("active");
            }

            listItem.InnerHtml += link;
            return listItem.ToString();
        }

        #region AjaxRenderPager

        public static MvcHtmlString AjaxRenderPager<T>(this AjaxHelper helper, TargetPager<T> dataList, string action, string controller, string tagert, object routeValues = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }
            if (string.IsNullOrEmpty(controller))
            {
                throw new ArgumentNullException("controller");
            }
            if (dataList == null)
            {
                dataList = new TargetPager<T> {PageTotal = 0};
            }

            if (dataList.PageTotal <= 0)
            {
                return new MvcHtmlString(string.Empty);
            }

            if (dataList.PageIndex > dataList.PageTotal)
            {
                dataList.PageIndex = 1;
            }

            int start = 1;
            int end = dataList.PageTotal;

            if (dataList.PageTotal > MaxDisplayPageCount)
            {
                if (dataList.PageIndex - (MaxDisplayPageCount - 1) <= 1)
                {
                    start = 1;
                    end = MaxDisplayPageCount;
                }

                if (dataList.PageIndex > MaxDisplayPageCount)
                {
                    int remainderForPageIndex = dataList.PageIndex%MaxDisplayPageCount;
                    int remainderForPageTotal = dataList.PageTotal%MaxDisplayPageCount;
                    int placePageForPageIndex = dataList.PageIndex/MaxDisplayPageCount;
                    int placePageForPageTotal = dataList.PageTotal/MaxDisplayPageCount;

                    if (remainderForPageTotal == 0)
                    {
                        if (remainderForPageIndex == 0)
                        {
                            start = (placePageForPageIndex - 1)*MaxDisplayPageCount + 1;
                        }
                        else
                        {
                            start = placePageForPageIndex*MaxDisplayPageCount + 1;
                        }
                        end = start + MaxDisplayPageCount - 1;
                    }

                    else
                    {
                        if (remainderForPageIndex == 0)
                        {
                            start = (placePageForPageIndex - 1)*MaxDisplayPageCount + 1;
                            end = start + MaxDisplayPageCount - 1;
                        }
                        else
                        {
                            start = placePageForPageIndex*MaxDisplayPageCount + 1;
                            if (placePageForPageIndex == placePageForPageTotal)
                            {
                                end = dataList.PageTotal;
                            }
                            else
                            {
                                end = start + MaxDisplayPageCount - 1;
                            }
                        }
                    }
                }
            }

            int firstPageNumber;
            int lastPageNumber;
            GetPageNumber(start, end, out firstPageNumber, out lastPageNumber);

            TagBuilder pagerDiv = new TagBuilder("div");
            pagerDiv.MergeAttribute("style", "display:inline-block;");

            TagBuilder pageDivGo = new TagBuilder("div");
            pageDivGo.AddCssClass("togoperpage");


            TagBuilder unsortList = new TagBuilder("ul");
            unsortList.AddCssClass("pagination");

            unsortList.InnerHtml += AjaxRenderPageLi(helper, dataList, FirstPageTag, action, controller, routeValues, tagert, firstPageNumber, lastPageNumber);
            unsortList.InnerHtml += AjaxRenderPageLi(helper, dataList, PreviousTag, action, controller, routeValues, tagert, firstPageNumber, lastPageNumber);

            for (int i = start; i <= end; i++)
            {
                unsortList.InnerHtml += AjaxRenderPageLi(helper, dataList, Convert.ToString(i), action, controller, routeValues, tagert, firstPageNumber, lastPageNumber);
            }

            unsortList.InnerHtml += AjaxRenderPageLi(helper, dataList, NextTag, action, controller, routeValues, tagert, firstPageNumber, lastPageNumber);
            unsortList.InnerHtml += AjaxRenderPageLi(helper, dataList, LastPageTag, action, controller, routeValues, tagert, firstPageNumber, lastPageNumber);

            pageDivGo.InnerHtml = AjaxRenderSearchPageAndPageSizeDiv(helper, dataList, " col-static-24", routeValues, tagert, action, controller);

            pagerDiv.InnerHtml = string.Concat(unsortList.ToString(), pageDivGo.ToString());

            return new MvcHtmlString(pagerDiv.ToString());
        }

        private static string AjaxRenderSearchPageAndPageSizeDiv<T>(AjaxHelper helper, TargetPager<T> dataList, string divClassName, object routeValues, string updateTarget, string action, string controller)
        {
            string goelementId = updateTarget + "_pageIndexInputId";
            string perPageelementId = updateTarget + "_pager_pagesize_selectid";

            MvcHtmlString ajaxLink = helper.ActionLink("Go", action, controller, routeValues, new AjaxOptions {HttpMethod = "Post", UpdateTargetId = updateTarget, InsertionMode = InsertionMode.Replace}, new {@class = "go", onclick = "setJumpPagerLinkWithGo(" + dataList.PageIndex + "," + dataList.PageTotal + "," + goelementId + "," + perPageelementId + ", this)"});
            MvcHtmlString ajaxLinkHide = helper.ActionLink("Go", action, controller, routeValues, new AjaxOptions {HttpMethod = "Post", UpdateTargetId = updateTarget, InsertionMode = InsertionMode.Replace}, new {id = updateTarget + "_perPageHiddenGo", hidden = "hidden", onclick = "setJumpPagerLinkWithPerPage(" + dataList.PageIndex + "," + dataList.PageTotal + "," + perPageelementId + ", this )"});

            string toPageDiv = AjaxRenderToPageDiv("col-static-12", dataList, ajaxLink.ToString(), updateTarget);
            string perPageDiv = AjaxRenderSetSizeDiv(dataList, "col-static-12", ajaxLinkHide.ToString(), updateTarget);

            return string.Concat(toPageDiv, perPageDiv);
        }

        private static string GetTargetUrl(object routeValues)
        {
            string targetUrl = string.Empty;

            if (routeValues != null)
            {
                PropertyInfo[] attributes = routeValues.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in attributes)
                {
                    if (targetUrl.Contains("?"))
                    {
                        targetUrl += "&";
                    }
                    else
                    {
                        targetUrl += "?";
                    }

                    targetUrl += propertyInfo.Name + "=" + propertyInfo.GetValue(routeValues, null);
                }
            }

            return targetUrl;
        }

        private static string AjaxRenderToPageDiv<T>(string divClassName, TargetPager<T> dataList, string pagerGoUrl, string updateTarget)
        {
            TagBuilder toPageDiv = new TagBuilder("div");
            toPageDiv.AddCssClass("go_to_page");

            //TagBuilder spanTotalPage = new TagBuilder("span");
            //spanTotalPage.SetInnerText("Total" + " " + dataList.PageTotal);
            //toPageDiv.InnerHtml += spanTotalPage.ToString();

            TagBuilder span = new TagBuilder("span");
            span.SetInnerText("To");
            toPageDiv.InnerHtml += span.ToString();

            TagBuilder textArea = new TagBuilder("input");
            textArea.MergeAttribute("name", "pageIndex");
            textArea.GenerateId(updateTarget + "_pageIndexInputId");
            textArea.MergeAttribute("type", "text");
            textArea.MergeAttribute("style", "width:50px;height:30px;");
            toPageDiv.InnerHtml += textArea.ToString();
            toPageDiv.InnerHtml += pagerGoUrl;

            return toPageDiv.ToString();
        }

        private static string AjaxRenderSetSizeDiv<T>(TargetPager<T> dataList, string divClassName, string pagePerPagerGoUrl, string updateTarget)
        {
            TagBuilder div = new TagBuilder("div");
            div.AddCssClass("per_page");

            TagBuilder span = new TagBuilder("span");
            span.SetInnerText("Per Page");
            div.InnerHtml += span.ToString();

            string perPageLinkId = updateTarget + "_perPageHiddenGo";
            TagBuilder select = new TagBuilder("select");
            select.GenerateId(updateTarget + "_pager_pagesize_selectid");
            select.MergeAttribute("name", "pageSize");
            string pageEvent = "javascript:document.getElementById('" + perPageLinkId + "').click();";
            select.MergeAttribute("onchange", pageEvent);

            TagBuilder option = new TagBuilder("option");
            option.SetInnerText("5");
            option.MergeAttribute("value", "5");


            TagBuilder option1 = new TagBuilder("option");
            option1.SetInnerText("10");
            option1.MergeAttribute("value", "10");

            TagBuilder option2 = new TagBuilder("option");
            option2.SetInnerText("15");
            option2.MergeAttribute("value", "15");

            TagBuilder option3 = new TagBuilder("option");
            option3.SetInnerText("20");
            option3.MergeAttribute("value", "20");

            if (dataList.PageSize == 20)
            {
                option3.MergeAttribute("selected", "selected");
            }
            else if (dataList.PageSize == 15)
            {
                option2.MergeAttribute("selected", "selected");
            }
            else if (dataList.PageSize == 10)
            {
                option1.MergeAttribute("selected", "selected");
            }
            else
            {
                option.MergeAttribute("selected", "selected");
            }

            select.InnerHtml += option;
            select.InnerHtml += option1;
            select.InnerHtml += option2;
            select.InnerHtml += option3;

            div.InnerHtml += select.ToString();
            div.InnerHtml += pagePerPagerGoUrl;
            return div.ToString();
        }

        private static void GetPageNumber(int start, int end, out int firstPageNumber, out int lastPageNumber)
        {
            firstPageNumber = ((start/(end - start + 1)) - 1)*(end - start + 1) + 1;
            lastPageNumber = end + 1;
        }

        private static string AjaxRenderPageLi<T>(AjaxHelper ajaxHelper, TargetPager<T> dataList, string text, string action, string controller, object routeValues, string updateTarget, int firstPageNumber, int lastPageNumber)
        {
            TagBuilder listItem = new TagBuilder("li");

            int pageNumber;


            if (text == PreviousTag)
            {
                pageNumber = dataList.PageIndex - 1;
            }
            else if (text == NextTag)
            {
                pageNumber = dataList.PageIndex + 1;
            }
            else if (text == FirstPageTag)
            {
                pageNumber = firstPageNumber;
            }
            else if (text == LastPageTag)
            {
                pageNumber = lastPageNumber;
            }
            else
            {
                pageNumber = Convert.ToInt16(text, 10);
            }

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageNumber > dataList.PageTotal)
            {
                pageNumber = dataList.PageTotal;
            }

            if (pageNumber != dataList.PageIndex)
            {
                string targetUrl = GetTargetUrl(routeValues);

                if (targetUrl.Contains("?"))
                {
                    targetUrl += "&pageIndex=" + pageNumber + "&pageSize=" + dataList.PageSize;
                }
                else
                {
                    targetUrl += "?pageIndex=" + pageNumber + "&pageSize=" + dataList.PageSize;
                }
                MvcHtmlString ajaxLink = ajaxHelper.ActionLink(text, action, controller, null, new AjaxOptions {HttpMethod = "Post", UpdateTargetId = updateTarget, InsertionMode = InsertionMode.Replace});
                string oldUrl = "/" + controller + "/" + action;
                string newUrl = "/" + controller + "/" + action + targetUrl;
                string link = ajaxLink.ToHtmlString().Replace(oldUrl, newUrl);
                listItem.InnerHtml += link;
            }
            else
            {
                TagBuilder aTagBuilder = new TagBuilder("a");
                aTagBuilder.InnerHtml += text;
                listItem.InnerHtml += aTagBuilder;
            }

            int currentIndex = 0;
            int.TryParse(text, out currentIndex);

            if (currentIndex == dataList.PageIndex)
            {
                listItem.AddCssClass("active");
            }

            return listItem.ToString();
        }

        #endregion
    }
}