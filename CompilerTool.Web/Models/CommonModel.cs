using System;
using System.Collections.Generic;
using System.Reflection;
using MetaShare.Common.Core.Entities;

namespace CompilerTool.Web.Models
{
	public class CommonModel<TEntity> where TEntity : MetaShare.Common.Core.Entities.Common
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EntityStatus EntityStatus { get; set; }
        public int OwnerId { get; set; }

        public virtual void PopulateFrom(TEntity entity)
        {
            if (entity == null) return;
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Description = entity.Description;
            this.EntityStatus = entity.EntityStatus;
            this.OwnerId = entity.OwnerId;
        }

        public virtual void PopulateTo(TEntity entity)
        {
            if (entity == null) return;
            entity.Id = this.Id;
            entity.Name = this.Name;
            entity.Description = this.Description;
            entity.EntityStatus = this.EntityStatus;
            entity.OwnerId = this.OwnerId;
        }

		public static T ConvertItem<T, TP>(T common, List<TP> collection, Predicate<TP> predicate, string propertyName)
        {
            if (collection == null) return common;
            if (predicate == null) return common;
            if (propertyName == null) return common;

            PropertyInfo propertyInfo = common.GetType().GetProperty(propertyName);
            if (propertyInfo != null) propertyInfo.SetValue(common, collection.Find(predicate));
            return common;
        }
	}
}
