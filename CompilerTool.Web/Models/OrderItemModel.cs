using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CompilerTool.Entities;

namespace CompilerTool.Web.Models
{
	public class OrderItemModel: CommonModel<OrderItem>
	{

		public override void PopulateFrom(OrderItem entity)
		{
			if (entity == null) return;
			base.PopulateFrom(entity);

		}

		public override void PopulateTo(OrderItem entity)
		{
			if (entity == null) return;
			base.PopulateTo(entity);

		}
	}
}
