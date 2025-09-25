using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Reports.Service
{
    public static class ReportFileLogger
    {
        private static string _wwwRootPath;
        private static string LogFilePath => Path.Combine(_wwwRootPath, "Logs", $"ReportLogs_{DateTime.Now:yyyyMMdd}.txt");
        public static void Init(string wwwRootPath)
        {
            _wwwRootPath = wwwRootPath;
        }

        public static void Log(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(_wwwRootPath))
                    throw new InvalidOperationException("Logger not initialized. Call Init(wwwRootPath) first.");

                string logDir = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                using (StreamWriter writer = File.AppendText(LogFilePath))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
                }
            }
            catch
            {

            }
        }
    }
}
