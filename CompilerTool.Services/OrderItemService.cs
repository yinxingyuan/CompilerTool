using System.Collections.Generic;
using MetaShare.Common.Core.Entities;
using CompilerTool.Entities;
using MetaShare.Common.Core.Services;
using CompilerTool.Daos.Interfaces;
using CompilerTool.Services.Interfaces;


namespace CompilerTool.Services
{
	public class OrderItemService : Service<OrderItem>, IOrderItemService
	{
		public OrderItemService() : base(typeof (IOrderItemDao))
		{
		}

	}
}
