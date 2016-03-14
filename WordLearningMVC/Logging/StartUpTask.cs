using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordLearningMVC.Logging
{
    public class StartUpTask
    {
        public bool IsEnabled { get { return true; } }

        public void Configure(string fileName)
        {
            log4net.Config.XmlConfigurator.Configure(
                new System.IO.FileInfo(
                    HttpContext.Current.Server.MapPath(fileName)));
        }
    }
}