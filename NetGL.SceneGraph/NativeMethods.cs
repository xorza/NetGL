using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NetGL.SceneGraph {
    [SuppressUnmanagedCodeSecurity]
    public delegate void TimerEventHandler(UInt32 id, UInt32 msg, ref UInt32 userCtx, UInt32 rsv1, UInt32 rsv2);

    internal enum MessageFlag : uint {
        Remove = 0x0001
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct Point {
        public Int32 x;
        public Int32 y;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct Message {
        public IntPtr hwnd;
        public UInt32 message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public UInt32 time;
        public Point pt;
    }

    internal enum TimePeriodResult : uint {
        NoError = 0,
        NoCanDo = 97
    }

    internal enum EventType :int{
         TimePeriodic = 1,
         //EVENT_TYPE = TimePeriodic,// + 0x100;   TIME_KILL_SYNCHRONOUS causes a hang ?!
    }

    internal static class NativeMethods {
        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceCounter")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceCounter([Out]out long value);


        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceFrequency")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency([Out]out long value);


        [SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        internal static extern TimePeriodResult TimeBeginPeriod(uint uMilliseconds);


        [SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        internal static extern TimePeriodResult TimeEndPeriod(uint uMilliseconds);

        /// <summary>
        /// A multi media timer with millisecond precision
        /// </summary>
        /// <param name="msDelay">One event every msDelay milliseconds</param>
        /// <param name="msResolution">Timer precision indication (lower value is more precise but resource unfriendly)</param>
        /// <param name="handler">delegate to start</param>
        /// <param name="userCtx">callBack data </param>
        /// <param name="eventType">one event or multiple events</param>
        /// <remarks>Dont forget to call timeKillEvent!</remarks>
        /// <returns>0 on failure or any other value as a timer id to use for timeKillEvent</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeSetEvent")]
        internal static extern UInt32 TimeSetEvent(UInt32 msDelay, UInt32 msResolution, TimerEventHandler handler, ref UInt32 userCtx, EventType eventType);

        /// <summary>
        /// The multi media timer stop function
        /// </summary>
        /// <param name="uTimerID">timer id from timeSetEvent</param>
        /// <remarks>This function stops the timer</remarks>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeKillEvent")]
        internal static extern void TimeKillEvent(UInt32 uTimerID);


        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "PeekMessage")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage([Out]out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, MessageFlag wRemoveMsg);

        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TranslateMessage([In]ref Message lpMsg);

        [DllImport("user32.dll", EntryPoint = "DispatchMessage")]
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr DispatchMessage([In]ref Message lpmsg);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "WaitMessage", CharSet = CharSet.Auto)]
        internal static extern void WaitMessage();
    }
}