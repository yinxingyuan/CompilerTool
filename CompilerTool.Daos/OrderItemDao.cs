using System;
using System.Data;
using MetaShare.Common.Core.Daos;
using CompilerTool.Daos.Interfaces;
using CompilerTool.Entities;

namespace CompilerTool.Daos
{
	public class OrderItemDao : CommonObjectDao<OrderItem>, IOrderItemDao
	{
		public class OrderItemSqlBuilder : ObjectSqlBuilder
		{
			public OrderItemSqlBuilder(SqlDialect sqlDialect) : base(sqlDialect,"OrderItem")
			{
				this.SqlInsert = "INSERT INTO OrderItem (" + this.SqlBaseFieldInsertFront + ") VALUES (" + this.SqlBaseFieldInsertBack + ")";
				this.SqlUpdate = "UPDATE OrderItem SET " + this.SqlBaseFieldUpdate + " WHERE Id=@Id";
			}
		}

		public class OrderItemResultHandler : CommonObjectResultHandler<OrderItem>
		{
			public override void GetColumnValues(IDataReader reader, OrderItem item)
			{
				base.GetColumnValues(reader, item);
			}

			public override void AddInsertParameters(IContext context, IDbCommand command, OrderItem item)
			{
				base.AddInsertParameters(context, command, item);
			}
		}

		public OrderItemDao(SqlDialect sqlDialect) : base(new OrderItemSqlBuilder(sqlDialect), new OrderItemResultHandler())
		{
		}

		public OrderItemDao(SqlDialect sqlDialect, string schemaConnectionString) : base(new OrderItemSqlBuilder(sqlDialect), new OrderItemResultHandler(), schemaConnectionString)
		{
		}
	}
}
