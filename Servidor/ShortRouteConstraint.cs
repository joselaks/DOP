namespace Servidor
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class ShortRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                return short.TryParse(value.ToString(), out _);
            }
            return false;
        }
    }

}
