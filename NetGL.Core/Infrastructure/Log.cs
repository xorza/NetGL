using System;
using System.Collections.Generic;

namespace NetGL.Core.Infrastructure {
    public static class Log {
        private static readonly List<ILogger> _loggers = new List<ILogger>();

        public static void Info(string message) {
            if (_loggers.Count == 0)
                return;
            if (string.IsNullOrWhiteSpace(message))
                return;

            var entry = new LogEntry() { Type = LogEntryType.Info, Message = message };

            Add(entry);
        }
        public static void Info(object message) {
            if (message == null)
                throw new NullReferenceException("message");

            Info(message.ToString());
        }
        public static void Warning(string message) {
            if (_loggers.Count == 0)
                return;
            if (string.IsNullOrWhiteSpace(message))
                return;

            var entry = new LogEntry() { Type = LogEntryType.Warning, Message = message };

            Add(entry);
        }
        public static void Error(string message) {
            if (_loggers.Count == 0)
                return;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.Trim();

            var entry = new LogEntry() { Type = LogEntryType.Error, Message = message };

            Add(entry);
        }
        public static void Exception(Exception ex) {
            if (_loggers.Count == 0)
                return;

            var entry = new LogEntry() { Type = LogEntryType.Exception, Exception = ex };
            Add(entry);
        }

        public static void AddLogger(ILogger logger) {
            Assert.False(_loggers.Contains(logger));

            _loggers.Add(logger);
        }
        public static void RemoveLogger(ILogger logger) {
            _loggers.Remove(logger);
        }

        private static void Add(LogEntry entry) {
            if (_loggers.Count == 0)
                return;

            lock (_loggers) {
                List<Exception> logExceptions = null;
                
                for (int i = _loggers.Count - 1; i >= 0; i--) {
                    var logger = _loggers[i];
                    try {
                        logger.Log(entry);
                    }
                    catch (Exception ex) {
                        _loggers.RemoveAt(i);

                        if (logExceptions == null)
                            logExceptions = new List<Exception>();
                        logExceptions.Add(ex);
                    }
                }

                if (logExceptions != null)
                    logExceptions.ForEach(Exception);
            }
        }
    }
}
