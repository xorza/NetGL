using NetGL.Core.Infrastructure;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NetGL.Core {
    public unsafe partial class GL {
        private static bool _areExtensionsLoaded = false;

        private static T GetMethodDelegate<T>(string procName = null) {
            if (typeof(Delegate).IsAssignableFrom(typeof(T)) == false)
                throw new InvalidOperationException("T should be delegate type!");

            if (procName == null)
                procName = typeof(T).Name;

            try {
                var proc = NativeMethods.GetGLProcAddress(procName);
                if (proc == IntPtr.Zero) {
                    Log.Warning("gl extension method not found: " + procName);
                    return default(T);
                }

                var del = Marshal.GetDelegateForFunctionPointer(proc, typeof(T));
                Assert.NotNull(del);
                return (T)(object)del;
            }
            catch (Exception ex) {
                throw new GLException(procName, ex);
            }
        }
        private static void LoadExtensions() {
            lock (_locker) {
                if (_areExtensionsLoaded == true)
                    return;
                _areExtensionsLoaded = true;

            }

            _glDebugMessageCallback = GetMethodDelegate<glDebugMessageCallback>("glDebugMessageCallback");

            _glGetActiveUniformsiv = GetMethodDelegate<glGetActiveUniformsiv>("glGetActiveUniformsiv");
            _glGetActiveUniformBlockiv = GetMethodDelegate<glGetActiveUniformBlockiv>("glGetActiveUniformBlockiv");
            _glGetActiveUniform = GetMethodDelegate<glGetActiveUniform>("glGetActiveUniform");

            _glGenerateMipmap = GetMethodDelegate<glGenerateMipmap>("glGenerateMipmap");
            _glGetUniformBlockIndex = GetMethodDelegate<glGetUniformBlockIndex>("glGetUniformBlockIndex");
            _glBindBufferBase = GetMethodDelegate<glBindBufferBase>("glBindBufferBase");
            _glBindBufferRange = GetMethodDelegate<glBindBufferRange>("glBindBufferRange");
            _glUniformBlockBinding = GetMethodDelegate<glUniformBlockBinding>("glUniformBlockBinding");
            _glGetShaderInfoLog = GetMethodDelegate<glGetShaderInfoLog>("glGetShaderInfoLog");
            _glGetProgramInfoLog = GetMethodDelegate<glGetProgramInfoLog>("glGetProgramInfoLog");

            _glUniform3f = GetMethodDelegate<glUniform3f>("glUniform3f");
            _glUniform2f = GetMethodDelegate<glUniform2f>("glUniform2f");
            _glUniform4f = GetMethodDelegate<glUniform4f>("glUniform4f");
            _glUniform1f = GetMethodDelegate<glUniform1f>("glUniform1f");
            _glUniform1i = GetMethodDelegate<glUniform1i>("glUniform1i");
            _glUniformMatrix4fv = GetMethodDelegate<glUniformMatrix4fv>("glUniformMatrix4fv");
            _glUniformMatrix3fv = GetMethodDelegate<glUniformMatrix3fv>("glUniformMatrix3fv");

            _glGetUniformiv = GetMethodDelegate<glGetUniformiv>("glGetUniformiv");
            _glGetUniformuiv = GetMethodDelegate<glGetUniformuiv>();
            _glGetUniformfv = GetMethodDelegate<glGetUniformfv>("glGetUniformfv");

            _glProgramUniform1f = GetMethodDelegate<glProgramUniform1f>("glProgramUniform1f");
            _glProgramUniform2f = GetMethodDelegate<glProgramUniform2f>("glProgramUniform2f");
            _glProgramUniform3f = GetMethodDelegate<glProgramUniform3f>("glProgramUniform3f");
            _glProgramUniform4f = GetMethodDelegate<glProgramUniform4f>("glProgramUniform4f");
            _glProgramUniform1i = GetMethodDelegate<glProgramUniform1i>("glProgramUniform1i");
            _glProgramUniform1ui = GetMethodDelegate<glProgramUniform1ui>();
            _glProgramUniformMatrix3fv = GetMethodDelegate<glProgramUniformMatrix3fv>("glProgramUniformMatrix3fv");
            _glProgramUniformMatrix4fv = GetMethodDelegate<glProgramUniformMatrix4fv>("glProgramUniformMatrix4fv");

            _glActiveTexture = GetMethodDelegate<glActiveTexture>("glActiveTexture");
            _glVertexAttrib2f = GetMethodDelegate<glVertexAttrib2f>("glVertexAttrib2f");
            _glVertexAttrib3f = GetMethodDelegate<glVertexAttrib3f>("glVertexAttrib3f");
            _glVertexAttrib4f = GetMethodDelegate<glVertexAttrib4f>("glVertexAttrib4f");
            _createShader = GetMethodDelegate<glCreateShader>("glCreateShader");
            _shaderSource = GetMethodDelegate<glShaderSource>("glShaderSource");
            _compileShader = GetMethodDelegate<glCompileShader>("glCompileShader");
            _getShaderiv = GetMethodDelegate<glGetShaderiv>("glGetShaderiv");
            _createProgram = GetMethodDelegate<glCreateProgram>("glCreateProgram");
            _attachShader = GetMethodDelegate<glAttachShader>("glAttachShader");
            _linkProgram = GetMethodDelegate<glLinkProgram>("glLinkProgram");
            _bindAttribLocation = GetMethodDelegate<glBindAttribLocation>("glBindAttribLocation");
            _useProgram = GetMethodDelegate<glUseProgram>("glUseProgram");
            _getProgramiv = GetMethodDelegate<glGetProgramiv>("glGetProgramiv");
            _wglSwapIntervalEXT = GetMethodDelegate<wglSwapIntervalEXT>("wglSwapIntervalEXT");
            _glGenBuffers = GetMethodDelegate<glCreateBuffers>("glGenBuffers");
            _glCreateBuffers = GetMethodDelegate<glCreateBuffers>("glCreateBuffers");

            _glBindBuffer = GetMethodDelegate<glBindBuffer>("glBindBuffer");
            _glBufferData = GetMethodDelegate<glBufferData>("glBufferData");
            _glBufferSubData = GetMethodDelegate<glBufferSubData>("glBufferSubData");
            _glNamedBufferSubData = GetMethodDelegate<glNamedBufferSubData>("glNamedBufferSubData");
            _glGetBufferSubData = GetMethodDelegate<glGetBufferSubData>("glGetBufferSubData");
            _glVertexAttribPointer = GetMethodDelegate<glVertexAttribPointer>("glVertexAttribPointer");
            _glEnableVertexAttribArray = GetMethodDelegate<glEnableVertexAttribArray>("glEnableVertexAttribArray");
            _glDisableVertexAttribArray = GetMethodDelegate<glDisableVertexAttribArray>("glDisableVertexAttribArray");
            _glGenVertexArrays = GetMethodDelegate<glCreateVertexArrays>("glGenVertexArrays");
            _glCreateVertexArrays = GetMethodDelegate<glCreateVertexArrays>("glCreateVertexArrays");
            _glBindVertexArray = GetMethodDelegate<glBindVertexArray>("glBindVertexArray");
            _glGetUniformLocation = GetMethodDelegate<glGetUniformLocation>("glGetUniformLocation");
            _glDeleteProgram = GetMethodDelegate<glDeleteProgram>("glDeleteProgram");
            _glDeleteShader = GetMethodDelegate<glDeleteShader>("glDeleteShader");
            _glDetachShader = GetMethodDelegate<glDetachShader>("glDetachShader");
            _glDeleteBuffers = GetMethodDelegate<glDeleteBuffers>("glDeleteBuffers");
            _glDeleteVertexArrays = GetMethodDelegate<glDeleteVertexArrays>("glDeleteVertexArrays");

            _glDeleteFramebuffers = GetMethodDelegate<glDeleteFramebuffers>("glDeleteFramebuffers");
            _glGenFramebuffers = GetMethodDelegate<glGenFramebuffers>("glGenFramebuffers");
            _glBindFramebuffer = GetMethodDelegate<glBindFramebuffer>("glBindFramebuffer");
            _glFramebufferTexture2D = GetMethodDelegate<glFramebufferTexture2D>("glFramebufferTexture2D");
            _glFramebufferRenderbuffer = GetMethodDelegate<glFramebufferRenderbuffer>("glFramebufferRenderbuffer");
            _glCheckFramebufferStatus = GetMethodDelegate<glCheckFramebufferStatus>("glCheckFramebufferStatus");
            _glDrawBuffers = GetMethodDelegate<glDrawBuffers>("glDrawBuffers");
            _glBlitFramebuffer = GetMethodDelegate<glBlitFramebuffer>("glBlitFramebuffer");

            _glGenRenderbuffers = GetMethodDelegate<glCreateRenderbuffers>("glGenRenderbuffers");
            _glCreateRenderbuffers = GetMethodDelegate<glCreateRenderbuffers>("glCreateRenderbuffers");
            _glBindRenderbuffer = GetMethodDelegate<glBindRenderbuffer>("glBindRenderbuffer");
            _glRenderbufferStorage = GetMethodDelegate<glRenderbufferStorage>("glRenderbufferStorage");
            _glRenderbufferStorageMultisample = GetMethodDelegate<glRenderbufferStorageMultisample>("glRenderbufferStorageMultisample");
            _glDeleteRenderbuffers = GetMethodDelegate<glDeleteRenderbuffers>("glDeleteRenderbuffers");

            _glMapBuffer = GetMethodDelegate<glMapBuffer>("glMapBuffer");
            _glUnmapBuffer = GetMethodDelegate<glUnmapBuffer>("glUnmapBuffer");

            _glTexImage2DMultisample = GetMethodDelegate<glTexImage2DMultisample>("glTexImage2DMultisample");

            _glDispatchCompute = GetMethodDelegate<glDispatchCompute>("glDispatchCompute");
            _glMemoryBarrier = GetMethodDelegate<glMemoryBarrier>("glMemoryBarrier");

            _glBlendEquation = GetMethodDelegate<glBlendEquation>("glBlendEquation");

            _glTextureParameteri = GetMethodDelegate<glTextureParameteri>("glTextureParameteri");
            _glTextureParameterf = GetMethodDelegate<glTextureParameterf>("glTextureParameterf");

            _glTexStorage2D = GetMethodDelegate<glTexStorage2D>("glTexStorage2D");
            _glTextureStorage2D = GetMethodDelegate<glTextureStorage2D>("glTextureStorage2D");

            _glTexSubImage2D = GetMethodDelegate<glTexSubImage2D>("glTexSubImage2D");
            _glTextureSubImage2D = GetMethodDelegate<glTextureSubImage2D>("glTextureSubImage2D");

            _glCreateTextures = GetMethodDelegate<glCreateTextures>("glCreateTextures");

            _glClearBufferuiv = GetMethodDelegate<glClearBufferuiv>("glClearBufferuiv");
            _glClearBufferiv = GetMethodDelegate<glClearBufferiv>("glClearBufferiv");
            _glClearBufferfv = GetMethodDelegate<glClearBufferfv>("glClearBufferfv");

            _glGetProgramStageiv = GetMethodDelegate<glGetProgramStageiv>("glGetProgramStageiv");
            _glGetActiveSubroutineUniformName = GetMethodDelegate<glGetActiveSubroutineUniformName>("glGetActiveSubroutineUniformName");
            _glGetActiveSubroutineUniformiv = GetMethodDelegate<glGetActiveSubroutineUniformiv>("glGetActiveSubroutineUniformiv");
            _glGetActiveSubroutineName = GetMethodDelegate<glGetActiveSubroutineName>("glGetActiveSubroutineName");
            _glGetSubroutineIndex = GetMethodDelegate<glGetSubroutineIndex>("glGetSubroutineIndex");
            _glGetSubroutineUniformLocation = GetMethodDelegate<glGetSubroutineUniformLocation>("glGetSubroutineUniformLocation");
            _glUniformSubroutinesuiv = GetMethodDelegate<glUniformSubroutinesuiv>("glUniformSubroutinesuiv");

            _glGetAttribLocation = GetMethodDelegate<glGetAttribLocation>("glGetAttribLocation");
        }


        [SuppressUnmanagedCodeSecurity]
        private delegate int glGetAttribLocation(uint program,
    string name);
        private static glGetAttribLocation _glGetAttribLocation;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniformSubroutinesuiv(ShaderType shadertype, int count, uint[] indices);
        private static glUniformSubroutinesuiv _glUniformSubroutinesuiv;


        [SuppressUnmanagedCodeSecurity]
        private delegate int glGetSubroutineUniformLocation(uint program, ShaderType shadertype, string name);
        private static glGetSubroutineUniformLocation _glGetSubroutineUniformLocation;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveSubroutineName(uint program, ShaderType shadertype, uint index, int bufsize, [Out]out int length, StringBuilder name);
        private static glGetActiveSubroutineName _glGetActiveSubroutineName;


        [SuppressUnmanagedCodeSecurity]
        private delegate uint glGetSubroutineIndex(uint program, ShaderType shadertype, string name);
        private static glGetSubroutineIndex _glGetSubroutineIndex;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveSubroutineUniformName(uint program, ShaderType shadertype, uint index, int bufsize, [Out]out int length, StringBuilder buffer);
        private static glGetActiveSubroutineUniformName _glGetActiveSubroutineUniformName;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveSubroutineUniformiv(uint program, ShaderType shadertype, uint index, ShaderSubroutine pname, int[] values);
        private static glGetActiveSubroutineUniformiv _glGetActiveSubroutineUniformiv;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetProgramStageiv(uint program, ShaderType shadertype, ShaderSubroutine pname, int[] values);
        private static glGetProgramStageiv _glGetProgramStageiv;


        [SuppressUnmanagedCodeSecurity]
        private unsafe delegate void glClearBufferuiv(ClearBuffer buffer, int drawbuffer, uint* value);
        private static glClearBufferuiv _glClearBufferuiv;
        [SuppressUnmanagedCodeSecurity]
        private unsafe delegate void glClearBufferiv(ClearBuffer buffer, int drawbuffer, int* value);
        private static glClearBufferiv _glClearBufferiv;
        [SuppressUnmanagedCodeSecurity]
        private unsafe delegate void glClearBufferfv(ClearBuffer buffer, int drawbuffer, float* value);
        private static glClearBufferfv _glClearBufferfv;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glCreateTextures(TextureTarget target, int count, uint* textures);
        private static glCreateTextures _glCreateTextures;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels);
        private static glTexSubImage2D _glTexSubImage2D;

        [SuppressUnmanagedCodeSecurity]
        private delegate void glTextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels);
        private static glTextureSubImage2D _glTextureSubImage2D;

        [SuppressUnmanagedCodeSecurity]
        private delegate void glTexStorage2D(TextureTarget target, int levels, PixelInternalFormat internalformat, int width, int height);
        private static glTexStorage2D _glTexStorage2D;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glTextureStorage2D(uint texture, int levels, PixelInternalFormat internalformat, int width, int height);
        private static glTextureStorage2D _glTextureStorage2D;


        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        [SuppressUnmanagedCodeSecurity]
        private delegate void DebugMessageCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        [SuppressUnmanagedCodeSecurity]
        private delegate void glDebugMessageCallback(DebugMessageCallback callback, IntPtr userParam);
        private static glDebugMessageCallback _glDebugMessageCallback;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBlendEquation(BlendEquations blendEquation);
        private static glBlendEquation _glBlendEquation;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDispatchCompute(uint x, uint y, uint z);
        private static glDispatchCompute _glDispatchCompute;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glMemoryBarrier(MemoryBarrier barriers);
        private static glMemoryBarrier _glMemoryBarrier;


        [SuppressUnmanagedCodeSecurity]
        private delegate IntPtr glMapBuffer(BindBufferTarget target, MapBufferAccess access);
        private static glMapBuffer _glMapBuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glUnmapBuffer(BindBufferTarget target);
        private static glUnmapBuffer _glUnmapBuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glCreateRenderbuffers(uint count, uint *buffers);
        private static glCreateRenderbuffers _glGenRenderbuffers;
        private static glCreateRenderbuffers _glCreateRenderbuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindRenderbuffer(RenderbufferBindTarget target, uint buffer);
        private static glBindRenderbuffer _glBindRenderbuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glRenderbufferStorage(RenderbufferBindTarget target, RenderbufferStorage internalformat, int width, int height);
        private static glRenderbufferStorage _glRenderbufferStorage;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glRenderbufferStorageMultisample(RenderbufferBindTarget target, int sampleCount, RenderbufferStorage internalformat, int width, int height);
        private static glRenderbufferStorageMultisample _glRenderbufferStorageMultisample;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteRenderbuffers(int n, uint *buffers);
        private static glDeleteRenderbuffers _glDeleteRenderbuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glFramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint textureHandle, int level);
        private static glFramebufferTexture2D _glFramebufferTexture2D;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glFramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferBindTarget textarget, uint renderBufferHandle);
        private static glFramebufferRenderbuffer _glFramebufferRenderbuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glTexImage2DMultisample(TextureTarget target, int samples, PixelInternalFormat internalformat, int width, int height, bool fixedsamplelocations);
        private static glTexImage2DMultisample _glTexImage2DMultisample;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glTextureParameteri(uint texture, TextureParameters parameter, int value);
        private static glTextureParameteri _glTextureParameteri;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glTextureParameterf(uint texture, TextureParameters parameter, float value);
        private static glTextureParameterf _glTextureParameterf;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDrawBuffers(int count, FramebufferAttachment[] buffers);
        private static glDrawBuffers _glDrawBuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, BufferMask mask, BlitFramebufferFilter filter);
        private static glBlitFramebuffer _glBlitFramebuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGenFramebuffers(uint count, uint[] buffers);
        private static glGenFramebuffers _glGenFramebuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate FramebufferStatus glCheckFramebufferStatus(FramebufferTarget target);
        private static glCheckFramebufferStatus _glCheckFramebufferStatus;


        private delegate void glBindFramebuffer(FramebufferTarget target, uint buffer);
        private static glBindFramebuffer _glBindFramebuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteFramebuffers(int n, uint* buffers);
        private static glDeleteFramebuffers _glDeleteFramebuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate bool wglSwapIntervalEXT(int interval);
        private static wglSwapIntervalEXT _wglSwapIntervalEXT = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glCreateBuffers(uint count, uint* buffers);
        private static glCreateBuffers _glGenBuffers;
        private static glCreateBuffers _glCreateBuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindBuffer(BindBufferTarget target, uint handle);
        private static glBindBuffer _glBindBuffer;


        [SuppressUnmanagedCodeSecurity]
        private delegate uint glCreateShader(ShaderType type);
        private static glCreateShader _createShader = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glShaderSource(uint shader, uint count, string[] codes, uint[] lengths);
        private static glShaderSource _shaderSource = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glCompileShader(uint shader);
        private static glCompileShader _compileShader = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetShaderiv(uint shader, ShaderInfo status, [Out]out int result);
        private static glGetShaderiv _getShaderiv = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate uint glCreateProgram();
        private static glCreateProgram _createProgram = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glAttachShader(uint program, uint shader);
        private static glAttachShader _attachShader = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glLinkProgram(uint program);
        private static glLinkProgram _linkProgram = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindAttribLocation(uint program, int index, string name);
        private static glBindAttribLocation _bindAttribLocation = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glUseProgram(uint program);
        private static glUseProgram _useProgram = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetProgramiv(uint program, ProgramInfo status, int[] result);
        private static glGetProgramiv _getProgramiv = null;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBufferData(BindBufferTarget target, int size, IntPtr data, BufferUsage usage);
        private static glBufferData _glBufferData;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBufferSubData(BindBufferTarget target, int offset, int size, IntPtr data);
        private static glBufferSubData _glBufferSubData;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glNamedBufferSubData(uint buffer, int offset, int size, IntPtr data);
        private static glNamedBufferSubData _glNamedBufferSubData;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetBufferSubData(BindBufferTarget target, int offset, int size, IntPtr data);
        private static glGetBufferSubData _glGetBufferSubData;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glVertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalize, int stride, int offset);
        private static glVertexAttribPointer _glVertexAttribPointer;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glEnableVertexAttribArray(uint index);
        private static glEnableVertexAttribArray _glEnableVertexAttribArray;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDisableVertexAttribArray(uint index);
        private static glDisableVertexAttribArray _glDisableVertexAttribArray;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glCreateVertexArrays(int n, uint *arrays);
        private static glCreateVertexArrays _glGenVertexArrays;
        private static glCreateVertexArrays _glCreateVertexArrays;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindVertexArray(uint array);
        private static glBindVertexArray _glBindVertexArray;


        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.I4)]
        private delegate int glGetUniformLocation(uint program, string name);
        private static glGetUniformLocation _glGetUniformLocation;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteProgram(uint program);
        private static glDeleteProgram _glDeleteProgram;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteShader(uint shader);
        private static glDeleteShader _glDeleteShader;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDetachShader(uint program, uint shader);
        private static glDetachShader _glDetachShader;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteBuffers(int n, uint *buffers);
        private static glDeleteBuffers _glDeleteBuffers;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glDeleteVertexArrays(int n, uint *arrays);
        private static glDeleteVertexArrays _glDeleteVertexArrays;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glVertexAttrib3f(uint attribIndex, float v1, float v2, float v3);
        private static glVertexAttrib3f _glVertexAttrib3f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glVertexAttrib4f(uint attribIndex, float v1, float v2, float v3, float v4);
        private static glVertexAttrib4f _glVertexAttrib4f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glVertexAttrib2f(uint attribIndex, float v1, float v2);
        private static glVertexAttrib2f _glVertexAttrib2f;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glActiveTexture(TextureUnit textureUnit);
        private static glActiveTexture _glActiveTexture;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniform1f(int location, float v0);
        private static glUniform1f _glUniform1f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniform1i(int location, int v0);
        private static glUniform1i _glUniform1i;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniform2f(int location, float f1, float f2);
        private static glUniform2f _glUniform2f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniform3f(int location, float f1, float f2, float f3);
        private static glUniform3f _glUniform3f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniform4f(int location, float f1, float f2, float f3, float f4);
        private static glUniform4f _glUniform4f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniformMatrix4fv(int location, int count, bool transpose, float[] value);
        private static glUniformMatrix4fv _glUniformMatrix4fv;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniformMatrix3fv(int location, int count, bool transpose, float[] value);
        private static glUniformMatrix3fv _glUniformMatrix3fv;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetUniformfv(uint program, int location, float* @params);
        private static glGetUniformfv _glGetUniformfv;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetUniformiv(uint program, int location, int* @params);
        private static glGetUniformiv _glGetUniformiv;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetUniformuiv(uint program, int location, uint* @params);
        private static glGetUniformuiv _glGetUniformuiv;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform1f(uint program, int location, float v0);
        private static glProgramUniform1f _glProgramUniform1f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform2f(uint program, int location, float f1, float f2);
        private static glProgramUniform2f _glProgramUniform2f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform3f(uint program, int location, float f1, float f2, float f3);
        private static glProgramUniform3f _glProgramUniform3f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform4f(uint program, int location, float f1, float f2, float f3, float f4);
        private static glProgramUniform4f _glProgramUniform4f;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform1i(uint program, int location, int v0);
        private static glProgramUniform1i _glProgramUniform1i;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniform1ui(uint program, int location, uint v0);
        private static glProgramUniform1ui _glProgramUniform1ui;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniformMatrix3fv(uint program, int location, int count, bool transpose, float[] value);
        private static glProgramUniformMatrix3fv _glProgramUniformMatrix3fv;
        [SuppressUnmanagedCodeSecurity]
        private delegate void glProgramUniformMatrix4fv(uint program, int location, int count, bool transpose, float[] value);
        private static glProgramUniformMatrix4fv _glProgramUniformMatrix4fv;




        [SuppressUnmanagedCodeSecurity]
        private delegate void glUniformBlockBinding(uint shaderProgram, uint uniformBlockIndx, uint uniformBlockBinding);
        private static glUniformBlockBinding _glUniformBlockBinding;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindBufferRange(BindBufferTarget target, uint index, uint buffer, uint offset, uint size);
        private static glBindBufferRange _glBindBufferRange;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glBindBufferBase(BindBufferTarget target, uint index, uint buffer);
        private static glBindBufferBase _glBindBufferBase;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetShaderInfoLog(uint shader, int maxLength, [Out]out int length, [Out] StringBuilder infoLog);
        private static glGetShaderInfoLog _glGetShaderInfoLog;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetProgramInfoLog(uint program, int maxLength, [Out]out int length, StringBuilder infoLog);
        private static glGetProgramInfoLog _glGetProgramInfoLog;


        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.I4)]
        private delegate int glGetUniformBlockIndex(uint program, string uniformBlockName);
        private static glGetUniformBlockIndex _glGetUniformBlockIndex;


        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.I4)]
        private delegate int glGenerateMipmap(GenerateMipmapTarget target);
        private static glGenerateMipmap _glGenerateMipmap;

        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveUniform(uint program, Int32 index, Int32 bufSize, [Out] out Int32 length, [Out] out Int32 size, [Out] out ActiveUniformType type, [Out] StringBuilder name);
        private static glGetActiveUniform _glGetActiveUniform;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveUniformBlockiv(uint program, int uniformBlockIndex, ActiveUniformBlockParameter pname, int[] @params);
        private static glGetActiveUniformBlockiv _glGetActiveUniformBlockiv;


        [SuppressUnmanagedCodeSecurity]
        private delegate void glGetActiveUniformsiv(uint program, int uniformCount, int *uniformIndices, ActiveUniformParameter pname, int *@params);
        private static glGetActiveUniformsiv _glGetActiveUniformsiv;
    }
}