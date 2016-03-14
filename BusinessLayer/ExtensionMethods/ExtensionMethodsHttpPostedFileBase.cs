using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLayer.ExtensionMethods
{
    public static class ExtensionMethodsHttpPostedFileBase
    {
        public static bool HasFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
    }
}
