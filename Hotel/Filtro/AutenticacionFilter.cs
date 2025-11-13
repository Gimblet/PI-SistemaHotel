using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Filtro
{
    public class AutenticacionFilter:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("eUsuario")))
            {
                context.Result = new RedirectToActionResult("Login", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
