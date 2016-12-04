using System;
using System.Diagnostics;
using NetGL.Core.Helpers;
using System.Runtime.CompilerServices;

public static class Assert {
    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void NotNull<T>(T obj, string message) where T : class {
        if (obj == null) throw new AssertException(message);
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void NotNull<T>(T obj) where T : class {
        if (obj == null) throw new AssertException();
    }

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    
    public static void NotNull<T>(Nullable<T> obj) where T : struct {
        if (!obj.HasValue)
            throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void True(bool condition) {
        if (!condition) throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void False(bool condition) {
        if (condition) throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void Null<T>(T obj) where T : class {
        if (obj != null) throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void Null<T>(Nullable<T> obj) where T : struct {
        if (obj.HasValue)
            throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void Fail() {
        throw new AssertException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void NotImplemented() {
        throw new NotImplementedException();
    }

    [DebuggerStepThrough]
    [Conditional("DEBUG")]
    
    public static void NotEmpty(string s) {
        if (string.IsNullOrEmpty(s))
            throw new AssertException();
    }
}