using System;
using System.Diagnostics.Contracts;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace AlohaService.Helpers
{
    public static class LogHelper
    {
        public static ILog GetLogger()
        {
            return LogManager.GetLogger("Aloha.Service");
        }

        public static ILog GetLogger(object owner)
        {
            Contract.Requires(owner != null);
            return LogManager.GetLogger(owner.GetType());
        }

        /// <summary>
        /// Simple configuration to use Log4Net without configuration in app.config
        /// </summary>
        public static void Configure()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            FileAppender fileAppender = new FileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = "Logs/log.txt";
            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = "%d [%2%t] %-5p %m%n";
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(fileAppender);
        }

        private static bool? _isConfigured;
        /// <summary>
        /// Lazy check if Log4Net is configured.
        /// </summary>
        private static bool IsConfigured()
        {
            if (_isConfigured == null)
            {
                try
                {
                    var log = LogManager.GetLogger(typeof(LogHelper));
                    _isConfigured = log != null;
                }
                catch
                {
                    _isConfigured = false;
                }
            }
            return _isConfigured.Value;
        }

        public static void Debug(string message)
        {
            if (IsConfigured())
            {
                var log = LogManager.GetLogger(typeof(LogHelper));
                log.Debug(message);
            }
            else
            {
                Console.WriteLine("{0} : {1}", DateTime.Now, message);
            }
        }

        public static void Error(string message)
        {
            if (IsConfigured())
            {
                var log = LogManager.GetLogger(typeof(LogHelper));
                log.Error(message);
            }
            else
            {
                Console.WriteLine("{0} : {1}", DateTime.Now, message);
            }
        }

        public static void Debug(string message, Exception exception)
        {
            if (IsConfigured())
            {
                var log = LogManager.GetLogger(typeof(LogHelper));
                log.Debug(message, exception);
            }
            else
            {
                Console.WriteLine("{0} : {1} - {2} - {3}",
                    DateTime.Now, message, exception.Message, exception.StackTrace);
            }
        }

        public static void Error(string message, Exception exception)
        {
            if (IsConfigured())
            {
                var log = LogManager.GetLogger(typeof(LogHelper));
                log.Error(message, exception);
            }
            else
            {
                Console.WriteLine("{0} : {1} - {2} - {3}",
                    DateTime.Now, message, exception.Message, exception.StackTrace);
            }
        }
    }
}