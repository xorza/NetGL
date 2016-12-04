using System;
using System.Windows;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace NetGL.Constructor.Infrastructure
{
    internal class Win32Window : IWin32Window
    {
        public IntPtr Handle { get; private set; }

        public Win32Window(Window wnd)
        {
            var interopHelper = new WindowInteropHelper(wnd);
            this.Handle = interopHelper.Handle;
        }
    }
}