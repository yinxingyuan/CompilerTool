using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services.Version;
using CompilerTool.Web.Models;

namespace CompilerTool.Web.Controllers
{
	public class ObjectVersionController<TEntity, TService, TModel> : CommonController<TEntity, TService, TModel> where TEntity : ObjectVersion, new() where TService : IObjectVersionService<TEntity> where TModel : CommonModel<TEntity>, new()
	{
		public ActionResult ViewHistories(int id)
        {
            List<TEntity> entities = this.Service.SelectAllVersions(id);
            return this.ToAction("Histories", entities);
        }

        public ActionResult ViewOneHistoryVersion(int systemId)
        {
            TEntity entity = this.Service.SelectBySystemId(systemId);
            if (entity == null)
            {
                return this.Json(new RequestResult() { IsSucceed = false, Message = DataIsNotExist }, JsonRequestBehavior.AllowGet);
            }
            TModel model = new TModel();
            model.PopulateFrom(entity);

            return this.ToAction("OneVersion", model);
        }
	}
}
