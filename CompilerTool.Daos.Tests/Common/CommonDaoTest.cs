using System;
using System.Collections.Generic;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.DataSchema;
using MetaShare.Common.Core.Daos.SqlCe;
using MetaShare.Common.Core.Entities;
using NUnit.Framework;
namespace CompilerTool.Daos.Tests.Common
{
	public class CommonDaoTest<TEntity, TDao, TDdlBuilder>
	where TEntity : MetaShare.Common.Core.Entities.Common
	where TDao : IDao<TEntity>
	where TDdlBuilder : DdlBuilder, new()
	{
		protected TDao Dao { get; set; }
		protected IContext Context { get; set; }

		private DatabaseBuilder DatabaseBuilder { get; set; }
		private IList<TEntity> Entities { get; set; }

		protected CommonDaoTest(IList<TEntity> entities)
		{
			this.Entities = entities;
		}

		[SetUp]
		public void BasicSetUp()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			DaoFactory.Instance.ConnectionStringBuilder = new SqlCeConnectionStringBuilder("CompilerTool.Entities", baseDirectory, null);

			RegisterDaos.RegisterAll(typeof(SqlCeDialect),typeof(SqlCeDialectVersion));
			this.CreateDatabase();

			this.Dao = DaoFactory.Instance.GetDao<TDao>();
			this.Context = DaoFactory.Instance.GetContext(typeof(TDao), true);
		}

		[TearDown]
		public void BasicTearDown()
		{
			this.Context.Rollback();
			this.Context.Dispose();

			this.DropDatabase();

			RegisterDaos.UnRegisterAll(typeof(SqlCeDialect), typeof(SqlCeDialectVersion));
		}

		private void CreateDatabase()
		{
			this.DatabaseBuilder = DaoFactory.Instance.ConnectionStringBuilder.CreateDatabaseBuilder(new List<DdlBuilder>
			{
				new TDdlBuilder(),
			});

			this.DatabaseBuilder.CreateSchema();

			DaoFactory.Instance.Populate(typeof(TDao), this.Entities);
		}

		private void DropDatabase()
		{
			this.DatabaseBuilder.DropSchema();
		}
	}
}
