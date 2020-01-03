using MetaShare.Common.Core.Daos;
using CompilerTool.Daos.Interfaces;

namespace CompilerTool.Daos.Mocks
{
	public class RegisterDaoMocks
	{
		public static void RegisterAll()
		{
			Register(DaoFactory.Instance, true);
		}

		public static void UnRegisterAll()
		{
			Register(DaoFactory.Instance, false);
		}

		public static void Register(DaoFactory factory, bool isRegister)
		{
			factory.Register(typeof(IOrderItemDao), new OrderItemDaoMock(), isRegister);
		}
	}
}
