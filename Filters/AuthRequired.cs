// AuthRequired.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace VideojuegosStore.Filters

{
    public class AdminRequiredAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("UserRole") != "Administrador")
            {
                context.Result = new RedirectToPageResult("/Login");
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
    }

    public class ClientRequiredAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("UserRole") != "Cliente")
            {
                context.Result = new RedirectToPageResult("/Login");
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
    }
}
