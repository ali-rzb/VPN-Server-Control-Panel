using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Haviro
{
    public class DotSeparatedParameterConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                // Check if the parameter value contains a dot
                string valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                return !valueString.Contains(".");
            }
            return true;
        }
    }
}