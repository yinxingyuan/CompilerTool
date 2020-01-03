using MetaShare.Common.Core.Entities;
using MetaShare.Common.Core.Presentation;

namespace CompilerTool.Web.Models
{
	public class IndexViewModel<TEntity> where TEntity : MetaShare.Common.Core.Entities.Common
	{
		public SearchModel SearchModel { get; set; }
		public TargetPager<TEntity> TargetPager { get; set; }
		}
	}
