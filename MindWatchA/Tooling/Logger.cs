using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Rollbar;

namespace MindWatchA.Tooling
{
    public class Logger
    {
        public static readonly string Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "logdata.log");
        private Logger()
        {
        }

        public static void Log(Rollbar.ErrorLevel errorLevel, object message)
        {
            var thread = new Thread(() =>
            {

                var logger = RollbarLocator.RollbarInstance; // .AsBlockingLogger(TimeSpan.FromSeconds(60));
                switch (errorLevel)
                {
                    case ErrorLevel.Critical: logger.Critical(message); break;
                    case ErrorLevel.Debug: logger.Debug(message); break;
                    case ErrorLevel.Error: logger.Error(message); break;
                    case ErrorLevel.Info: logger.Info(message);break;
                    case ErrorLevel.Warning: logger.Warning(message);break;
                }
                var waitingForFileWritten = true;
                while (waitingForFileWritten)
                {
                    try
                    {
                        using (var writer = File.AppendText(Path))
                        {
                            writer.WriteLine($"{DateTime.Now} - {errorLevel}: {message}");
                            writer.Close();
                            waitingForFileWritten = false;
                        }
                    }
                    catch
                    {
                        Task.Delay(200);
                    }
                }
            });
            thread.Start();
        }

        public static void Info(object message)
        {
            Log(ErrorLevel.Info, message);
        }

        public static void Warning(object message)
        {
            Log(ErrorLevel.Warning, message);
        }

        public static void Debug(object message)
        {
            Log(ErrorLevel.Debug, message);
        }

        public static void Critical(object message)
        {
            Log(ErrorLevel.Critical, message);
        }

        public static void Error(object message)
        {
            Log(ErrorLevel.Error, message);
        }
    }
}
