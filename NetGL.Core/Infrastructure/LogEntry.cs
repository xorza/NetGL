using System;

namespace NetGL.Core.Infrastructure {
    public class LogEntry {
        public LogEntryType Type { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime Time { get; set; }
        public String StackTrace { get; set; }

        public LogEntry() {
            this.Time = DateTime.Now;
            this.StackTrace = Environment.StackTrace;
        }

        public override string ToString() {
            string format = null;
            if (Message != null) {
                if (Exception != null)
                    format = "{0}: {1}\r\nMessage: {2}\r\nException: {3}";
                else
                    format = "{0}: {1}\r\nMessage: {2}";
            }
            else {
                if (Exception != null)
                    format = "{0}: {1}\r\nException: {3}";
                else
                    format = "{0}: {1}";
            }
            return string.Format(format, Time, Type, Message, Exception);
        }

        public string ToString(bool stackTrace) {
            if (!stackTrace)
                return this.ToString();

            string format = null;
            if (Message != null) {
                if (Exception != null)
                    format = "{0}: {1}\r\nMessage: {2}\r\nException: {3}\r\nStacktrace: {4}";
                else
                    format = "{0}: {1}\r\nMessage: {2}\r\nStacktrace: {4}";
            }
            else {
                if (Exception != null)
                    format = "{0}: {1}\r\nException: {3}\r\nStacktrace: {4}";
                else
                    format = "{0}: {1}\r\nStacktrace: {4}";
            }
            return string.Format(format, Time, Type, Message, Exception, this.StackTrace);
        }
    }
}
