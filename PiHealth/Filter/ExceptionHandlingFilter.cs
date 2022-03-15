using log4net;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace PiHealth.Web.Filter
{
    public class ExceptionLogFilter : ExceptionFilterAttribute
    {
        ILog _logger = null;
        public override void OnException(ExceptionContext Context)
        {
            _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Error(Context.Exception);
            Context.ExceptionHandled = false;
        }
    }

    public class ExceptionHandlingFilter:IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
