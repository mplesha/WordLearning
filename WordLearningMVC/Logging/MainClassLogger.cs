using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Web.Hosting;
using BusinessLayer.Interfaces;

namespace WordLearningMVC.Logging
{
    public class MainClassLogger : ILogger
    {
        protected ILog Logger;
        protected string _name;
        private bool _isDebugEnabled;

        public MainClassLogger() : this("Log4NetLogger") { }

        public MainClassLogger(string name)
        {
            _name = name;
            Logger = LogManager.GetLogger(name);
            _isDebugEnabled = Logger.IsDebugEnabled;
        }

        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                Logger = LogManager.GetLogger(_name);
            }
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Info(string message, Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                MDC.Set(item.Key, item.Value);
            }

            Logger.Info(message);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        public void Warn(string message, Dictionary<string, string> data)
        {
            MdcSetter(data);
            Logger.Warn(message);
        }

        public void Debug(string message)
        {
            if (_isDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        public void Debug(string message, Dictionary<string, string> data)
        {
            MdcSetter(data);
            Logger.Debug(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            Logger.Error(message, exception);
        }

        public void Error(string message, Exception exception, Dictionary<string, string> data)
        {
            MdcSetter(data);
            Logger.Error(message, exception);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            Logger.Error(message, exception);
        }

        public void Fatal(string message, Exception exception, Dictionary<string, string> data)
        {
            MdcSetter(data);
            Logger.Error(message, exception);
        }

        private void MdcSetter(Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                MDC.Set(item.Key, item.Value);
            }
        }

        internal void Error()
        {
            throw new NotImplementedException();
        }
    }

    public class AspNetRollingFileAppender : log4net.Appender.RollingFileAppender
    {
        public override string File
        {
            get { return base.File; }
            set { base.File = HostingEnvironment.MapPath(value); }
        }
    }

}