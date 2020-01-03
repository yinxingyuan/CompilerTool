using CompilerTool.Entities;
using System.Collections.Generic;
using CompilerTool.Services.Interfaces;
using CompilerTool.Web.Models;
using MetaShare.Common.Core.CommonService;
using MetaShare.Common.Core.Entities;


namespace CompilerTool.Web.Controllers
{
	public class OrderItemController:CommonController<OrderItem, IOrderItemService, OrderItemModel>
	{


		protected override List<OrderItem> GetBySearchModel(SearchModel pagerSearchModel)
        {
	        if (pagerSearchModel == null) return this.GetPagerData(new Pager { PageIndex = 1, PageSize = PageSize });

            List<OrderItem> lists = this.Service.SelectBy(pagerSearchModel.Pager,new OrderItem { Name = pagerSearchModel.Name }, orderItem => orderItem.Name.Contains(pagerSearchModel.Name));
        return lists;
	}

}
}
