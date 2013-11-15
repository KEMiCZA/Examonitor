using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examonitor
{
    public static class Helpers
    {
        public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
        {
            return evaluation ? value : MvcHtmlString.Empty;
        }
    }
}