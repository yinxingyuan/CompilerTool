using System;
using System.Collections.Generic;
using NUnit.Framework;
using CompilerTool.Entities;
using CompilerTool.TestData;
using CompilerTool.Services.Tests.Common;
using CompilerTool.Services.Interfaces;
namespace CompilerTool.Services.Tests
{

	[TestFixture]
	public class OrderItemServiceTest : CommonServiceTest<OrderItem, IOrderItemService>
	{
		[TestCase]
		public void SelectAllTest()
		{
			List<OrderItem> items = this.Service.SelectAll();
			Assert.AreEqual(OrderItemTestData.OrderItemCount, items.Count);
		}

		[TestCase]
		public void SelectByTest()
		{
			OrderItem itemTest = OrderItemTestData.CreateOrderItem1();

			List<OrderItem> find = this.Service.SelectBy(new OrderItem {Name = itemTest.Name}, new List<string> {"Name"});
			Assert.IsNotNull(find);

			foreach (OrderItem item in find)
			{
				Assert.AreEqual(itemTest.Name, item.Name);
			}
		}

		[TestCase]
		public void SelectByIdTest()
		{
			OrderItem itemTest = OrderItemTestData.CreateOrderItem1();

			OrderItem find = this.Service.SelectById(new OrderItem {Id = itemTest.Id});
			Assert.IsNotNull(find);

			OrderItemTestData.AssertAreEqual(itemTest, find);
		}

		[TestCase]
		public void DeleteTest()
		{
			OrderItem itemTest = OrderItemTestData.CreateOrderItem2();
			int affectedRows = this.Service.Delete(itemTest, true);

			List<OrderItem> items = this.Service.SelectAll();
			Assert.AreEqual(items.Count, OrderItemTestData.OrderItemCount - 1);
			Assert.AreEqual(-1, affectedRows);
		}

		[TestCase]
		public void InsertTest()
		{
			OrderItem itemTest = new OrderItem
			{
				Name = string.Empty, 
				Description = string.Empty, 
			};

			int affectedRows = this.Service.Insert(itemTest, true);

			List<OrderItem> items = this.Service.SelectAll();
			Assert.AreEqual(items.Count, OrderItemTestData.OrderItemCount + 1);
			Assert.AreEqual(1, affectedRows);
		}

		[TestCase]
		public void UpdateTest()
		{
			OrderItem itemTest = OrderItemTestData.CreateOrderItem1();

			OrderItem beforeUpdate = this.Service.SelectById(new OrderItem {Id = itemTest.Id});
			beforeUpdate.Name = string.Empty ;
			this.Service.Update(beforeUpdate, true);

			OrderItem afterUpdate = this.Service.SelectById(new OrderItem {Id = itemTest.Id});
			OrderItemTestData.AssertAreEqual(beforeUpdate, afterUpdate);
		}
	}

}
