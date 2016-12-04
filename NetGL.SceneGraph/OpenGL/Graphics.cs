using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Shaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace NetGL.SceneGraph.OpenGL {
    public enum ShadingTechnique {
        Forward,
        Deferred
    }

    internal class Graphics : IDisposable {
        private static readonly ThreadLocal<Graphics> _currentContext = new ThreadLocal<Graphics>();

        public static Graphics GetCurrent(bool throwIfNull) {
            var result = _currentContext.Value;
            if (result == null && throwIfNull)
                throw new ContextMissingException();
            return result;
        }

        private int _swapInterval = 0;
        private int _sampleCount = 1;

        private readonly IRenderTechnique _renderTechnique;

        private readonly GL _glContext;
        private readonly SceneTime _time;
        private readonly UserControl _control;

        private Framebuffer _backbuffer = null;
        private Size _screenSize;

        private readonly VertexArrayObject _displayQuadVao;
        private readonly Material _defaultMaterial;
        private readonly QuadShader _quadShader;
        private readonly Texture2 _blankTexture;
        private readonly GlobalUniformsBufferObject _globalUniforms;
        private readonly StandardUniformsBufferObject _standartUniforms;

        //private UIRender _uiRender;

        private readonly MeshVaoCollection _meshVaoCollection = new MeshVaoCollection();

        private readonly Matrix _mvpMatrix = new Matrix();
        private readonly Matrix _mvMatrix = new Matrix();
        private readonly Matrix _normalMatrix = new Matrix();
        //private readonly Matrix _shadowMatrix = new Matrix();

        public Boolean IsDisposed { get; private set; }
        public Size ScreenSize {
            get { return _screenSize; }
            internal set {
                if (_screenSize == value)
                    return;
                _screenSize = value;

                ChangeScreenSize();
            }
        }
        public int DrawCalls { get; internal set; }
        public GL GLContext {
            get {
                return _glContext;
            }
        }
        public bool WaitVBlanc {
            get {
                return _swapInterval == 1;
            }
            set {
                if (WaitVBlanc == value)
                    return;

                _swapInterval = value ? 1 : 0;
                _glContext.SwapInterval(_swapInterval);
            }
        }
        public int SampleCount {
            get {
                return _sampleCount;
            }
            set {
                if (value < 1 || value > _glContext.MaxSamples)
                    throw new ArgumentOutOfRangeException("SampleCount");

                if (_sampleCount == value)
                    return;

                _sampleCount = value;
                ChangeScreenSize();
            }
        }
        public ShadingTechnique CurrentShadingTechnique { get; private set; }

        internal GlobalUniformsBufferObject GlobalUniforms { get { return _globalUniforms; } }
        internal StandardUniformsBufferObject StandartUniforms { get { return _standartUniforms; } }
        internal MeshVaoCollection MeshVaoCollection { get { return _meshVaoCollection; } }
        internal Framebuffer Backbuffer { get { return _backbuffer; } }

        public Graphics(UserControl control, SceneTime time, ShadingTechnique technique) {
            Assert.NotNull(control);
            Assert.NotNull(time);

            IsDisposed = false;
            CurrentShadingTechnique = technique;

            if (GL.GetCurrent(false) != null || GetCurrent(false) != null) {
                var error = "Only one NetGL view per thread is now supported. Try launching 3D view in different thread.";
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            _control = control;
            _time = time;

            try {
                _glContext = new GL(_control.Handle);
            }
            catch {
                Dispose();
                throw;
            }

            _currentContext.Value = this;

            _glContext.Enable(EnableCap.DepthTest);
            _glContext.Enable(EnableCap.Texture2D);
            _glContext.Enable(EnableCap.CullFace);
            _glContext.Enable(EnableCap.ProgramPointSize);
            _glContext.Enable(EnableCap.PointSprite);
            _glContext.CullFace(MaterialFace.Back);
            _glContext.FrontFace(FrontFaceDirection.CounterClockwise);
            _glContext.BlendEquation(BlendEquations.FuncAdd);
            _glContext.BlendFunc(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);

            _glContext.SwapInterval(_swapInterval);

            _globalUniforms = new GlobalUniformsBufferObject();
            _standartUniforms = new StandardUniformsBufferObject();
            _defaultMaterial = new Material(MaterialType.DiffuseColor);
            _blankTexture = new Texture2();
            _blankTexture.SetImage(new byte[4] { 255, 0, 255, 255 }, new Size(1, 1), Core.PixelFormat.Rgba, Core.PixelInternalFormat.Rgba8);
            _quadShader = new QuadShader();

            _displayQuadVao = CreateDisplayQuadVao();

            switch (CurrentShadingTechnique) {
                case ShadingTechnique.Forward:
                    _renderTechnique = new ForwardRender();
                    break;
                case ShadingTechnique.Deferred:
                    _renderTechnique = new DeferredRender();
                    break;
                default:
                    throw new NotSupportedException(CurrentShadingTechnique.ToString());
            }

            ScreenSize = _control.ClientSize;
        }

        public void Dispose() {
            if (IsDisposed)
                return;
            IsDisposed = true;

            GC.SuppressFinalize(this);
            _currentContext.Value = null;

            Disposer.Dispose(_globalUniforms);
            Disposer.Dispose(_standartUniforms);
            Disposer.Dispose(_renderTechnique);
            Disposer.Dispose(_meshVaoCollection);

            Disposer.Dispose(_defaultMaterial);
            Disposer.Dispose(_blankTexture);

            Disposer.Dispose(_glContext);
        }
        ~Graphics() {
            Log.Error("OpenGLGraphics was not disposed!");
        }

        public void RenderEmpty() {
            Check();

            _glContext.BindDefaultFramebuffer(FramebufferTarget.Framebuffer);
            _glContext.ClearColor(new Vector3(0.8f, 0.1f, 0.8f));
            _glContext.Clear(BufferMask.ColorBufferBit);

            _glContext.SwapBuffers();
        }
        public Bitmap ReadImage() {
            return _backbuffer.ReadColorImage(FramebufferAttachment.ColorAttachment0);
        }

        [Conditional("DEBUG")]
        private void Check() {
            if (IsDisposed)
                throw new ObjectDisposedException("OpenGLGraphics");

            if (_glContext.IsCurrent() == false)
                throw new GLException("Graphics context has beend changed. Only one NetGL context per thread is now supported.");
        }

        internal void RenderAll(IReadOnlyList<Camera> cameras, IReadOnlyList<LightSource> lights, RendererCollection renderables) {
            Check();

            _renderTechnique.RenderAll(cameras, lights, renderables);

            //_uiRender.Render();

            SwapBuffers();
        }

        private void SwapBuffers() {
            _glContext.Disable(EnableCap.Multisample);

            _glContext.BindDefaultFramebuffer(FramebufferTarget.DrawFramebuffer);
            _glContext.DrawBuffer(DrawBufferMode.Back);

            Backbuffer.Bind(FramebufferTarget.ReadFramebuffer);
            _glContext.ReadBuffer(FramebufferAttachment.ColorAttachment0);

            _glContext.BlitFramebuffer(0, 0, _screenSize.Width, _screenSize.Height, 0, 0, _screenSize.Width, _screenSize.Height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            _glContext.SwapBuffers();
        }

        private void ChangeScreenSize() {
            Disposer.Dispose(ref _backbuffer);
            _backbuffer = new Framebuffer(_screenSize, SampleCount);
            _backbuffer.Bind();
            var renderBuffer = _backbuffer.AddColorRenderbuffer(FramebufferAttachment.ColorAttachment0);
            _backbuffer.AddDepthRenderbuffer();
            _backbuffer.CheckStatus(true);

            _renderTechnique.ScreenSize = _screenSize;

            //_uiRender = new UIRender(renderBuffer);
        }
        internal void SetupCameraAndLights(Camera camera, IReadOnlyList<LightSource> lights) {
            camera.UpdateMatrix(camera.Transform.ModelMatrix);

            _globalUniforms.ClearLights();
            _globalUniforms.Ambient = camera.AmbientLight;
            _globalUniforms.CameraPosition = camera.Transform.WorldPosition;

            var lightCount = Math.Min(lights.Count, GlobalUniformsBufferObject.MaxLightCount);

            for (int j = lightCount - 1; j >= 0; j--) {
                var light = lights[j];
                if (light.IsEnabled) {
                    light.Update(camera);
                    _globalUniforms.AddLight(light.Light);
                    _globalUniforms.LightCastingShadowNumber = j;
                }
            }

            _globalUniforms.Update();
        }

        internal void UpdateAndRender(Camera camera, Renderable renderable, uint id) {
            if (renderable.IsEnabled == false)
                return;

            renderable.IsVisible = CheckIfVisible(camera, renderable);
            if (renderable.IsVisible == false)
                return;

            var renderer = renderable.Renderer;
            var material = renderable.Material;
            if (material == null)
                material = _defaultMaterial;

            Assert.NotNull(renderer);
            Assert.NotNull(material);

            renderer.Prerender();
            BindMaterial(material);
            UpdateStandartUniforms(camera, renderable.Transform, material.Shader, id);
            renderer.Render();

            DrawCalls++;
        }
        internal bool CheckIfVisible(Camera camera, Renderable renderable) {
            var sphere = renderable.Renderer.BoundSphere;
            Assert.NotNull(sphere);

            var volumeCenter = sphere.Center + renderable.Transform.WorldPosition;
            return camera.Frustum.SphereInsideFrustum(volumeCenter, sphere.Radius);
        }
        internal void BindMaterial(Material material) {
            BindTextures(material);

            material.Shader.Bind();

            if (material.RenderQueue != RenderQueue.Transparent)
                return;

            switch (material.BlendMode) {
                case BlendingMode.Normal:
                    _glContext.BlendFunc(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendingMode.Additive:
                    _glContext.BlendFunc(BlendMode.SrcAlpha, BlendMode.One);
                    break;
                default:
                    throw new NotSupportedException(material.BlendMode.ToString());
            }
        }
        internal void BindTextures(Material material) {
            int textureSlot = 1;
            for (int i = 0; i < material.Textures.Count; i++) {
                var unit = material.Textures[i];
                unit.Uniform.Value = textureSlot;
                BindTexture(textureSlot, unit.Texture);
                textureSlot++;
            }
        }
        private void BindTexture(int textureSlot, Texture texture) {
            if (texture == null)
                texture = _blankTexture;

            var textureUnit = TextureUnit.Texture0 + textureSlot;
            texture.Bind(textureUnit);
        }
        internal void UpdateStandartUniforms(Camera camera, Transform transform, ShaderProgram shader, uint id) {
            Assert.True(id > 0u);

            Matrix normalPass = null, mvPass = null, mPass = null;

            var projectionMatrix = camera.ProjectionMatrix;
            var viewMatrix = camera.ViewMatrix;
            var modelMatrix = transform.ModelMatrix;

            Matrix.Multiply(viewMatrix, modelMatrix, _mvMatrix);
            Matrix.Multiply(projectionMatrix, _mvMatrix, _mvpMatrix);

            if (shader.UsesNormalMatrix) {
                Matrix.Invert(_mvMatrix, _normalMatrix);
                normalPass = _normalMatrix;
            }
            if (shader.UsesModelViewMatrix)
                mvPass = _mvMatrix;
            if (shader.UsesModelMatrix)
                mPass = modelMatrix;

            _standartUniforms.Update(_mvpMatrix, mvPass, mPass, normalPass, shader.UsesInversedModelMatrix, _time.CurrentFloat, id);
        }

        public void Blit(Material material) {
            Check();

            BindMaterial(material);

            _displayQuadVao.Bind();

            _glContext.Disable(EnableCap.DepthTest);
            _glContext.DepthMask(false);

            _glContext.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            _glContext.DepthMask(true);
            _glContext.Enable(EnableCap.DepthTest);
        }

        private static VertexArrayObject CreateDisplayQuadVao() {
            var vao = new VertexArrayObject();

            vao.SetAttributeData(StandardAttribute.Position,
                new Vector2Buffer(new Vector2[4] {
                        new Vector2(-1, -1),
                        new Vector2( 1, -1),
                        new Vector2(-1,  1),                                  
                        new Vector2( 1,  1)
                    }), false);
            vao.SetAttributeData(StandardAttribute.TexCoord,
                new Vector2Buffer(new Vector2[4] {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
            }), false);
            return vao;
        }
    }
}





//[Obsolete]
//internal void RenderShadowMap(OpenGLGraphics graphics) {
//    if (_mesh == null)
//        return;

//    if (!InFrustum(graphics._camera.Frustum))
//        return;

//    if (_vao == null) {
//        _vao = new MeshVertexArrayRenderer();
//        _vao.Mesh = _mesh;
//    }

//    Matrix.Multiply(SceneObject._transform.ModelMatrix, graphics._camera.ViewProjectionMatrix, _shadowMatrix);

//    //graphics.StandartUniformsBufferObject.Update(_shadowMatrix);

//    _vao.RenderShadowMap();
//}

//[Obsolete]
//private void RenderShadowMap() {
//    if (_lights.Count == 0) {
//        _graphics.HasShadowMap = false;
//        _graphics.LightCastingShadowNumber = -1;
//        return;
//    }

//    var light = _lights[0];
//    if (!light.CastShadow) {
//        _graphics.HasShadowMap = false;
//        _graphics.LightCastingShadowNumber = -1;
//        return;
//    }

//    _graphics.LightCastingShadowNumber = 0;
//    _graphics.HasShadowMap = true;

//    _renderInfo.ShadowMapFrameBuffer.Bind(FramebufferTarget.Framebuffer);
//    _renderInfo.ShadowMaterial.Bind();

//    GL.DrawBuffer(DrawBufferMode.None);
//    GL.CheckForError();

//    _shadowCamera.UpdateMatrix(light.SceneObject._transform.ModelMatrix);

//    _renderInfo.Update(_shadowCamera);

//    GL.Clear(BufferMask.DepthBufferBit);
//    GL.Viewport(1, 1, _renderInfo.ShadowMapResolution.Width - 2, _renderInfo.ShadowMapResolution.Height - 2);
//    GL.CullFace(MaterialFace.Front);

//    //_rendererCollection.RenderShadowMap();
//    GL.CheckForError();

//    GL.CullFace(MaterialFace.Back);
//    GL.Viewport(0, 0, _screenSize.Width, _screenSize.Height);
//    _renderInfo.ShadowMap.Bind(TextureUnit.Texture0);
//}
//[Obsolete]
//public void RenderShadowMap() {
//    _vertexBufferArray.Bind();

//    if (_shouldUpdateMesh)
//        UpdateMeshInternal();

//    GL.CheckForError();

//    if (_isIndexed)
//        GL.DrawElements(_primitiveType, _verticesCount, _indexType, IntPtr.Zero);
//    else
//        GL.DrawArrays(_primitiveType, 0, _verticesCount);

//    GL.CheckForError();
//}