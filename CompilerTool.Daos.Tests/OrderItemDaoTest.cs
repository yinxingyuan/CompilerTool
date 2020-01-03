using System.Collections.Generic;
using NUnit.Framework;
using System;
using CompilerTool.Daos.Interfaces;
using CompilerTool.Daos.DataSchema;
using CompilerTool.TestData;
using CompilerTool.Daos.Tests.Common;
using CompilerTool.Entities;

namespace CompilerTool.Daos.Tests
{
	public class OrderItemDaoTest : CommonDaoTest<OrderItem, IOrderItemDao, OrderItemDdlBuilder>
	{
		public OrderItemDaoTest() : base(OrderItemTestData.CreateOrderItem())
		{
		}

		[TestCase]
		public void SelectAllTest()
		{
			Assert.AreEqual(OrderItemTestData.OrderItemCount, this.Dao.SelectAll(this.Context).Count);
		}

		[TestCase]
		public void SelectByIdTest()
		{
			OrderItem item = OrderItemTestData.CreateOrderItem1();
			OrderItem find = this.Dao.SelectById(this.Context, item);

			Assert.AreEqual(item.Id, find.Id);
			OrderItemTestData.AssertAreEqual(item, find);
		}

		[TestCase]
		public void InsertTest()
		{
			OrderItem item = new OrderItem
			{
				Name = string.Empty, 
				Description = string.Empty, 
			};
			int affectedRows = this.Dao.Insert(this.Context, item);
			Assert.AreEqual(1, affectedRows);

			OrderItem find = this.Dao.SelectById(this.Context, item);
			OrderItemTestData.AssertAreEqual(item, find);

			List<OrderItem> items = this.Dao.SelectAll(this.Context);
			Assert.AreEqual(OrderItemTestData.OrderItemCount + 1, items.Count);
		}

		[TestCase]
		public void UpdateTest()
		{
			OrderItem item = OrderItemTestData.CreateOrderItem1();
			OrderItem beforeUpdate = this.Dao.SelectById(this.Context, item);
			Assert.IsNotNull(beforeUpdate);
			beforeUpdate.Name = string.Empty;

			this.Dao.Update(this.Context, beforeUpdate);

			OrderItem afterUpdate = this.Dao.SelectById(this.Context, beforeUpdate);
			OrderItemTestData.AssertAreEqual(beforeUpdate, afterUpdate);
		}

		[TestCase]
		public void DeleteTest()
		{
			OrderItem item = OrderItemTestData.CreateOrderItem1();
			OrderItem beforedelete = this.Dao.SelectById(this.Context, item);
			Assert.IsNotNull(beforedelete);

			int affectedRows = this.Dao.Delete(this.Context, beforedelete);
			Assert.AreEqual(1, affectedRows);

			OrderItem afterDelete = this.Dao.SelectById(this.Context, beforedelete);
			Assert.IsNull(afterDelete);

			List<OrderItem> items = this.Dao.SelectAll(this.Context);
			Assert.AreEqual(OrderItemTestData.OrderItemCount - 1, items.Count);
		}
	}
}
