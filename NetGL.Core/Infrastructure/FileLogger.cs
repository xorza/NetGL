using System;
using System.IO;
using System.Text;

namespace NetGL.Core.Infrastructure {
    public class FileLogger : ILogger,IDisposable {
        private  StreamWriter _writer;

        public FileLogger(string fileName) {
            _writer = new StreamWriter(fileName, true, Encoding.UTF8);
        }

        public void Log(LogEntry entry) {
            if (_writer == null)
                return;

            var text = entry.ToString(true);

            _writer.WriteLine(text);
            _writer.WriteLine();
            _writer.Flush();
        }

        public void Dispose() {
            if (_writer == null)
                return;

            _writer.Dispose();
            _writer = null;

            GC.SuppressFinalize(this);
        }
    }
}
