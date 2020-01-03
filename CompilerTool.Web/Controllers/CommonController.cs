using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Reflection;
using MetaShare.Common.Core.Services;
using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Presentation;
using CompilerTool.Web.Models;
using ServiceFactory = MetaShare.Common.Core.CommonService.ServiceFactory;

namespace CompilerTool.Web.Controllers
{
	public class CommonController<TEntity, TService, TModel> : Controller where TEntity : MetaShare.Common.Core.Entities.Common, new() where TService : IPagingService<TEntity> where TModel : CommonModel<TEntity>, new()
	
    {
        #region Const

        public const string DataIsNotExist = "This data does not exist or has been deleted, please refresh the page retry.";
        public const string BaseViewPath = "~/Views/";
        public const string FilenameExtension = ".cshtml";
        public const int PageSize = 15;

        #endregion

        #region Properties

        protected TService Service { get; set; }

        #endregion

        #region Construstor

        protected CommonController()
        {
            this.Service = ServiceFactory.Instance.GetService<TService>();
        }

        #endregion

        #region Methods

        #region Index

        public virtual ActionResult Index(int pageIndex = 1, int pageSize = PageSize)
        {
            Pager pager = new Pager {PageIndex = pageIndex, PageSize = pageSize};

            List<TEntity> entities = this.GetPagerData(pager);

            TargetPager<TEntity> targetPager = new TargetPager<TEntity>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageTotal = pager.PageTotal,
                TotalCounts = pager.TotalCounts,
                Datas = entities
            };

            return this.ToAction("Index", new IndexViewModel<TEntity> {SearchModel  = new SearchModel{Pager = pager} ,TargetPager = targetPager });
        }

        protected virtual List<TEntity> GetPagerData(Pager pager)
        {
            return this.Service.SelectAll(pager);
        }

        protected virtual ActionResult ToAction(string view, object model)
        {
            if (string.IsNullOrEmpty(this.GetCurrentAreaPath()))
            {
                return this.View(view, model);
            }
            return this.View(this.GetViewPath(view), model);
        }

        [HttpPost]
        public virtual ActionResult Index(SearchModel pagerSearchModel)
        {
            pagerSearchModel.Pager = this.GetPagerFormRequest(pagerSearchModel.Pager);
            List<TEntity> entities = this.GetBySearchModel(pagerSearchModel);
            TargetPager<TEntity> targetPager = this.CommonCorePagerToTargetPager(pagerSearchModel.Pager, entities);
            return this.ToAction("Index", new IndexViewModel<TEntity> { SearchModel = pagerSearchModel, TargetPager = targetPager });
        }

        #endregion

        #region Details

        public virtual ActionResult Details(int id)
        {
            TModel model = this.GetModel(id);
            if (model == null)
            {
                return this.Json(new RequestResult() {IsSucceed = false, Message = DataIsNotExist}, JsonRequestBehavior.AllowGet);
            }
            return this.ToAction("Details", model);
        }

        #endregion

        #region Create

        public virtual ActionResult Create()
        {
            TModel model = new TModel();
            this.PreCreate(model);
            this.LoadViewReferenceData(model);

            return this.ToAction("Create", model);
        }

        protected virtual void PreCreate(TModel model)
        {
        }
        [HttpPost]
        public virtual ActionResult Create(TModel model)
        {
            try
            {
                this.LoadViewReferenceData(model);
                if (this.ModelState.IsValid)
                {
                    TEntity entity = new TEntity();
                    this.OnCreating(model);
                    model.PopulateTo(entity);
                    
                    this.OnCreated(entity, model);
                    this.DoInsert(entity);

                    return this.ToActionCreated(entity);
                }

                return this.ToAction("Create", model);
            }
            catch
            {
                return this.ToAction("Create", model);
            }
        }
        protected virtual void OnCreating(TModel model)
        {
        }

        protected virtual void OnCreated(TEntity entity, TModel model)
        {
        }

        protected virtual int DoInsert(TEntity entity)
        {
            return this.Service.Insert(entity, false);
        }

        protected virtual ActionResult ToActionCreated(TEntity entity)
        {
            return this.RedirectToAction("Index");
        }

        #endregion

        #region Edit

        public virtual ActionResult Edit(int id)
        {
            TModel model = this.GetModel(id);
            if (model == null)
            {
                return this.Json(new RequestResult() {IsSucceed = false, Message = DataIsNotExist}, JsonRequestBehavior.AllowGet);
            }
            this.OnEditing(model);
            this.LoadViewReferenceData(model);
            return this.ToAction("Edit", model);
        }

        protected virtual void OnEditing(TModel model)
        {
        }
        [HttpPost]
        public virtual ActionResult Edit(TModel model)
        {
            try
            {
                this.LoadViewReferenceData(model);
                if (this.ModelState.IsValid)
                {
                    TEntity selectEntity = this.GetEntity(model.Id);
                    if (selectEntity == null)
                    {
                        return this.Json(new RequestResult() {IsSucceed = false, Message = DataIsNotExist}, JsonRequestBehavior.AllowGet);
                    }

                    TEntity modifyEntity = new TEntity();
                    this.OnEditing(model);
                    model.PopulateTo(modifyEntity);

                    this.OnUpdated(modifyEntity, selectEntity, model);
                    this.DoUpdate(modifyEntity);

                    return this.ToActionEdited(modifyEntity);
                }

                return this.ToAction("Edit", model);
            }
            catch
            {
                return this.ToAction("Edit", model);
            }
        }

        protected virtual ActionResult ToActionEdited(TEntity modifyEntity)
        {
            return this.RedirectToAction("Index");
        }

        protected virtual void OnUpdated(TEntity newEntity, TEntity oldEntity, TModel model)
        {
        }

        protected virtual int DoUpdate(TEntity entity)
        {
            return this.Service.Update(entity, false);
        }

        #endregion

        #region Delete

        public virtual ActionResult Delete(int id)
        {
            TEntity entity = this.GetEntity(id);
            if (entity == null)
            {
                return this.Json(new RequestResult() {IsSucceed = false, Message = DataIsNotExist}, JsonRequestBehavior.AllowGet);
            }
            if (this.DoDelete(entity) == 1)
            {
                return new JsonResult() {Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            }
            return new JsonResult() {Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        protected virtual int DoDelete(TEntity entity)
        {
            return this.Service.Delete(entity, false);
        }

        public virtual ActionResult SoftDelete(int id)
        {
            TModel model = this.GetModel(id);
            if (model == null)
            {
                return this.Json(new RequestResult() {IsSucceed = false, Message = DataIsNotExist}, JsonRequestBehavior.AllowGet);
            }
            return this.ToAction("SoftDelete", model);
        }

        [HttpPost]
        public virtual ActionResult SoftDelete(TModel model)
        {
            TEntity entity = this.GetEntity(model.Id);
            this.OnDeleting(entity);
            this.Service.Update(entity, false);

            return this.RedirectToAction("Index");
        }

        protected virtual void OnDeleting(TEntity entity)
        {
        }

        #endregion

        #region Basic methods

        protected string GetViewPath(string view)
        {
            string currentAreaPath = this.GetCurrentAreaPath();
            if (!string.IsNullOrWhiteSpace(currentAreaPath))
            {
                return BaseViewPath + this.GetCurrentAreaPath() + "/" + view + FilenameExtension;
            }
            return  view;
        }

        protected virtual string GetCurrentAreaPath()
        {
            return string.Empty;
        }
        protected virtual TEntity GetEntity(int id)
        {
            return this.Service.SelectById(new TEntity {Id = id});
        }
        protected TModel GetModel(int id)
        {
            TEntity entity = this.GetEntity(id);
            if (entity == null)
            {
                return null;
            }
            TModel model = new TModel();
            model.PopulateFrom(entity);
            this.AfterGetPopulateModel(model, entity);
            return model;
        }
        protected virtual TModel AfterGetPopulateModel(TModel model, TEntity entity)
        {
            return model;
        }
        
        protected virtual JsonResult ValidateName(TEntity entity, IList<string> byColumnNames, int entityId)
        {
            List<TEntity> tEntities = this.Service.SelectBy(entity, byColumnNames);
            TEntity item = tEntities.FirstOrDefault();
            if (tEntities.Count > 0 && item != null)
            {
                if (item.Id != entityId)
                {
                    return this.Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            return this.Json(true, JsonRequestBehavior.AllowGet);
        }
        protected string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            ViewEngineResult viewEngineResult = partial ? ViewEngines.Engines.FindPartialView(context, viewPath) : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null) throw new FileNotFoundException("View cannot be found.");

            IView view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result;

            using (StringWriter sw = new StringWriter())
            {
                ViewContext ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        protected TargetPager<TEntity> CommonCorePagerToTargetPager(Pager pager, List<TEntity> entities)
        {
            TargetPager<TEntity> entityPager = new TargetPager<TEntity> {PageIndex = pager.PageIndex, PageSize = pager.PageSize, PageTotal = pager.PageTotal, TotalCounts = pager.TotalCounts, Datas = entities};
            return entityPager;
        }

        protected TargetPager<TModel> CommonCorePagerToTargetPager(Pager pager, List<TModel> models)
        {
            TargetPager<TModel> modelPager = new TargetPager<TModel> {PageIndex = pager.PageIndex, PageSize = pager.PageSize, PageTotal = pager.PageTotal, TotalCounts = pager.TotalCounts, Datas = models};
            return modelPager;
        }

        protected byte[] ReadFileContent(HttpPostedFileBase file)
        {
            if (file == null) return null;
            byte[] fileContent = new byte[file.ContentLength];

            using (Stream stream = file.InputStream)
            {
                stream.Read(fileContent, 0, fileContent.Length);
                return fileContent;
            }
        }

        protected virtual void LoadViewReferenceData(TModel model)
        {
        }

        #endregion

        #region Search
        [HttpPost]
        public virtual ActionResult Search(SearchModel pagerSearchModel)
        {
            pagerSearchModel.Pager = this.GetPagerFormRequest(pagerSearchModel.Pager);
            List<TEntity> entities = this.GetBySearchModel(pagerSearchModel);
            TargetPager<TEntity> targetPager = this.CommonCorePagerToTargetPager(pagerSearchModel.Pager, entities);
            return this.RenderSearchPartialView(new IndexViewModel<TEntity> { SearchModel = pagerSearchModel, TargetPager = targetPager });
        }
        protected virtual ActionResult RenderSearchPartialView(object model)
        {
            return this.PartialView(this.GetViewPath("_DataList"), model);
        }

        protected virtual List<TEntity> GetBySearchModel(SearchModel pagerSearchModel)
        {
            if (pagerSearchModel == null)  return this.GetPagerData(new Pager{PageIndex = 1,PageSize = PageSize });
            return this.GetPagerData(pagerSearchModel.Pager);
        }

        public virtual Pager GetPagerFormRequest(Pager pager)
        {
            if (pager == null) pager = new Pager { PageIndex = 1, PageSize = PageSize };
            pager.PageIndex = pager.PageIndex == 0 ? 1 : pager.PageIndex;
            pager.PageSize = pager.PageSize == 0 ? PageSize : pager.PageSize;

            string pageIndex = this.HttpContext.Request.QueryString["pageIndex"];
            string pageSize = this.HttpContext.Request.QueryString["pageSize"];

            if (string.IsNullOrWhiteSpace(pageIndex)) pageIndex = this.HttpContext.Request.Form["pageIndex"];
            if (!string.IsNullOrWhiteSpace(pageIndex)) pager.PageIndex = Convert.ToInt32(pageIndex);

            if (string.IsNullOrWhiteSpace(pageSize)) pageSize = this.HttpContext.Request.Form["pageSize"];
            if (!string.IsNullOrWhiteSpace(pageSize)) pager.PageSize = Convert.ToInt32(pageSize);
            return pager;
        }

         #endregion
        # region GetEnumValues
  
        protected List<SelectListItem> GetEnumValues(Type type)
        {
            List<SelectListItem> selectListItems  = new List<SelectListItem>();
            
            foreach(int i in Enum.GetValues(type))
            {
                SelectListItem selectListItem = new SelectListItem { Text = this.GetEnumDescription(type,i), Value = i.ToString() };
                selectListItems.Add(selectListItem);
            }
            
            return selectListItems;
        }
        
        public string GetEnumDescription(Type type, int value)
        {
            string name = Enum.GetName(type, value);
            FieldInfo fieldInfo = type.GetField(name);

            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(attrs != null && attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
            return value.ToString();
        }
        
        #endregion
        #endregion
    }
}