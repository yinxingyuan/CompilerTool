using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CompilerTool.WebApi
{
    public class NamespaceHttpControllerSelector : DefaultHttpControllerSelector
    {
        private const string NamespaceRouteVariableName = "namespaceName";
        private readonly HttpConfiguration _configuration;
        private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerCache;

        public NamespaceHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            this._configuration = configuration;
            this._apiControllerCache = new Lazy<ConcurrentDictionary<string, Type>>(
                new Func<ConcurrentDictionary<string, Type>>(this.InitializeApiControllerCache));
        }

        private ConcurrentDictionary<string, Type> InitializeApiControllerCache()
        {
            IAssembliesResolver assembliesResolver = this._configuration.Services.GetAssembliesResolver();
            var types = this._configuration.Services.GetHttpControllerTypeResolver()
                .GetControllerTypes(assembliesResolver).ToDictionary(t => t.FullName, t => t);

            return new ConcurrentDictionary<string, Type>(types);
        }

        public IEnumerable<string> GetControllerFullName(HttpRequestMessage request, string controllerName)
        {
            object namespaceName;
            var data = request.GetRouteData();
            IEnumerable<string> keys = this._apiControllerCache.Value.ToDictionary<KeyValuePair<string, Type>, string, Type>(t => t.Key,
                t => t.Value, StringComparer.CurrentCultureIgnoreCase).Keys.ToList();

            if (!data.Values.TryGetValue(NamespaceRouteVariableName, out namespaceName))
            {
                return from k in keys
                    where k.EndsWith(string.Format(".{0}{1}", controllerName,
                        ControllerSuffix), StringComparison.CurrentCultureIgnoreCase)
                    select k;
            }

            string[] namespaces = (string[]) namespaceName;
            return from n in namespaces
                join k in keys on string.Format("{0}.{1}{2}", n, controllerName,
                    ControllerSuffix).ToLower() equals k.ToLower()
                select k;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            Type type;
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            string controllerName = this.GetControllerName(request);
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("No route providing a controller name was found to match request URI '{0}'",
                        new object[] {request.RequestUri})));
            }
            IEnumerable<string> fullNames = this.GetControllerFullName(request, controllerName);
            IEnumerable<string> enumerable = fullNames as string[] ?? fullNames.ToArray();
            if (!enumerable.Any())
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("No route providing a controller name was found to match request URI '{0}'",
                        new object[] {request.RequestUri})));
            }

            if (this._apiControllerCache.Value.TryGetValue(enumerable.First(), out type))
            {
                return new HttpControllerDescriptor(this._configuration, controllerName, type);
            }
            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound,
                string.Format("No route providing a controller name was found to match request URI '{0}'",
                    new object[] {request.RequestUri})));
        }
    }
}