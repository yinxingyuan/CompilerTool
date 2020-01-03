using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MetaShare.Common.Core.Daos;
using MetaShare.Common.Core.Daos.SqlServer;
using CompilerTool.Daos;
using CompilerTool.Services;

namespace CompilerTool.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterArea(Type t, RouteCollection routes, object state)
        {
            AreaRegistration registration = (AreaRegistration)Activator.CreateInstance(t);
            AreaRegistrationContext context = new AreaRegistrationContext(registration.AreaName, routes, state);
            string tNamespace = registration.GetType().Namespace;
            if (tNamespace != null)
                context.Namespaces.Add(tNamespace + ".*");
            registration.RegisterArea(context);
        }

        public static int GetAreaLevelNumber(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return 0; 
           
            string[] names = fullName.Split('.');
            return  names.Length;         
        }

        protected void Application_Start()
        {
            Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(AreaRegistration))).OrderByDescending(r => GetAreaLevelNumber(r.FullName)).ToList().ForEach(r => RegisterArea(r, RouteTable.Routes, null));
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string connectionString = ConfigurationManager.ConnectionStrings["CompilerTool"].ConnectionString;
	        DaoFactory.Instance.ConnectionStringBuilder = new ConnectionStringBuilder(connectionString, typeof(SqlContext)){SqlDialectType = typeof(SqlServerDialect), SqlDialectVersionType = typeof(SqlServerDialectVersion)};
	
            RegisterDaos.RegisterAll(DaoFactory.Instance.ConnectionStringBuilder.SqlDialectType, DaoFactory.Instance.ConnectionStringBuilder.SqlDialectVersionType);
            RegisterServices.RegisterAll();

            this.RegisterView();
        }

        protected void RegisterView()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new AreaViewEngine());
        }
    }
}