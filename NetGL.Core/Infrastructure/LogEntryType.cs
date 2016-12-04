using System;

namespace NetGL.Core.Infrastructure {
    [Flags]
    public enum LogEntryType {
        Info = 1,
        Debug = 2,
        Warning = 4,
        Error = 8,
        Exception = 16,
        Fault = 32,
        WTF = 64
    }
}
