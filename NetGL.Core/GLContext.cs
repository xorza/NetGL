using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace NetGL.Core {
    public partial class GL : IDisposable {
        private static readonly object _locker = new object();
        private static readonly ThreadLocal<GL> _currentContext = new ThreadLocal<GL>();

        public static GL GetCurrent(bool throwIfNull) {
            var context = _currentContext.Value;
            if (context == null && throwIfNull)
                throw new ContextMissingException();
            return context;
        }

        private readonly IntPtr _renderingContextPtr;
        private readonly IntPtr _deviceContextPtr;

        private readonly ResourceTracker _resourceTracker = new ResourceTracker();

        #region currenly bound
        private uint _program = 0;
        private uint _vertexArray = 0;
        private readonly uint[] _textures;
        private TextureUnit _activeTextureUnit = TextureUnit.Texture0;
        private uint _drawFramebuffer, _readFramebuffer;
        #endregion

        public bool IsDisposed { get; private set; }

        public GLVersion Version { get; private set; }
        public int MaxSamples { get; private set; }
        public int MaxTextureSize { get; private set; }
        public int MaxTextureUnits { get; private set; }
        public int FramebufferMaxColorAttachments { get; private set; }
        public bool DirectStateAccess { get; private set; }

        public string Vendor { get; private set; }
        public string Renderer { get; private set; }
        public string ShadingLanguageVersion { get; private set; }
        public string VersionText { get; private set; }

        public IReadOnlyCollection<string> Extensions { get; private set; }

        public ResourceTracker ResourceTracker { get { return _resourceTracker; } }

        public GL(IntPtr windowHandle) {
            if (GetCurrent(false) != null)
                throw new GLException("GLContext already exists in current thread. Try launching in different thread");

            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("windowHandle");

            lock (_locker) {
                NativeMethods.LoadDLLs();

                _deviceContextPtr = NativeMethods.GetDeviceContext(windowHandle);
                Assert.False(_deviceContextPtr == IntPtr.Zero);

                var pixelFormatDescriptor = PixelFormatDescriptor.CreateDefault();
                var pixelformat = NativeMethods.ChoosePixelFormat(_deviceContextPtr, ref pixelFormatDescriptor);
                Assert.False(pixelformat == 0);

                var success = NativeMethods.SetPixelFormat(_deviceContextPtr, pixelformat, ref pixelFormatDescriptor);
                Assert.True(success);

                _renderingContextPtr = NativeMethods.CreateContext(_deviceContextPtr);

                if (MakeCurrent() == false) {
                    MakeCurrentEmpty();
                    NativeMethods.DeleteContext(_renderingContextPtr);

                    var error = "Error making created context current.";
                    Log.Error(error);
                    throw new GLException(error);
                }

                LoadExtensions();
                LoadConstants();

                Log.Info(string.Format("Initialized GL context version: {0}.", Version));
                if (Version < new GLVersion(4, 0)) {
                    Dispose();

                    var error = string.Format("Only OpenGL 4.0 and higher versions supported. You have {0}", Version);
                    Log.Error(error);
                    throw new GLException(error);
                }

                SetupDebugCallback();

                _currentContext.Value = this;

                _textures = new uint[MaxTextureUnits];
                ActiveTexture(TextureUnit.Texture0);

                CheckForError();
            }
        }

        private void LoadConstants() {
            Version = new GLVersion(GetInteger(GetName.MajorVersion), GetInteger(GetName.MinorVersion));
            MaxSamples = Math.Min(16, GetInteger(GetName.MaxSamples));
            MaxTextureSize = GetInteger(GetName.MaxTextureSize);
            MaxTextureUnits = GetInteger(GetName.MaxTextureImageUnits);
            FramebufferMaxColorAttachments = GetInteger(GetName.MaxColorAttachments);
            DirectStateAccess = (Version.Major >= 4 && Version.Minor >= 5);

            Vendor = GetString(StringName.Vendor);
            Renderer = GetString(StringName.Renderer);
            ShadingLanguageVersion = GetString(StringName.ShadingLanguageVersion);
            VersionText = GetString(StringName.Version);

            this.Extensions = GetString(StringName.Extenstions)
                .Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .AsReadOnly();
        }

        public void Dispose() {
            if (IsDisposed)
                return;
            IsDisposed = true;
            GC.SuppressFinalize(this);

            _resourceTracker.Dispose();

            if (IsCurrent()) {
                CheckForError();
                MakeCurrentEmpty();
            }

            var result = NativeMethods.DeleteContext(this._renderingContextPtr);
            Assert.True(result);
        }
        ~GL() {
            NativeMethods.DeleteContext(this._renderingContextPtr);
        }

        public bool MakeCurrent() {
            if (IsCurrent())
                return true;

            var result = NativeMethods.MakeCurrent(_deviceContextPtr, _renderingContextPtr);
            if (result) {
                _currentContext.Value = this;
                return true;
            }
            else {
                Dispose();
                return false;
            }
        }
        public bool IsCurrent() {
            var currentRenderContextPtr1 = NativeMethods.GetCurrentContext();
            var currentContext = GetCurrent(false);
            var currentRenderContextPtr2 = currentContext != null ? currentContext._renderingContextPtr : IntPtr.Zero;

            if (currentRenderContextPtr2 != currentRenderContextPtr1)
                throw new GLException("Current context error");

            return ReferenceEquals(this, currentContext);
        }
        public void SwapBuffers() {
            var result = NativeMethods.SwapBuffers(_deviceContextPtr);
            if (result == false) {
                Dispose();
                throw new GLException("Disposed context");
            }
            CheckForError();
        }
        public void SwapInterval(int interval) {
            var result = _wglSwapIntervalEXT(interval);
            CheckForError();
            if (result == false)
                Log.Warning("failed to set swapinterval to " + interval.ToString());
        }

        #region renderbuffer
        internal unsafe uint CreateRenderbuffer() {
            uint param = 0;
            if (_glCreateRenderbuffers != null)
                _glCreateRenderbuffers(1, &param);
            else
                _glGenRenderbuffers(1, &param);

            if (param == 0)
                throw new GLException();

            return param;
        }
        internal unsafe void DeleteRenderbuffer(uint handle) {
            _glDeleteRenderbuffers(1, &handle);
            CheckForError();
        }
        internal void BindRenderbuffer(uint handle) {
            _glBindRenderbuffer(RenderbufferBindTarget.Renderbuffer, handle);
            CheckForError();
        }

        public void RenderbufferStorage(RenderbufferStorage internalformat, int width, int height) {
            _glRenderbufferStorage(RenderbufferBindTarget.Renderbuffer, internalformat, width, height);
            CheckForError();
        }
        public void RenderbufferStorageMultisample(RenderbufferStorage internalformat, int sampleCount, int width, int height) {
            _glRenderbufferStorageMultisample(RenderbufferBindTarget.Renderbuffer, sampleCount, internalformat, width, height);
            CheckForError();
        }
        #endregion
        #region texture
        internal void ActiveTexture(TextureUnit unit) {
            if (_activeTextureUnit == unit)
                return;

            _activeTextureUnit = unit;
            _glActiveTexture(unit);
            CheckForError();
        }

        internal unsafe uint CreateTexture(TextureTarget target) {
            uint param = 0;
            if (_glCreateTextures != null)
                _glCreateTextures(target, 1, &param);
            else
                NativeMethods.CreateTextures(1, &param);

            if (param == 0)
                throw new GLException();

            return param;
        }
        internal void BindTexture(TextureTarget target, uint handle) {
            var slot = _activeTextureUnit - TextureUnit.Texture0;
            if (_textures[slot] == handle)
                return;

            _textures[slot] = handle;
            NativeMethods.BindTexture(target, handle);
            CheckForError();
        }
        internal unsafe void DeleteTexture(uint handle) {
            for (int i = 0; i < _textures.Length; i++)
                if (_textures[i] == handle)
                    _textures[i] = 0;

            NativeMethods.DeleteTextures(1, &handle);
            CheckForError();
        }

        internal void TexImage2DMultisample(TextureTarget target, int samples, PixelInternalFormat internalformat, int width, int height, bool fixedsamplelocations) {
            _glTexImage2DMultisample(target, samples, internalformat, width, height, fixedsamplelocations);
            CheckForError();
        }
        internal int GenerateMipmap(GenerateMipmapTarget target) {
            var result = _glGenerateMipmap(target);
            CheckForError();
            return result;
        }

        internal void TexParameter(TextureTarget target, TextureParameters parameter, float value) {
            NativeMethods.TexParameter(target, parameter, value);
            CheckForError();
        }
        internal void TexParameter(TextureTarget target, TextureParameters parameter, int value) {
            NativeMethods.TexParameter(target, parameter, value);
            CheckForError();
        }
        internal void TexParameter(TextureTarget target, TextureParameters parameter, TextureFilter value) {
            NativeMethods.TexParameter(target, parameter, (int)value);
            CheckForError();
        }
        internal void TexParameter(TextureTarget target, TextureParameters parameter, TextureWrapMode value) {
            NativeMethods.TexParameter(target, parameter, (int)value);
            CheckForError();
        }
        internal void TexParameter(TextureTarget target, TextureParameters parameter, TextureCompareMode value) {
            NativeMethods.TexParameter(target, parameter, (int)value);
            CheckForError();
        }
        internal void TexParameter(TextureTarget target, TextureParameters parameter, DepthFunc value) {
            NativeMethods.TexParameter(target, parameter, (int)value);
            CheckForError();
        }

        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, float value) {
            _glTextureParameterf(texture, parameter, value);
            CheckForError();
        }
        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, int value) {
            _glTextureParameteri(texture, parameter, value);
            CheckForError();
        }
        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, TextureFilter value) {
            _glTextureParameteri(texture, parameter, (int)value);
            CheckForError();
        }
        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, TextureWrapMode value) {
            _glTextureParameteri(texture, parameter, (int)value);
            CheckForError();
        }
        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, TextureCompareMode value) {
            _glTextureParameteri(texture, parameter, (int)value);
            CheckForError();
        }
        [DirectStateAccess]
        internal void TextureParameter(uint texture, TextureParameters parameter, DepthFunc value) {
            _glTextureParameteri(texture, parameter, (int)value);
            CheckForError();
        }


        internal void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels) {
            NativeMethods.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
            CheckForError();
        }
        internal void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels) {
            NativeMethods.GetTexImage(target, level, format, type, pixels);
            CheckForError();
        }

        internal void TexStorage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height) {
            _glTexStorage2D(target, level, internalformat, width, height);
            CheckForError();
        }
        [DirectStateAccess]
        [Obsolete("chashes")]
        internal void TextureStorage2D(uint texture, int level, PixelInternalFormat internalformat, int width, int height) {
            _glTextureStorage2D(texture, level, internalformat, width, height);
            CheckForError();
        }

        internal void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels) {
            _glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
            CheckForError();
        }
        internal void TextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels) {
            _glTextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, type, pixels);
            CheckForError();
        }
        #endregion
        #region framebuffer
        internal uint CreateFramebuffer() {
            var @params = new uint[1];
            _glGenFramebuffers(1, @params);
            if (@params[0] == 0)
                throw new GLException();

            return @params[0];
        }
        internal void BindFramebuffer(FramebufferTarget target, uint handle) {
            switch (target) {
                case FramebufferTarget.ReadFramebuffer:
                    if (_readFramebuffer == handle)
                        return;
                    _readFramebuffer = handle;
                    break;
                case FramebufferTarget.DrawFramebuffer:
                    if (_drawFramebuffer == handle)
                        return;
                    _drawFramebuffer = handle;
                    break;
                case FramebufferTarget.Framebuffer:
                    if (_drawFramebuffer == handle && _readFramebuffer == handle)
                        return;
                    _readFramebuffer = handle;
                    _drawFramebuffer = handle;
                    break;
                default:
                    throw new NotSupportedException(target.ToString());
            }

            _glBindFramebuffer(target, handle);
            CheckForError();
        }
        internal unsafe void DeleteFramebuffer(uint handle) {
            if (handle == _readFramebuffer)
                _readFramebuffer = 0;
            if (handle == _drawFramebuffer)
                _drawFramebuffer = 0;

            _glDeleteFramebuffers(1, &handle);
            CheckForError();
        }

        public void BindDefaultFramebuffer(FramebufferTarget target) {
            BindFramebuffer(target, 0);
        }
        public void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, BufferMask mask, BlitFramebufferFilter filter) {
            _glBlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
            CheckForError();
        }
        public FramebufferStatus CheckFramebufferStatus(FramebufferTarget target) {
            return _glCheckFramebufferStatus(target);
        }

        internal void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, uint renderBuffer) {
            _glFramebufferRenderbuffer(target, attachment, RenderbufferBindTarget.Renderbuffer, renderBuffer);
            CheckForError();
        }
        internal void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture) {
            _glFramebufferTexture2D(target, attachment, textarget, texture, 0);
            CheckForError();
        }
        #endregion
        #region uniforms
        [DirectStateAccess]
        internal void Uniform(uint program, int location, int v0) {
            _glProgramUniform1i(program, location, v0);
            CheckForError();
        }
        [DirectStateAccess]
        internal void Uniform(uint program, int location, uint v0) {
            _glProgramUniform1ui(program, location, v0);
            CheckForError();
        }
        [DirectStateAccess]
        internal void Uniform(uint program, int location, float v0) {
            _glProgramUniform1f(program, location, v0);
            CheckForError();
        }
        [DirectStateAccess]
        internal void Uniform(uint program, int location, Vector2 v) {
            _glProgramUniform2f(program, location, v.X, v.Y);
            CheckForError();
        }
        [DirectStateAccess]
        internal void Uniform(uint program, int location, Vector3 v) {
            _glProgramUniform3f(program, location, v.X, v.Y, v.Z);
            CheckForError();
        }
        [DirectStateAccess]
        internal void Uniform(uint program, int location, Vector4 v) {
            _glProgramUniform4f(program, location, v.X, v.Y, v.Z, v.W);
            CheckForError();
        }
        [DirectStateAccess]
        internal void UniformMatrix(uint program, int location, int count, bool transpose, float[] value) {
            switch (value.Length) {
                case 16:
                    _glProgramUniformMatrix4fv(program, location, count, transpose, value);
                    CheckForError();
                    return;
                case 9:
                    _glProgramUniformMatrix3fv(program, location, count, transpose, value);
                    CheckForError();
                    return;
                default:
                    throw new NotSupportedException(value.Length.ToString());
            }
        }

        internal unsafe float GetUniformSingle(uint program, int location) {
            float param;
            _glGetUniformfv(program, location, &param);
            CheckForError();
            return param;
        }
        internal unsafe Vector2 GetUniformVector2(uint program, int location) {
            float* param = stackalloc float[2];
            _glGetUniformfv(program, location, param);
            CheckForError();
            return new Vector2(param[0], param[1]);
        }
        internal unsafe Vector3 GetUniformVector3(uint program, int location) {
            float* param = stackalloc float[3];
            _glGetUniformfv(program, location, param);
            CheckForError();
            return new Vector3(param[0], param[1], param[2]);
        }
        internal unsafe Vector4 GetUniformVector4(uint program, int location) {
            float* param = stackalloc float[4];
            _glGetUniformfv(program, location, param);
            CheckForError();
            return new Vector4(param[0], param[1], param[2], param[3]);
        }
        internal unsafe int GetUniformInt32(uint program, int location) {
            int param;
            _glGetUniformiv(program, location, &param);
            CheckForError();
            return param;
        }
        internal unsafe uint GetUniformUInt32(uint program, int location) {
            uint param;
            _glGetUniformuiv(program, location, &param);
            CheckForError();
            return param;
        }

        internal int GetUniformLocation(uint program, string name) {
            var result = _glGetUniformLocation(program, name);
            CheckForError();
            return result;
        }
        internal void UniformBlockBinding(uint shaderProgram, uint uniformBlockIndex, uint uniformBlockBinding) {
            _glUniformBlockBinding(shaderProgram, uniformBlockIndex, uniformBlockBinding);
            CheckForError();
        }
        internal void GetActiveUniform(uint program, Int32 index, out Int32 size, out ActiveUniformType type, out string name) {
            var length = GetProgramiv(program, ProgramInfo.ActiveUniformMaxLength);
            var stringBuilder = new StringBuilder(length);

            _glGetActiveUniform(program, index, stringBuilder.Capacity, out length, out size, out type, stringBuilder);
            CheckForError();
            Assert.True(stringBuilder.Length == length);
            if (length == 0)
                name = string.Empty;
            else
                name = stringBuilder.ToString(0, length);
        }
        internal int GetUniformBlockIndex(uint program, string uniformBlockName) {
            var result = _glGetUniformBlockIndex(program, uniformBlockName);
            CheckForError();
            return result;
        }
        internal void GetActiveUniformBlockiv(uint program, int uniformBlockIndex, ActiveUniformBlockParameter pname, int[] @params) {
            _glGetActiveUniformBlockiv(program, uniformBlockIndex, pname, @params);
            CheckForError();
        }
        internal unsafe int GetActiveUniformsiv(uint program, int uniformIndex, ActiveUniformParameter pname) {
            int result = 0;
            _glGetActiveUniformsiv(program, 1, &uniformIndex, pname, &result);
            CheckForError();
            return result;
        }
        #endregion
        #region shader
        internal uint CreateShader(ShaderType shaderType) {
            var h = _createShader(shaderType);
            if (h == 0)
                throw new GLException();

            return h;
        }
        internal void DeleteShader(uint shader) {
            _glDeleteShader(shader);
            CheckForError();
        }
        internal void DetachShader(uint program, uint shader) {
            _glDetachShader(program, shader);
            CheckForError();
        }
        internal string GetShaderInfoLog(uint shader) {
            var length = GetShaderiv(shader, ShaderInfo.InfoLogLength);
            if (length == 0)
                return string.Empty;

            var stringBuilder = new StringBuilder(length);
            _glGetShaderInfoLog(shader, stringBuilder.Capacity, out length, stringBuilder);
            CheckForError();
            Assert.True(stringBuilder.Length == length);
            if (length == 0)
                return string.Empty;
            else
                return stringBuilder.ToString(0, length);
        }
        internal void ShaderSource(uint shader, string code) {
            _shaderSource(shader, 1, new string[] { code }, new uint[1] { (uint)code.Length });
            CheckForError();
        }
        internal void CompileShader(uint shader) {
            _compileShader(shader);
            CheckForError();
        }
        internal int GetShaderiv(uint shader, ShaderInfo status) {
            int result;
            _getShaderiv(shader, status, out result);
            CheckForError();
            return result;
        }
        internal void AttachShader(uint program, uint shader) {
            _attachShader(program, shader);
            CheckForError();
        }

        public void DispatchCompute(uint x, uint y, uint z) {
            _glDispatchCompute(x, y, z);
            CheckForError();
        }

        internal uint CreateProgram() {
            var h = _createProgram();
            if (h == 0)
                throw new GLException();

            return h;
        }

        /// <summary>
        /// Binds shader program to current context
        /// </summary>
        /// <param name="program">shader program handle</param>
        /// <returns>false if this program is already bound, otherwise - true</returns>
        internal bool UseProgram(uint program) {
            if (_program == program)
                return false;

            _program = program;
            _useProgram(program);
            CheckForError();
            return true;
        }
        internal void DeleteProgram(uint program) {
            if (_program == program)
                _program = 0;

            _glDeleteProgram(program);
            CheckForError();
        }
        internal void LinkProgram(uint program) {
            _linkProgram(program);
            CheckForError();
        }
        internal int GetProgramiv(uint program, ProgramInfo status) {
            var result = new int[1];
            _getProgramiv(program, status, result);
            CheckForError();
            return result[0];
        }
        internal void GetProgramiv(uint program, ProgramInfo status, int[] values) {
            Assert.NotNull(values);
            Assert.True(values.Length >= 1);

            _getProgramiv(program, status, values);
            CheckForError();
        }
        internal string GetProgramInfoLog(uint program) {
            var length = GetProgramiv(program, ProgramInfo.InfoLogLength);
            if (length == 0)
                return string.Empty;

            var stringBuilder = new StringBuilder(length);
            _glGetProgramInfoLog(program, stringBuilder.Capacity, out length, stringBuilder);
            CheckForError();
            //Assert.True(stringBuilder.Length == length);
            if (length == 0)
                return string.Empty;
            return stringBuilder.ToString();
        }

        internal int GetProgramStage(uint program, ShaderType shadertype, ShaderSubroutine parameter) {
            var values = new int[1];
            _glGetProgramStageiv(program, shadertype, parameter, values);
            CheckForError();
            return values[0];
        }
        internal string GetActiveSubroutineUniformName(uint program, ShaderType shaderType, uint index) {
            var sb = new StringBuilder(256);

            int length;
            _glGetActiveSubroutineUniformName(program, shaderType, index, sb.Capacity, out length, sb);
            CheckForError();

            return sb.ToString(0, length);
        }
        internal int GetActiveSubroutineUniform(uint program, ShaderType shaderType, uint index, ShaderSubroutine parameter) {
            var values = new int[1];
            _glGetActiveSubroutineUniformiv(program, shaderType, index, parameter, values);
            CheckForError();

            return values[0];
        }
        internal void GetActiveSubroutineUniform(uint program, ShaderType shaderType, uint index, ShaderSubroutine parameter, int[] values) {
            Assert.NotNull(values);
            Assert.True(values.Length > 0);
            _glGetActiveSubroutineUniformiv(program, shaderType, index, parameter, values);
            CheckForError();
        }
        internal string GetActiveSubroutineName(uint program, ShaderType shaderType, uint index) {
            var sb = new StringBuilder(256);

            int length;
            _glGetActiveSubroutineName(program, shaderType, index, sb.Capacity, out length, sb);
            CheckForError();

            return sb.ToString(0, length);
        }
        internal uint GetSubroutineIndex(uint program, ShaderType shadertype, string name) {
            return _glGetSubroutineIndex(program, shadertype, name);
        }
        internal int GetSubroutineUniformLocation(uint program, ShaderType shadertype, string name) {
            return _glGetSubroutineUniformLocation(program, shadertype, name);
        }
        internal void UniformSubroutinesuiv(ShaderType shaderType, uint[] indices) {
            _glUniformSubroutinesuiv(shaderType, indices.Length, indices);
        }
        #endregion
        #region bufferobject
        internal unsafe uint CreateBuffer() {
            uint param = 0;

            if (_glCreateTextures != null)
                _glCreateBuffers(1, &param);
            else
                _glGenBuffers(1, &param);

            if (param == 0)
                throw new GLException();

            return param;
        }

        internal void BindBuffer(BindBufferTarget target, uint handle) {
            _glBindBuffer(target, handle);
            CheckForError();
        }
        internal void BindElementArrayBuffer(uint handle) {
            _glBindBuffer(BindBufferTarget.ElementArrayBuffer, handle);
            CheckForError();
        }
        internal void BindShaderStorageBuffer(uint handle) {
            _glBindBuffer(BindBufferTarget.ShaderStorageBuffer, handle);
            CheckForError();
        }
        internal void BindArrayBuffer(uint handle) {
            _glBindBuffer(BindBufferTarget.ArrayBuffer, handle);
            CheckForError();
        }
        internal void BindUniformBuffer(uint handle) {
            _glBindBuffer(BindBufferTarget.UniformBuffer, handle);
            CheckForError();
        }

        internal unsafe void DeleteBuffer(uint buffer) {
            _glDeleteBuffers(1, &buffer);
            CheckForError();
        }

        internal void BindBufferRange(BindBufferTarget target, uint index, uint bufferObject, uint offset, uint size) {
            _glBindBufferRange(target, index, bufferObject, offset, size);
            CheckForError();
        }
        internal void BindBufferBase(BindBufferTarget target, uint index, uint buffer) {
            _glBindBufferBase(target, index, buffer);
            CheckForError();
        }

        internal void BufferSubData(BindBufferTarget target, UnsafeBuffer data) {
            _glBufferSubData(target, 0, data.SizeInBytes, data.Pointer);
            CheckForError();
        }
        [DirectStateAccess]
        internal void BufferSubData(uint buffer, UnsafeBuffer data) {
            _glNamedBufferSubData(buffer, 0, data.SizeInBytes, data.Pointer);
            CheckForError();
        }
        internal void BufferSubData(BindBufferTarget target, int offset, int size, IntPtr data) {
            _glBufferSubData(target, offset, size, data);
            CheckForError();
        }
        [DirectStateAccess]
        internal void BufferSubData(uint buffer, int offset, int size, IntPtr data) {
            _glNamedBufferSubData(buffer, offset, size, data);
            CheckForError();
        }

        internal void GetBufferSubData(BindBufferTarget target, UnsafeBuffer data) {
            _glGetBufferSubData(target, 0, data.SizeInBytes, data.Pointer);
            CheckForError();
        }

        internal void BufferData(BindBufferTarget target, UnsafeBuffer data, BufferUsage usage) {
            _glBufferData(target, data.SizeInBytes, data.Pointer, usage);
            CheckForError();
        }
        internal void BufferData(BindBufferTarget target, int size, BufferUsage usage) {
            _glBufferData(target, size, IntPtr.Zero, usage);
            CheckForError();
        }
        internal void BufferData(BindBufferTarget target, int size, IntPtr ptr, BufferUsage usage) {
            _glBufferData(target, size, IntPtr.Zero, usage);
            CheckForError();
        }

        internal void UnmapBuffer(BindBufferTarget target) {
            _glUnmapBuffer(target);
            CheckForError();
        }
        internal IntPtr MapBuffer(BindBufferTarget target, MapBufferAccess access) {
            var result = _glMapBuffer(target, access);
            CheckForError();
            return result;
        }
        #endregion
        #region vertexarray
        internal unsafe uint GenVertexArray() {
            uint param = 0;

            if (_glCreateVertexArrays != null)
                _glCreateVertexArrays(1, &param);
            else
                _glGenVertexArrays(1, &param);

            if (param == 0)
                throw new GLException();
            return param;
        }
        internal void DisableVertexAttribArray(uint attributeIndex) {
            _glDisableVertexAttribArray(attributeIndex);
            CheckForError();
        }
        internal void EnableVertexAttribArray(uint attributeIndex) {
            _glEnableVertexAttribArray(attributeIndex);
            CheckForError();
        }
        internal void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalize, int stride, int offset) {
            _glVertexAttribPointer(index, size, type, normalize, stride, offset);
            CheckForError();
        }
        internal void BindAttributeLocation(uint program, int index, string name) {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            _bindAttribLocation(program, index, name);
            CheckForError();
        }
        internal int GetAttributeLocation(uint program, string name) {
            var result = _glGetAttribLocation(program, name);
            CheckForError();
            return result;
        }
        internal void BindVertexArray(uint vertexArray) {
            if (_vertexArray == vertexArray)
                return;

            _vertexArray = vertexArray;
            _glBindVertexArray(vertexArray);
            CheckForError();
        }
        internal unsafe void DeleteVertexArray(uint handle) {
            if (_vertexArray == handle)
                _vertexArray = 0;

            _glDeleteVertexArrays(1, &handle);
            CheckForError();
        }
        #endregion

        #region static
        private static void MakeCurrentEmpty() {
            _currentContext.Value = null;
            var result = NativeMethods.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
            //Assert.True(result);
        }
        #endregion

        public void ClearColor(float red, float green, float blue, float alpha) {
            NativeMethods.ClearColor(red, green, blue, alpha);
            CheckForError();
        }
        public void ClearColor(Vector3 color) {
            NativeMethods.ClearColor(color.X, color.Y, color.Z, 1);
            CheckForError();
        }
        public void ClearColor(Vector4 color) {
            NativeMethods.ClearColor(color.X, color.Y, color.Z, color.W);
            CheckForError();
        }
        public void Clear(BufferMask mask) {
            NativeMethods.Clear(mask);
            CheckForError();
        }

        [Obsolete("https://www.opengl.org/wiki/Vertex_Specification#Non-array_attribute_values")]
        public void VertexAttribute(uint attribIndex, Vector4 v) {
            _glVertexAttrib4f(attribIndex, v.X, v.Y, v.Z, v.W);
            CheckForError();
        }

        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, UIVector4 value) {
            ClearBuffer(buffer, drawBuffer, (uint*)&value);
        }
        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, uint value) {
            ClearBuffer(buffer, drawBuffer, &value);
        }
        private unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, uint* value) {
            if (buffer == NetGL.Core.ClearBuffer.Depth || buffer == NetGL.Core.ClearBuffer.Stencil)
                Assert.True(drawBuffer == 0);
            _glClearBufferuiv(buffer, drawBuffer, value);
            CheckForError();
        }

        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, int value) {
            ClearBuffer(buffer, drawBuffer, &value);
        }
        private unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, int* value) {
            if (buffer == NetGL.Core.ClearBuffer.Depth || buffer == NetGL.Core.ClearBuffer.Stencil)
                Assert.True(drawBuffer == 0);
            _glClearBufferiv(buffer, drawBuffer, value);
            CheckForError();
        }

        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, Vector4 value) {
            ClearBuffer(buffer, drawBuffer, (float*)&value);
        }
        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, Vector3 value) {
            ClearBuffer(buffer, drawBuffer, (float*)&value);
        }
        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, Vector2 value) {
            ClearBuffer(buffer, drawBuffer, (float*)&value);
        }
        public unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, float value) {
            ClearBuffer(buffer, drawBuffer, (float*)&value);
        }
        private unsafe void ClearBuffer(NetGL.Core.ClearBuffer buffer, int drawBuffer, float* value) {
            if (buffer == NetGL.Core.ClearBuffer.Depth || buffer == NetGL.Core.ClearBuffer.Stencil)
                Assert.True(drawBuffer == 0);

            _glClearBufferfv(buffer, drawBuffer, value);

            CheckForError();
        }

        public void DrawBuffers(params FramebufferAttachment[] attachments) {
            _glDrawBuffers(attachments.Length, attachments);
            CheckForError();
        }
        public void DrawBuffer(FramebufferAttachment frameBufferAttachment) {
            NativeMethods.DrawBuffer(frameBufferAttachment);
            CheckForError();
        }
        public void DrawBuffer(DrawBufferMode drawBufferMode) {
            NativeMethods.DrawBuffer(drawBufferMode);
            CheckForError();
        }

        public void ReadBuffer(DrawBufferMode drawBufferMode) {
            NativeMethods.ReadBuffer(drawBufferMode);
            CheckForError();
        }
        public void ReadBuffer(FramebufferAttachment frameBufferAttachment) {
            NativeMethods.ReadBuffer(frameBufferAttachment);
            CheckForError();
        }

        public void Viewport(uint x, uint y, uint width, uint height) {
            NativeMethods.Viewport(x, y, width, height);
            CheckForError();
        }
        public void Viewport(int x, int y, int width, int height) {
            NativeMethods.Viewport(x, y, width, height);
            CheckForError();
        }

        public void DrawArrays(PrimitiveType mode, uint first, uint count) {
            NativeMethods.DrawArrays(mode, first, count);
            CheckForError();
        }
        public void DrawElements(PrimitiveType mode, uint count, DrawElementsType type, IntPtr indices) {
            NativeMethods.DrawElements(mode, count, type, indices);
            CheckForError();
        }

        public unsafe int GetInteger(GetName target) {
            int param;
            NativeMethods.GetIntegerv((uint)target, &param);
            CheckForError();

            return param;
        }
        public unsafe float GetFloat(GetName target) {
            float param;
            NativeMethods.GetFloatv(target, &param);
            CheckForError();

            return param;
        }
        public string GetString(StringName name) {
            var ptr = NativeMethods.GetString(name);
            CheckForError();
            return Marshal.PtrToStringAnsi(ptr);
        }

        public void Enable(EnableCap target) {
            NativeMethods.Enable(target);
            CheckForError();
        }
        public void Disable(EnableCap target) {
            NativeMethods.Disable(target);
            CheckForError();
        }

        public void CullFace(MaterialFace face) {
            NativeMethods.CullFace(face);
            CheckForError();
        }
        public void FrontFace(FrontFaceDirection mode) {
            NativeMethods.FrontFace(mode);
            CheckForError();
        }
        public void PolygonMode(MaterialFace face, PolygonMode mode) {
            NativeMethods.PolygonMode(face, mode);
            CheckForError();
        }

        public void BlendEquation(BlendEquations blendEquation) {
            _glBlendEquation(blendEquation);
            CheckForError();
        }

        public void BlendFunc(BlendMode sourceFactor, BlendMode destinationFactor) {
            NativeMethods.BlendFunc(sourceFactor, destinationFactor);
            CheckForError();
        }

        public void DepthMask(bool flag) {
            NativeMethods.DepthMask(flag);
            CheckForError();
        }

        public void MemoryBarrier(MemoryBarrier barriers) {
            _glMemoryBarrier(barriers);
            CheckForError();
        }

        public void ReadPixels(Int32 x, Int32 y, Int32 width, Int32 height, PixelFormat format, PixelType type, IntPtr pixels) {
            NativeMethods.ReadPixels(x, y, width, height, format, type, pixels);
            CheckForError();
        }

        #region DEBUG
        private DebugMessageCallback _debugCallback;
        [Conditional("GL_DEBUG")]
        private void SetupDebugCallback() {
            if (this.Version < new GLVersion(4, 3))
                return;

            Assert.Null(_debugCallback);
            NativeMethods.Enable(EnableCap.DebugOutput);
            _debugCallback = new DebugMessageCallback(DebugCallback);
            _glDebugMessageCallback(_debugCallback, IntPtr.Zero);
        }
        private void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr messagePtr, IntPtr userParam) {
            switch (id) {
                case 131185:
                    return;
                default:
                    break;
            }

            var message = Marshal.PtrToStringAnsi(messagePtr, length);
            var text = string.Format("GL DEBUG source: {0}, type: {1}, id: {2}, severity: {3}, message:\n{4}", source, type, id, severity, message);

            Log.Info(text);
            if (DebugType.DebugTypeError == type)
                throw new GLException(text);
        }
        [Conditional("GL_DEBUG")]
        [DebuggerStepThrough()]
        private void CheckForError() {
            var error = NativeMethods.GetError();
            if (error == Errors.NoError)
                return;

            if (error == Errors.InvalidOperation && NativeMethods.GetCurrentContext() == IntPtr.Zero)
                throw new GLException("No current context");

            throw new GLException(error.ToString());
        }
        #endregion

        #region obsolete
        [Obsolete("use DSA")]
        internal void Uniform(int location, int v0) {
            _glUniform1i(location, v0);
            CheckForError();
        }
        [Obsolete("use DSA")]
        internal void Uniform(int location, float v0) {
            _glUniform1f(location, v0);
            CheckForError();
        }
        [Obsolete("use DSA")]
        internal void Uniform(int location, Vector2 v) {
            _glUniform2f(location, v.X, v.Y);
            CheckForError();
        }
        [Obsolete("use DSA")]
        internal void Uniform(int location, Vector3 v) {
            _glUniform3f(location, v.X, v.Y, v.Z);
            CheckForError();
        }
        [Obsolete("use DSA")]
        internal void Uniform(int location, Vector4 v) {
            _glUniform4f(location, v.X, v.Y, v.Z, v.W);
            CheckForError();
        }
        [Obsolete("use DSA")]
        internal void UniformMatrix(int location, int count, bool transpose, float[] value) {
            switch (value.Length) {
                case 16:
                    _glUniformMatrix4fv(location, count, transpose, value);
                    CheckForError();
                    return;
                case 9:
                    _glUniformMatrix3fv(location, count, transpose, value);
                    CheckForError();
                    return;
                default:
                    throw new NotSupportedException(value.Length.ToString());
            }
        }
        #endregion
    }
}