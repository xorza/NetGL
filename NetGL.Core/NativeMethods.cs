using NetGL.Core.Types;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NetGL.Core {
    internal static class NativeMethods {
        private const string
            Kernel32 = "kernel32.dll",
            Opengl32 = "opengl32.dll",
            Gdi32 = "gdi32.dll",
            User32 = "user32.dll";

        private static readonly object _locker = new object();

        private static IntPtr
            _opengl32,
            _gdi32,
            _user32;

        private static bool _areDllsLoaded = false;
        internal static void LoadDLLs() {
            lock (_locker) {
                if (_areDllsLoaded)
                    return;
                _areDllsLoaded = true;
            }

            _opengl32 = LoadLibrary(Opengl32);
            if (_opengl32 == IntPtr.Zero)
                throw new InvalidOperationException(Opengl32);
            _gdi32 = LoadLibrary(Gdi32);
            if (_gdi32 == IntPtr.Zero)
                throw new InvalidOperationException(Gdi32);
            _user32 = LoadLibrary(User32);
            if (_user32 == IntPtr.Zero)
                throw new InvalidOperationException(User32);
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Kernel32, EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);


        [DllImport(Kernel32, EntryPoint = "LoadLibrary", CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LoadLibrary(string lpFileName);


        [DllImport(Opengl32, EntryPoint = "glGetString")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr GetString(StringName name);


        [DllImport(Opengl32, EntryPoint = "glGetError")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern Errors GetError();


        [DllImport(Opengl32, EntryPoint = "glClearColor")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void ClearColor(float red, float green, float blue, float alpha);


        [DllImport(Opengl32, EntryPoint = "glClear")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Clear(BufferMask mask);


        [DllImport(Opengl32, EntryPoint = "glEnable")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Enable(EnableCap target);

        [DllImport(Opengl32, EntryPoint = "glDisable")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Disable(EnableCap target);


        [DllImport(Opengl32, EntryPoint = "glPolygonOffset")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void PolygonOffset(float a, float b);


        [DllImport(Opengl32, EntryPoint = "glLineWidth")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void LineWidth(float width);


        [DllImport(Opengl32, EntryPoint = "glBlendFunc")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void BlendFunc(BlendMode sfactor, BlendMode dfactor);


        [DllImport(Opengl32, EntryPoint = "glDepthMask")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void DepthMask(bool flag);


        [DllImport(Opengl32, EntryPoint = "glPolygonMode")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void PolygonMode(MaterialFace face, PolygonMode mode);


        [DllImport(Opengl32, EntryPoint = "glCullFace")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void CullFace(MaterialFace face);


        [DllImport(Opengl32, EntryPoint = "glFrontFace")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void FrontFace(FrontFaceDirection mode);


        [DllImport(Opengl32, EntryPoint = "glViewport")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Viewport(uint x, uint y, uint width, uint height);
        [DllImport(Opengl32, EntryPoint = "glViewport")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Viewport(int x, int y, int width, int height);


        [DllImport(Opengl32, EntryPoint = "glDrawArrays")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void DrawArrays(PrimitiveType mode, uint first, uint count);


        [DllImport(Opengl32, EntryPoint = "glDrawElements")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void DrawElements(PrimitiveType mode, uint count, DrawElementsType type, IntPtr indices);


        [DllImport(Opengl32, EntryPoint = "wglGetCurrentContext")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr GetCurrentContext();


        [DllImport(Opengl32, EntryPoint = "wglCreateContext")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr CreateContext(IntPtr deviceContext);


        [DllImport(Opengl32, EntryPoint = "wglMakeCurrent")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern bool MakeCurrent(IntPtr deviceContext, IntPtr renderingContext);


        [DllImport(Opengl32, EntryPoint = "wglDeleteContext")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern bool DeleteContext(IntPtr renderingContext);


        [DllImport(Opengl32, EntryPoint = "wglGetProcAddress")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern IntPtr GetGLProcAddress(string name);


        [DllImport(Gdi32, EntryPoint = "ChoosePixelFormat")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern uint ChoosePixelFormat(IntPtr deviceContext, [In]ref PixelFormatDescriptor ppfd);


        [DllImport(Gdi32, EntryPoint = "SetPixelFormat")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetPixelFormat(IntPtr deviceContext, uint pixelFormat, [In]ref PixelFormatDescriptor ppfd);


        [DllImport(Gdi32, EntryPoint = "SwapBuffers")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SwapBuffers(IntPtr deviceContext);


        [DllImport(User32, EntryPoint = "GetDC")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern IntPtr GetDeviceContext(IntPtr hWnd);


        [DllImport(User32, EntryPoint = "RelaseDC")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RelaseDC(IntPtr hWnd, IntPtr deviceContext);


        [DllImport(User32, EntryPoint = "SetCapture")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern IntPtr SetCapture(IntPtr hWnd);


        [DllImport(User32, EntryPoint = "ReleaseCapture")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern bool ReleaseCapture();

        [DllImport(User32, EntryPoint = "GetDesktopWindow")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr GetDesktopWindow();


        [DllImport(Opengl32, EntryPoint = "glDrawBuffer")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void DrawBuffer(DrawBufferMode drawBufferMode);


        [DllImport(Opengl32, EntryPoint = "glDrawBuffer")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void DrawBuffer(FramebufferAttachment frameBufferAttachment);


        [DllImport(Opengl32, EntryPoint = "glReadBuffer")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void ReadBuffer(DrawBufferMode drawBufferMode);


        [DllImport(Opengl32, EntryPoint = "glReadBuffer")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void ReadBuffer(FramebufferAttachment frameBufferAttachment);


        [DllImport(Opengl32, EntryPoint = "glGenTextures")]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void CreateTextures(int n, uint* textures);


        [DllImport(Opengl32, EntryPoint = "glDeleteTextures")]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void DeleteTextures(int n, uint* textures);


        [DllImport(Opengl32, EntryPoint = "glBindTexture")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void BindTexture(TextureTarget target, uint texture);


        [DllImport(Opengl32, EntryPoint = "glTexImage2D")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels);


        [DllImport(Opengl32, EntryPoint = "glTexParameterf")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void TexParameter(TextureTarget target, TextureParameters parameter, float value);


        [DllImport(Opengl32, EntryPoint = "glTexParameteri")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void TexParameter(TextureTarget target, TextureParameters parameter, int value);


        [DllImport(Opengl32, EntryPoint = "glGetIntegerv")]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void GetIntegerv(uint pname, int* param);


        [DllImport(Opengl32, EntryPoint = "glGetFloatv")]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void GetFloatv(GetName pname, float* v);


        [DllImport(Opengl32, EntryPoint = "glPointSize")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void PointSize(float size);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(Opengl32, EntryPoint = "glReadPixels", CallingConvention = CallingConvention.Winapi)]
        internal static extern void ReadPixels(Int32 x, Int32 y, Int32 width, Int32 height, PixelFormat format, PixelType type, IntPtr pixels);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(Opengl32, EntryPoint = "glGetTexImage")]
        internal static extern void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels);
    }
}
