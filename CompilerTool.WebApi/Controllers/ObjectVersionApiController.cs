using System;
using System.Collections.Generic;
using System.Web.Http;
using CompilerTool.WebApi.Models;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services.Version;

namespace CompilerTool.WebApi.Controllers
{
	public class ObjectVersionApiController<TEntity, TService> : CommonApiController<TEntity, TService> where TEntity : ObjectVersion, new() where TService : IObjectVersionService<TEntity>
	{
		 [Route("SelectBySpecifyDate")]
         [HttpGet]
         public virtual RequestResult SelectBySpecifyDate(int id, DateTime timeStamp)
         {
             if (id > 0)
             {
                 TEntity entity = this.Service.SelectById(id, timeStamp);
                 if (entity != null)
                 {
                     return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entity};
                 }
             }
             return new RequestResult {IsSucceed = false, Message = SelectNotFound, Data = null};
         }

		[Route("SelectAllByDateTime")]
        [HttpGet]
        public virtual RequestResult SelectAllByDateTime(DateTime timeStamp)
        {
            
            List<TEntity> entities = this.Service.SelectAllByDateTime(timeStamp);
            if (entities != null)
            {
                return new RequestResult { IsSucceed = true, Message = SelectSuccessful, Data = entities };
            }

            return new RequestResult { IsSucceed = false, Message = SelectNotFound, Data = null };
        }

		[Route("SelectAllByDateTimeWithChild")]
        [HttpGet]
        public virtual RequestResult SelectAllByDateTimeWithChild(DateTime timeStamp)
        {
            List<TEntity> entities = this.Service.SelectAllByDateTime(timeStamp, true);
            if (entities != null)
            {
                return new RequestResult { IsSucceed = true, Message = SelectSuccessful, Data = entities };
            }

            return new RequestResult { IsSucceed = false, Message = SelectNotFound, Data = null };
        }
	}
}
