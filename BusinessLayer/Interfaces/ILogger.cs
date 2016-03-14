using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Info(string message, Dictionary<string, string> data);
        void Warn(string message);
        void Warn(string message, Dictionary<string, string> data);
        void Debug(string message);
        void Debug(string message, Dictionary<string, string> data);
        void Error(string message);
        void Error(string message, Exception exception);
        void Error(string message, Exception exception, Dictionary<string, string> data);
        void Fatal(string message);
        void Fatal(string message, Exception exception);
        void Fatal(string message, Exception exception, Dictionary<string, string> data);
    }
}
