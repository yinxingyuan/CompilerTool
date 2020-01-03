using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using MetaShare.Common.Core.Presentation;
using CompilerTool.WebApi.Models;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Services;
using ServiceFactory = MetaShare.Common.Core.CommonService.ServiceFactory;

namespace CompilerTool.WebApi.Controllers
{
	public class CommonApiController<TEntity, TService> : ApiController where TEntity : MetaShare.Common.Core.Entities.Common, new() where TService : IPagingService<TEntity>
	{
		#region Consts
        public const string SearchFail = "Search failed!";
        public const string SearchSuccessful = "Search Successful!";
        public const string DeleteFailed = "Delete failed!";
        public const string DeleteSuccessful = "Delete Successful!";
        public const string InsertFailed = "Insert failed!";
        public const string InsertSuccessful = "Insert Successful!";
        public const string SelectFailed = "Select failed!";
        public const string SelectNotFound = "Can not found！";
        public const string SelectSuccessful = "Select Successful!";
        public const string UpdateFailed = "Update failed!";
        public const string UpdateSuccessful = "Update Successful!";
        public const int PageSize = 10; 
        #endregion

		protected TService Service  { get; set; }

		protected CommonApiController()
        {
            this.Service = ServiceFactory.Instance.GetService<TService>();
        }

		[Route("SelectById")]
        [HttpGet]
        public virtual RequestResult SelectById(int id)
        {
            TEntity entity = this.Service.SelectById(new TEntity {Id = id});
            if (entity != null)
            {
                    return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entity};
            }
            return new RequestResult {IsSucceed = false, Message = SelectNotFound, Data = null};
        }
		[Route("SelectByIdWithChild")]
        [HttpGet]
        public virtual RequestResult SelectByIdWithChild(int id, bool isAggregatedChildren)
        {
            TEntity entity = this.Service.SelectById(new TEntity {Id = id},true);
            if (entity != null)
            {
                    return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entity};
            }
            return new RequestResult {IsSucceed = false, Message = SelectNotFound, Data = null};
        }
		[Route("SelectAll")]
        [HttpGet]
        public virtual RequestResult SelectAll()
        {
            try
            {
                List<TEntity> entities = this.Service.SelectAll();
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entities};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectNotFound, Data = null};
            }
        }

		[Route("SelectAllWithChild")]
        [HttpGet]
        public virtual RequestResult SelectAllWithChild()
        {
            try
            {
                List<TEntity> entities = this.Service.SelectAll(true);
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entities};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectNotFound, Data = null};
            }
        }

		[Route("SelectBy")]
        [HttpPost]
        public virtual RequestResult SelectBy(RequestData requestData)
        {
            RequestData<TEntity> data = new RequestData<TEntity>();
            data.Deserialize(requestData);
            try
            {
                List<TEntity> entities = this.Service.SelectBy(data.Entity, data.Columns);
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = entities};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectFailed, Data = null};
            }
        }

		[Route("Insert")]
        [HttpPost]
        public virtual RequestResult Insert([FromBody] TEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    
                    if (this.Service.Insert(entity, false) > 0)
                    {
                        return new RequestResult {IsSucceed = true, Message = InsertSuccessful, Data = entity};
                    }
                }
                return new RequestResult {IsSucceed = false, Message = InsertFailed, Data = entity};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = InsertFailed, Data = entity};
            }
        }

		[Route("Update")]
        [HttpPost]
        public virtual RequestResult Update(TEntity entity)
        {
            if (entity != null)
            {
                if (this.Service.Update(entity, false) > 0)
                {
                    return new RequestResult {IsSucceed = true, Message = UpdateSuccessful, Data = entity};
                }
            }
            return new RequestResult {IsSucceed = false, Message = UpdateFailed, Data = entity};
        }

		

		[Route("Delete")]
        [HttpPost]
        public virtual RequestResult Delete(TEntity entity)
        {
            if (this.Service.Delete(entity, false) > 0)
            {
                return new RequestResult {IsSucceed = true, Message = DeleteSuccessful, Data = entity};
            }
            return new RequestResult {IsSucceed = false, Message = DeleteFailed, Data = entity};
        }

		[Route("Delete")]
        [HttpGet]
        public virtual RequestResult Delete(int id)
        {
            if (this.Service.Delete(new TEntity {Id = id}, false) > 0)
            {
                return new RequestResult {IsSucceed = true, Message = DeleteSuccessful, Data = new TEntity {Id = id}};
            }
            return new RequestResult {IsSucceed = false, Message = DeleteFailed, Data = new TEntity {Id = id}};
        }

		[Route("SelectPagerByColumns")]
        [HttpPost]
        public virtual RequestResult SelectPagerByColumns(RequestData requestData)
        {
            RequestData<TEntity> data = new RequestData<TEntity>();
            data.Deserialize(requestData);
            if (data.Pager == null) data.Pager = new Pager {PageIndex = 1, PageSize = PageSize};
            if (data.Pager.PageSize == 0) data.Pager.PageSize = PageSize;
            try
            {
                Pager pager = data.Pager;
                List<TEntity> entities = this.Service.SelectBy(data.Pager, data.Entity, data.Columns);
                TargetPager<TEntity> targetPager = new TargetPager<TEntity> {PageSize = pager.PageSize, PageIndex = pager.PageIndex, PageTotal = pager.PageTotal, Datas = entities};
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = targetPager};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectFailed, Data = requestData};
            }
        }

		[Route("SelectAllByPager")]
        [HttpPost]
        public virtual RequestResult SelectAllByPager(RequestData requestData)
        {
            RequestData<TEntity> data = new RequestData<TEntity>();
            data.Deserialize(requestData);
            if (data.Pager == null) data.Pager = new Pager {PageIndex = 1, PageSize = PageSize};
            if (data.Pager.PageSize == 0) data.Pager.PageSize = PageSize;
            try
            {
                Pager pager = data.Pager;
                List<TEntity> entities = this.Service.SelectAll(data.Pager);
                TargetPager<TEntity> targetPager = new TargetPager<TEntity> {PageSize = pager.PageSize, PageIndex = pager.PageIndex, PageTotal = pager.PageTotal, Datas = entities};
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = targetPager};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectFailed, Data = requestData};
            }
        }

		[Route("SelectAllByPagerWithChild")]
        [HttpPost]
        public virtual RequestResult SelectAllByPagerWithChild(RequestData requestData)
        {
            RequestData<TEntity> data = new RequestData<TEntity>();
            data.Deserialize(requestData);
            if (data.Pager == null) data.Pager = new Pager {PageIndex = 1, PageSize = PageSize};
            if (data.Pager.PageSize == 0) data.Pager.PageSize = PageSize;
            try
            {
                Pager pager = data.Pager;
                List<TEntity> entities = this.Service.SelectAll(data.Pager, true);
                TargetPager<TEntity> targetPager = new TargetPager<TEntity> {PageSize = pager.PageSize, PageIndex = pager.PageIndex, PageTotal = pager.PageTotal, Datas = entities};
                return new RequestResult {IsSucceed = true, Message = SelectSuccessful, Data = targetPager};
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SelectFailed, Data = requestData};
            }
        }

		#region Api For App

		[Route("SelectAllByColumns")]
        [HttpPost]
        public virtual RequestResult SelectAllByColumns(TEntity entity)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpContext.Current.Request.Form;
                TEntity newEntity = Utility.GetEntityFromRequest(entity, nameValueCollection) as TEntity;

                List<string> allKeys = Utility.GetColumnStrings(newEntity, nameValueCollection.AllKeys);
   
                List<TEntity> entities = this.Service.SelectBy(newEntity, allKeys);
                if(entities != null)
                {
                    return new RequestResult { IsSucceed = true, Message = SelectSuccessful, Data = entities };
                }
                return new RequestResult { IsSucceed = true, Message = SelectNotFound, Data = null };
            }
            catch (Exception)
            {
                return new RequestResult { IsSucceed = false, Message = SelectFailed, Data = null };
            }
        }

		[Route("SelectByColumns")]
        [HttpPost]
        public virtual RequestResult SelectByColumns(TEntity entity)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpContext.Current.Request.Form;
                TEntity newEntity = Utility.GetEntityFromRequest(entity, nameValueCollection) as TEntity;

                List<string> allKeys = Utility.GetColumnStrings(newEntity, nameValueCollection.AllKeys);
   
                TEntity find = this.Service.SelectBy(newEntity, allKeys).FirstOrDefault();
                if(find != null)
                {
                    return new RequestResult { IsSucceed = true, Message = SelectSuccessful, Data = find };
                }
                return new RequestResult { IsSucceed = true, Message = SelectNotFound, Data = null };
            }
            catch (Exception)
            {
                return new RequestResult { IsSucceed = false, Message = SelectFailed, Data = null };
            }
        }

		[Route("Add")]
        [HttpPost]
        public virtual RequestResult Add([FromBody] TEntity entity)
        {
            if(entity == null) entity = Utility.GetEntityFromRequest(new TEntity(), HttpContext.Current.Request.Form) as TEntity;
            
            try
            {
                if(entity != null)
                { 
                     
                    this.AdditionalInsertLogic(entity);                 
                    if (this.Service.Insert(entity, false) > 0)
                    {
                        return new RequestResult { IsSucceed = true, Message = InsertSuccessful, Data = entity };
                    }
                }
                return new RequestResult { IsSucceed = false, Message = InsertFailed, Data = entity};
            }
            catch(Exception)
            {
                return new RequestResult {IsSucceed = false, Message = InsertFailed, Data = entity};               
            }
        }

		protected virtual void AdditionalInsertLogic(TEntity entity)
        {
        }

		[Route("Edit")]
        [HttpPost]
        public virtual RequestResult Edit(TEntity entity)
        {
            string id = HttpContext.Current.Request.Form["Id"];
            if(string.IsNullOrWhiteSpace(id)) throw new Exception("Update entity must have the Id.");
       
            try
            {
                NameValueCollection nameValueCollection = HttpContext.Current.Request.Form;
                TEntity newEntity = this.Service.SelectById(new TEntity() { Id = Convert.ToInt32(id) });
       
                ICollection<string> keys = nameValueCollection.AllKeys;
  
                Utility.UpdateEntityValue(newEntity, nameValueCollection, this.RemoveByteArrayKey(newEntity, keys), Utility.ConvertStringToPropertyTypeValue);
                this.AdditionalUpdateLogic(entity, newEntity);
                if(this.Service.Update(newEntity, false) > 0)
                {
                    return new RequestResult { IsSucceed = true, Message = UpdateSuccessful, Data = newEntity};
                }
                return new RequestResult { IsSucceed = false, Message = UpdateFailed, Data = newEntity };
            }
            catch(Exception)
            {
                return new RequestResult {IsSucceed = false, Message = UpdateFailed, Data = null};
            }        
        }

		[Route("UpdateFromBody")]
        [HttpPost]       
        public virtual RequestResult UpdateFromBody([FromBody] dynamic entity)
        {
            string id = HttpContext.Current.Request.Form["Id"];
            if(string.IsNullOrWhiteSpace(id)) throw new Exception("Update entity must have the Id.");
       
            try
            {
                TEntity newEntity = this.Service.SelectById(new TEntity() { Id = Convert.ToInt32(id) });
       
                ICollection<string> keys = Utility.GetJsonKeys(entity);
  
                Utility.UpdateEntityValue(newEntity, entity,  keys, new Utility.GetPropertyValueByRequestKey(Utility.ConvertJTokenToPropertyTypeValue));
                this.AdditionalUpdateLogic(entity, newEntity);
                if(this.Service.Update(newEntity, false) > 0)
                {
                    return new RequestResult {IsSucceed = true, Message = UpdateSuccessful, Data = newEntity};
                }
                return new RequestResult { IsSucceed = false, Message = UpdateFailed, Data = newEntity };
            }
            catch(Exception)
            {
                return new RequestResult {IsSucceed = false, Message = UpdateFailed, Data = null};
            }        
        }

		protected virtual void AdditionalUpdateLogic(TEntity entity, TEntity newEntity)
        {
        }

		protected virtual void AdditionalUpdateLogic(dynamic entity, TEntity newEntity)
        {
        }

		[HttpPost]
        public virtual RequestResult SelectPagerByColumns(dynamic pagerSearch)
        {
            try
            {
                Pager pager = Utility.ConvertJTokenToPropertyTypeValue(pagerSearch, "Pager", typeof(Pager));
                if (pager == null) throw new Exception("Pager info must be konwn.");
                TEntity newEntity = Utility.ConvertJTokenToPropertyTypeValue(pagerSearch, "Entity", new TEntity().GetType()) as TEntity;
                if (newEntity != null)
                {
                    dynamic keyValue = pagerSearch.GetValue("Entity");

                    ICollection<string> keys = Utility.GetJsonKeys(keyValue);
                    List<string> columns = Utility.GetColumnStrings(newEntity, keys);
                    List<TEntity> entities = this.Service.SelectBy(pager, newEntity, columns);
                    TargetPager<TEntity> targetPager = new TargetPager<TEntity> {PageSize = pager.PageSize, PageIndex = pager.PageIndex, PageTotal = pager.PageTotal, Datas = entities};

                    return new RequestResult {IsSucceed = true, Message = SearchSuccessful, Data = targetPager};
                }
                else
                {
                    List<TEntity> entities = this.Service.SelectAll(pager);
                    TargetPager<TEntity> targetPager = new TargetPager<TEntity> {PageSize = pager.PageSize, PageIndex = pager.PageIndex, PageTotal = pager.PageTotal, Datas = entities};

                    return new RequestResult {IsSucceed = true, Message = SearchSuccessful, Data = targetPager};
                }
            }
            catch (Exception)
            {
                return new RequestResult {IsSucceed = false, Message = SearchFail, Data = pagerSearch};
            }
        }

		protected virtual ICollection<string> RemoveByteArrayKey(TEntity fileSource,ICollection<string> keys)
        {
            if (keys == null) return null;
            
            List<string> byteArrayPropertyKeys = this.GetByteArrayPropertyKeys(fileSource);
            
            if (byteArrayPropertyKeys == null) return keys;
    
            List<string> newKeys = new List<string>();
            foreach (string propertyKey in keys)
            {
                if (byteArrayPropertyKeys.Contains(propertyKey)) continue;
                newKeys.Add(propertyKey);
            }

            return newKeys;
        }

		protected virtual List<string> GetByteArrayPropertyKeys(TEntity fileSource)
        {
            if (fileSource == null) throw new ArgumentNullException("fileSource");
 
            List<string> bytePropertyNames = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(TEntity).GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(Byte[]) || propertyInfo.PropertyType == typeof(byte[]))
                {
                    bytePropertyNames.Add(propertyInfo.Name);
                }
            }

            if (bytePropertyNames.Any()) return bytePropertyNames;
            return null;
        }

		#endregion

	}
}
