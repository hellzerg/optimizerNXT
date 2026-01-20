using System;
using System.IO;

namespace optimizerNXT {
    public static class Logger {
        private static readonly string LogFile;
        private static readonly object _lock = new object();

        static Logger()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "optimizerNXT-logs");
            LogFile = Path.Combine(basePath, $"optimizerNXT-{timestamp}.log");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message, Exception ex = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += Environment.NewLine + ex;
            }
            Write("ERROR", fullMessage);
        }

        private static void Write(string level, string message)
        {
            lock (_lock)
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}]: {message}";
                ConsoleColor color = Console.ForegroundColor;
                if (level.Equals("error", StringComparison.OrdinalIgnoreCase))
                    color = ConsoleColor.Red;
                else if (level.Equals("warn", StringComparison.OrdinalIgnoreCase))
                    color = ConsoleColor.Yellow;
                else if (level.Equals("info", StringComparison.OrdinalIgnoreCase))
                    color = ConsoleColor.Green;

                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = prevColor;
                try
                {
                    File.AppendAllText(LogFile, line + Environment.NewLine);
                }
                catch { }
            }
        }
    }
}
