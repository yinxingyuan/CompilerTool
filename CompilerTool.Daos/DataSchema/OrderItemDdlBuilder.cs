using MetaShare.Common.Core.DataSchema;

namespace CompilerTool.Daos.DataSchema
{
	public class OrderItemDdlBuilder : DdlBuilder
	{
		public override string GetSqlCreateTable()
		{
			return @"CREATE TABLE OrderItem(Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,Name nvarchar(255),Description nvarchar(255),Description nvarchar(255),Owner_Id int,Entity_Status int)";
		}

		public override string GetSqlDropTable()
		{
			return @"DROP TABLE OrderItem";
		}

		public override string GetSqlExistTable()
		{
			return @"SELECT COUNT(*) FROM Information_Schema.COLUMNS WHERE TABLE_NAME = 'OrderItem'";
		}
	}
}
