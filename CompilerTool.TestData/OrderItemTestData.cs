using System;
using System.Collections.Generic;
using NUnit.Framework;
using CompilerTool.Entities;

namespace CompilerTool.TestData
{
	public class OrderItemTestData
	{
		public const int OrderItemCount = 3;

		 public static List<OrderItem> CreateOrderItem()
		{
			return new List<OrderItem>
			{
				CreateOrderItem1(),
				CreateOrderItem2(),
				CreateOrderItem3(),
			};
		}

		 public static OrderItem CreateOrderItem1()
		{
			return new OrderItem
			{
					Id = 1, 
					Name = string.Empty, 
					Description = string.Empty, 
			};
		}
		 public static OrderItem CreateOrderItem2()
		{
			return new OrderItem
			{
					Id = 2, 
					Name = string.Empty, 
					Description = string.Empty, 
			};
		}
		 public static OrderItem CreateOrderItem3()
		{
			return new OrderItem
			{
					Id = 3, 
					Name = string.Empty, 
					Description = string.Empty, 
			};
		}
		public static void AssertAreEqual(OrderItem expected, OrderItem actual)
		{
			Assert.AreEqual(expected.Name, actual.Name);
			Assert.AreEqual(expected.Description, actual.Description);
		}
	}
}
