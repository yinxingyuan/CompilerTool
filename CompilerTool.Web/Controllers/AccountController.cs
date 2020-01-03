using System.Web;
using System.Web.Mvc;
using SSO.Utilities;


namespace CompilerTool.Web.Controllers
{
    public class AccountController : AccountBaseController
    {
        protected override ActionResult ToActionRegistered(RegisterModelBase model)
        {
            if (!string.IsNullOrEmpty(model.OrigUrl))
            {
                return this.Redirect(HttpUtility.UrlDecode(model.OrigUrl));
            }
            return this.RedirectToRoute(new { Controller = "OrderItem", Action = "index", Area =""});
        }

        protected override ActionResult ToActionLogined(LoginModelBase model)
        {
            if (!string.IsNullOrEmpty(model.OrigUrl))
            {
                return this.Redirect(HttpUtility.UrlDecode(model.OrigUrl));
            }
            return this.RedirectToRoute(new { Controller = "OrderItem", Action = "index", Area =""});
        }
    }
}