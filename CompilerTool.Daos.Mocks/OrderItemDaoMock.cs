using MetaShare.Common.Core.Daos;
using CompilerTool.Entities;
using CompilerTool.Daos.Interfaces;
using CompilerTool.TestData;

namespace CompilerTool.Daos.Mocks
{
	public class OrderItemDaoMock : MockDao<OrderItem>, IOrderItemDao
	{
		public OrderItemDaoMock() : base(OrderItemTestData.CreateOrderItem())
		{
		}
	}
}
