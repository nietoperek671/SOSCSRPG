using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Services
{
    public static class LoggingService
    {
        private const string LOG_FILE_DIRECTORY = "Logs";

        static LoggingService()
        {
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_FILE_DIRECTORY);

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        public static void Log(Exception ex, bool isInnerException = false)
        {
            using (var sw = new StreamWriter(LogFileName(), true))
            {
                sw.WriteLine(isInnerException ? "INNER EXCEPTION" : $"EXCEPTION: {DateTime.Now}");
                sw.WriteLine(new string(isInnerException ? '-' : '=', 40));
                sw.WriteLine($"{ex.Message}");
                sw.WriteLine($"{ex.StackTrace}");

                sw.WriteLine();
            }

            if (ex.InnerException != null)
            {
                Log(ex.InnerException, true);
            }
        }

        private static string LogFileName()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                LOG_FILE_DIRECTORY,
                $"SOSCSRPG_{DateTime.Now:yyyyMMdd}.log");
        }
    }
}