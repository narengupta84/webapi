using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Generic
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //All common validation or varification we can do here
        }
    }
}
