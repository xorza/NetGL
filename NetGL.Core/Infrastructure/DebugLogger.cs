using System.Diagnostics;

namespace NetGL.Core.Infrastructure {
    public class DebugLogger : ILogger {
        public void Log(LogEntry entry) {
            Debug.WriteLine(entry.ToString());
        }
    }
}
