using Microsoft.AspNetCore.Mvc.Rendering;

namespace STD.Helpers
{
    public static class MenuHelper
    {
        public static string IsActive(this IHtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
        {
            var currentController = html.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = html.ViewContext.RouteData.Values["action"] as string;

            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
            var acceptedActions = (actions ?? currentAction ?? "").Split(',');

            return acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction)
                ? cssClass
                : "";
        }

        public static string IsMenuOpen(this IHtmlHelper html, string controllers = "", string cssClass = "menu-open")
        {
            var currentController = html.ViewContext.RouteData.Values["controller"] as string;
            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');

            return acceptedControllers.Contains(currentController)
                ? cssClass
                : "";
        }
    }
}