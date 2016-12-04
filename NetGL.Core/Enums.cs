using System;

namespace NetGL.Core {
    [Flags]
    public enum BufferMask : uint {
        DepthBufferBit = 0x00000100,
        StencilBufferBit = 0x00000400,
        ColorBufferBit = 0x00004000
    }

    public enum EnableCap : int {
        PointSmooth = 0x0B10,
        LineSmooth = 0x0B20,
        LineStipple = 0x0B24,
        PolygonSmooth = 0x0B41,
        PolygonStipple = 0x0B42,
        CullFace = 0x0B44,
        DepthTest = 0x0B71,
        StencilTest = 0x0B90,
        AlphaTest = 0x0BC0,
        Blend = 0x0BE2,
        Texture2D = 0x0DE1,
        PolygonOffsetPoint = 0x2A01,
        PolygonOffsetLine = 0x2A02,
        Multisample = 0x809D,
        ProgramPointSize = 0x8642,
        PointSprite = 0x8861,
        DebugOutput = 0x92E0
    }

    public enum ClearBuffer {
        Color = 0x1800,
        Depth = 0x1801,
        Stencil = 0x1802
    }

    public enum PrimitiveType : uint {
        Points = 0x0000,
        Lines = 0x0001,
        LineStrip = 0x0003,
        Triangles = 0x0004,
        TriangleStrip = 0x0005
    }

    public enum ShaderType : uint {
        VertexShader = 0x8B31,
        FragmentShader = 0x8B30,
        GeometryShader = 0x8DD9,
        ComputeShader = 0x91B9
    }

    public enum ShaderInfo : uint {
        InfoLogLength = 0x8B84,
        DeleteStatus = 0x8B80,
        CompileStatus = 0x8B81
    }
    public enum ProgramInfo : uint {
        LinkStatus = 0x8B82,
        ValidateStatus = 0x8B83,
        InfoLogLength = 0x8B84,
        ActiveUniforms = 0x8B86,
        ActiveUniformMaxLength = 0x8B87,
        ActiveUniformBlocks = 0x8A36,
        ComputeWorkGroupSize = 0x8267
    }

    public enum PolygonMode : uint {
        Point = 0x1B00,
        Line = 0x1B01,
        Fill = 0x1B02
    }

    public enum MaterialFace : uint {
        Front = 0x0404,
        Back = 0x0405,
        FrontAndBack = 0x0408
    }

    public enum FrontFaceDirection : uint {
        Clockwise = 0x0900,
        CounterClockwise = 0x0901
    }

    public enum TextureFilter : int {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipmapNearest = 0x2700,
        LinearMipmapNearest = 0x2701,
        NearestMipmapLinear = 0x2702,
        LinearMipmapLinear = 0x2703
    }

    public enum TextureTarget : int {
        Texture2D = 0x0DE1,
        Texture2DMultisample = 0x9100,
        TextureCubeMap = 0x8513,
        TextureCubeMapPositiveX = ((int)0x8515),
        TextureCubeMapNegativeX = ((int)0x8516),
        TextureCubeMapPositiveY = ((int)0x8517),
        TextureCubeMapNegativeY = ((int)0x8518),
        TextureCubeMapPositiveZ = ((int)0x8519),
        TextureCubeMapNegativeZ = ((int)0x851A),
    }
    public enum PixelInternalFormat : int {
        R3G3B2 = 0x2A10,
        Alpha4 = 0x803B,
        Alpha8 = 0x803C,
        Alpha12 = 0x803D,
        Alpha16 = 0x803E,
        Luminance4 = 0x803F,
        Luminance8 = 0x8040,
        Luminance12 = 0x8041,
        Luminance16 = 0x8042,
        Luminance4Alpha4 = 0x8043,
        Luminance6Alpha2 = 0x8044,
        Luminance8Alpha8 = 0x8045,
        Luminance12Alpha4 = 0x8046,
        Luminance12Alpha12 = 0x8047,
        Luminance16Alpha16 = 0x8048,
        Intensity = 0x8049,
        Intensity4 = 0x804A,
        Intensity8 = 0x804B,
        Intensity12 = 0x804C,
        Intensity16 = 0x804D,
        Rgb4 = 0x804F,
        Rgb5 = 0x8050,
        Rgb8 = 0x8051,
        Rgb10 = 0x8052,
        Rgb12 = 0x8053,
        Rgb16 = 0x8054,
        Rgba2 = 0x8055,
        Rgba4 = 0x8056,
        Rgb5A1 = 0x8057,
        Rgba8 = 0x8058,
        Rgb10A2 = 0x8059,
        Rgba12 = 0x805A,
        Rgba16 = 0x805B,
        DepthComponent16 = 0x81a5,
        DepthComponent24 = 0x81a6,
        DepthComponent32 = 0x81a7,
        R8 = 0x8229,
        R16 = 0x822A,
        Rg8 = 0x822B,
        Rg16 = 0x822C,
        R16f = 0x822D,
        R32f = 0x822E,
        Rg16f = 0x822F,
        Rg32f = 0x8230,
        R8i = 0x8231,
        R8ui = 0x8232,
        R16i = 0x8233,
        R16ui = 0x8234,
        R32i = 0x8235,
        R32ui = 0x8236,
        Rg8i = 0x8237,
        Rg8ui = 0x8238,
        Rg16i = 0x8239,
        Rg16ui = 0x823A,
        Rg32i = 0x823B,
        Rg32ui = 0x823C,
        CompressedRgbS3tcDxt1Ext = 0x83F0,
        CompressedRgbaS3tcDxt1Ext = 0x83F1,
        CompressedRgbaS3tcDxt3Ext = 0x83F2,
        CompressedRgbaS3tcDxt5Ext = 0x83F3,
        CompressedAlpha = 0x84E9,
        CompressedLuminance = 0x84EA,
        CompressedLuminanceAlpha = 0x84EB,
        CompressedIntensity = 0x84EC,
        CompressedRgb = 0x84ED,
        CompressedRgba = 0x84EE,
        DepthStencil = 0x84F9,
        Rgba32f = 0x8814,
        Rgb32f = 0x8815,
        Rgba16f = 0x881A,
        Rgb16f = 0x881B,
        Depth24Stencil8 = 0x88F0,
        R11fG11fB10f = 0x8C3A,
        Rgb9E5 = 0x8C3D,
        Srgb = 0x8C40,
        Srgb8 = 0x8C41,
        SrgbAlpha = 0x8C42,
        Srgb8Alpha8 = 0x8C43,
        SluminanceAlpha = 0x8C44,
        Sluminance8Alpha8 = 0x8C45,
        Sluminance = 0x8C46,
        Sluminance8 = 0x8C47,
        CompressedSrgb = 0x8C48,
        CompressedSrgbAlpha = 0x8C49,
        CompressedSluminance = 0x8C4A,
        CompressedSluminanceAlpha = 0x8C4B,
        CompressedSrgbS3tcDxt1Ext = 0x8C4C,
        CompressedSrgbAlphaS3tcDxt1Ext = 0x8C4D,
        CompressedSrgbAlphaS3tcDxt3Ext = 0x8C4E,
        CompressedSrgbAlphaS3tcDxt5Ext = 0x8C4F,
        DepthComponent32f = 0x8CAC,
        Depth32fStencil8 = 0x8CAD,
        Rgba32ui = 0x8D70,
        Rgb32ui = 0x8D71,
        Rgba16ui = 0x8D76,
        Rgb16ui = 0x8D77,
        Rgba8ui = 0x8D7C,
        Rgb8ui = 0x8D7D,
        Rgba32i = 0x8D82,
        Rgb32i = 0x8D83,
        Rgba16i = 0x8D88,
        Rgb16i = 0x8D89,
        Rgba8i = 0x8D8E,
        Rgb8i = 0x8D8F,
        Float32UnsignedInt248Rev = 0x8DAD,
        CompressedRedRgtc1 = 0x8DBB,
        CompressedSignedRedRgtc1 = 0x8DBC,
        CompressedRgRgtc2 = 0x8DBD,
        CompressedSignedRgRgtc2 = 0x8DBE,
        CompressedRgbaBptcUnorm = 0x8E8C,
        CompressedRgbBptcSignedFloat = 0x8E8E,
        CompressedRgbBptcUnsignedFloat = 0x8E8F,
        R8Snorm = 0x8F94,
        Rg8Snorm = 0x8F95,
        Rgb8Snorm = 0x8F96,
        Rgba8Snorm = 0x8F97,
        R16Snorm = 0x8F98,
        Rg16Snorm = 0x8F99,
        Rgb16Snorm = 0x8F9A,
        Rgba16Snorm = 0x8F9B,
        Rgb10A2ui = 0x906F,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
    }
    public enum PixelFormat : int {
        StencilIndex = 0x1901,
        DepthComponent = 0x1902,
        Red = 0x1903,
        Rgb = 0x1907,
        Rgba = 0x1908,
        Bgr = 0x80E0,
        Bgra = 0x80E1,
        Rg = 0x8227,
        DepthStencil = 0x84F9
    }

    public enum GetName : int {
        MajorVersion = 0x821B,
        MinorVersion = 0x821C,
        MaxTextureSize = 0x0D33,
        //MaxFramebufferSamples = 0x9318,
        MaxTextureImageUnits = 0x8872,
        MaxSamples = 0x8D57,
        MaxColorAttachments = 0x8CDF
    }
    public enum TextureCompareMode : int {
        None = 0,
        CompareRefToTexture = 0x884E
    }
    public enum DepthFunc : int {
        Never = 0x0200,
        Less = 0x0201,
        Equal = 0x0202,
        Lequal = 0x0203,
        Greater = 0x0204,
        Notequal = 0x0205,
        Gequal = 0x0206,
        Always = 0x0207,
    }

    public enum DrawBufferMode : uint {
        None = 0,
        Front = 0x0404,
        Back = 0x0405,
    }

    public enum BlendEquations : uint {
        FuncAdd = 0x8006,
        FuncSubtract = 0x800A,
        FuncReverseSubtract = 0x800B
    }
    public enum BlendMode : uint {
        Zero = 0,
        One = 1,
        SrcColor = 0x0300,
        OneMinusSrcColor = 0x0301,
        SrcAlpha = 0x0302,
        OneMinusSrcAlpha = 0x0303,
        DstAlpha = 0x0304,
        OneMinusDstAlpha = 0x0305,
        DstColor = 0x0306,
        OneMinusDstColor = 0x0307
    }

    public enum StringName : uint {
        Vendor = 0x1F00,
        Renderer = 0x1F01,
        Version = 0x1F02,
        Extenstions = 0x1F03,
        ShadingLanguageVersion = 0x8B8C
    }

    public enum Errors : uint {
        NoError = 0,
        InvalidEnum = 0x0500,
        InvalidValue = 0x0501,
        InvalidOperation = 0x0502,
        StackOverflow = 0x0503,
        StackUnderflow = 0x0504,
        OutOfMemory = 0x0505,
        InvalidFrameBufferOperation = 0x0506,
        ContextLost = 0x0507
    }

    public enum MemoryBarrier : uint {
        ShaderStorageBarrierBit = 0x00002000,
        VertexAttribArrayBarrierBit = 0x00000001,
        AllBarrierBits = 0xFFFFFFFF
    }

    public enum BlitFramebufferFilter : int {
        Nearest = 0x2600,
        Linear = 0x2601,
    }

    public enum BindBufferTarget : uint {
        ArrayBuffer = 0x8892,
        ElementArrayBuffer = 0x8893,
        UniformBuffer = 0x8A11,
        ShaderStorageBuffer = 0x90D2
    }

    public enum BufferUsage : uint {
        StreamDraw = 0x88E0,
        StreamRead = 0x88E1,
        StreamCopy = 0x88E2,
        StaticDraw = 0x88E4,
        StaticRead = 0x88E5,
        StaticCopy = 0x88E6,
        DynamicDraw = 0x88E8,
        DynamicRead = 0x88E9,
        DynamicCopy = 0x88EA
    }
    public enum TextureWrapMode : int {
        Clamp = 0x2900,
        Repeat = 0x2901,
        ClampToBorder = 0x812D,
        ClampToEdge = 0x812F,
        MirroredRepeat = 0x8370,
    }
    public enum VertexAttribPointerType : int {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        Double = 0x140A,
        Fixed = 0x140C,
    }
    public enum DrawElementsType : int {
        UnsignedByte = 0x1401,
        UnsignedShort = 0x1403,
        UnsignedInt = 0x1405,
    }
    public enum PixelType : int {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        HalfFloat = 0x140B,
        UnsignedByte332 = 0x8032,
        UnsignedShort4444 = 0x8033,
        UnsignedShort5551 = 0x8034,
        UnsignedInt8888 = 0x8035,
        UnsignedInt1010102 = 0x8036,
        UnsignedShort565 = 0x8363,
        UnsignedInt248 = 0x84FA,
    }
    public enum GenerateMipmapTarget : int {
        Texture2D = 0x0DE1,
        TextureCubeMap = 0x8513,
        Texture2DMultisample = 0x9100,
    }
    public enum TextureUnit : int {
        Texture0 = 0x84C0,
        Texture1 = 0x84C1,
        Texture2 = 0x84C2,
        Texture3 = 0x84C3,
        Texture4 = 0x84C4,
        Texture5 = 0x84C5,
        Texture6 = 0x84C6,
        Texture7 = 0x84C7,
        Texture8 = 0x84C8,
        Texture9 = 0x84C9,
        Texture10 = 0x84CA,
        Texture11 = 0x84CB,
        Texture12 = 0x84CC,
        Texture13 = 0x84CD,
        Texture14 = 0x84CE,
        Texture15 = 0x84CF,
        Texture16 = 0x84D0,
        Texture17 = 0x84D1,
        Texture18 = 0x84D2,
        Texture19 = 0x84D3,
        Texture20 = 0x84D4,
        Texture21 = 0x84D5,
        Texture22 = 0x84D6,
        Texture23 = 0x84D7,
        Texture24 = 0x84D8,
        Texture25 = 0x84D9,
        Texture26 = 0x84DA,
        Texture27 = 0x84DB,
        Texture28 = 0x84DC,
        Texture29 = 0x84DD,
        Texture30 = 0x84DE,
        Texture31 = 0x84DF,
    }

    public enum TextureParameters : uint {
        TextureMagFilter = 0x2800,
        TextureMinFilter = 0x2801,
        TextureWrapS = 0x2802,
        TextureWrapT = 0x2803,
        TextureWrapR = 0x8072,
        TextureMaxAnisotropy = 0x84FE,
        TextureCompareMode = 0x884C,
        TextureCompareFunc = 0x884D
    }
    public enum FramebufferTarget : uint {
        ReadFramebuffer = 0x8CA8,
        DrawFramebuffer = 0x8CA9,
        Framebuffer = 0x8D40
    }
    public enum FramebufferAttachment : uint {
        ColorAttachment0 = 0x8CE0,
        ColorAttachment1 = 0x8CE1,
        ColorAttachment2 = 0x8CE2,
        ColorAttachment3 = 0x8CE3,
        ColorAttachment4 = 0x8CE4,
        ColorAttachment5 = 0x8CE5,
        ColorAttachment6 = 0x8CE6,
        ColorAttachment7 = 0x8CE7,
        ColorAttachment8 = 0x8CE8,
        ColorAttachment9 = 0x8CE9,
        ColorAttachment10 = 0x8CEA,
        ColorAttachment11 = 0x8CEB,
        ColorAttachment12 = 0x8CEC,
        ColorAttachment13 = 0x8CED,
        ColorAttachment14 = 0x8CEE,
        ColorAttachment15 = 0x8CEF,
        DepthAttachment = 0x8D00,
        StencilAttachement = 0x8D20
    }
    public enum RenderbufferBindTarget : uint {
        Renderbuffer = 0x8D41
    }
    public enum RenderbufferStorage : uint {
        DepthComponent16 = 0x81A5,
        DepthComponent24 = 0x81A6,
        DepthComponent32 = 0x81A7,
        Rgba8 = 0x8058,
        Depth24Stencil8 = 0x88F0
    }
    public enum FramebufferStatus : uint {
        Complete = 0x8CD5,
        IncompleteAttachment = 0x8CD6,
        MissingAttachment = 0x8CD7,
        IncompleteDrawBuffer = 0x8CDB,
        IncompleteReadBuffer = 0x8CDC,
        Unsupported = 0x8CDD,
        IncompleteLayerTargets = 0x8DA8,
        IncompleteMultisample = 0x8D56
    }

    public enum MapBufferAccess : uint {
        ReadOnly = 0x88B8,
        WriteOnly = 0x88B9,
        ReadWrite = 0x88BA
    }

    public enum ActiveUniformType : int {
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        Double = 0x140A,
        FloatVec2 = 0x8B50,
        FloatVec3 = 0x8B51,
        FloatVec4 = 0x8B52,
        IntVec2 = 0x8B53,
        IntVec3 = 0x8B54,
        IntVec4 = 0x8B55,
        Bool = 0x8B56,
        BoolVec2 = 0x8B57,
        BoolVec3 = 0x8B58,
        BoolVec4 = 0x8B59,
        FloatMat2 = 0x8B5A,
        FloatMat3 = 0x8B5B,
        FloatMat4 = 0x8B5C,
        Sampler1D = 0x8B5D,
        Sampler2D = 0x8B5E,
        Sampler3D = 0x8B5F,
        SamplerCube = 0x8B60,
        Sampler1DShadow = 0x8B61,
        Sampler2DShadow = 0x8B62,
        Sampler2DRect = 0x8B63,
        Sampler2DRectShadow = 0x8B64,
        FloatMat2x3 = 0x8B65,
        FloatMat2x4 = 0x8B66,
        FloatMat3x2 = 0x8B67,
        FloatMat3x4 = 0x8B68,
        FloatMat4x2 = 0x8B69,
        FloatMat4x3 = 0x8B6A,
        Sampler1DArray = 0x8DC0,
        Sampler2DArray = 0x8DC1,
        SamplerBuffer = 0x8DC2,
        Sampler1DArrayShadow = 0x8DC3,
        Sampler2DArrayShadow = 0x8DC4,
        SamplerCubeShadow = 0x8DC5,
        UnsignedIntVec2 = 0x8DC6,
        UnsignedIntVec3 = 0x8DC7,
        UnsignedIntVec4 = 0x8DC8,
        IntSampler1D = 0x8DC9,
        IntSampler2D = 0x8DCA,
        IntSampler3D = 0x8DCB,
        IntSamplerCube = 0x8DCC,
        IntSampler2DRect = 0x8DCD,
        IntSampler1DArray = 0x8DCE,
        IntSampler2DArray = 0x8DCF,
        IntSamplerBuffer = 0x8DD0,
        UnsignedIntSampler1D = 0x8DD1,
        UnsignedIntSampler2D = 0x8DD2,
        UnsignedIntSampler3D = 0x8DD3,
        UnsignedIntSamplerCube = 0x8DD4,
        UnsignedIntSampler2DRect = 0x8DD5,
        UnsignedIntSampler1DArray = 0x8DD6,
        UnsignedIntSampler2DArray = 0x8DD7,
        UnsignedIntSamplerBuffer = 0x8DD8,
        DoubleVec2 = 0x8FFC,
        DoubleVec3 = 0x8FFD,
        DoubleVec4 = 0x8FFE,
        SamplerCubeMapArray = 0x900C,
        SamplerCubeMapArrayShadow = 0x900D,
        IntSamplerCubeMapArray = 0x900E,
        UnsignedIntSamplerCubeMapArray = 0x900F,
        Image1D = 0x904C,
        Image2D = 0x904D,
        Image3D = 0x904E,
        Image2DRect = 0x904F,
        ImageCube = 0x9050,
        ImageBuffer = 0x9051,
        Image1DArray = 0x9052,
        Image2DArray = 0x9053,
        ImageCubeMapArray = 0x9054,
        Image2DMultisample = 0x9055,
        Image2DMultisampleArray = 0x9056,
        IntImage1D = 0x9057,
        IntImage2D = 0x9058,
        IntImage3D = 0x9059,
        IntImage2DRect = 0x905A,
        IntImageCube = 0x905B,
        IntImageBuffer = 0x905C,
        IntImage1DArray = 0x905D,
        IntImage2DArray = 0x905E,
        IntImageCubeMapArray = 0x905F,
        IntImage2DMultisample = 0x9060,
        IntImage2DMultisampleArray = 0x9061,
        UnsignedIntImage1D = 0x9062,
        UnsignedIntImage2D = 0x9063,
        UnsignedIntImage3D = 0x9064,
        UnsignedIntImage2DRect = 0x9065,
        UnsignedIntImageCube = 0x9066,
        UnsignedIntImageBuffer = 0x9067,
        UnsignedIntImage1DArray = 0x9068,
        UnsignedIntImage2DArray = 0x9069,
        UnsignedIntImageCubeMapArray = 0x906A,
        UnsignedIntImage2DMultisample = 0x906B,
        UnsignedIntImage2DMultisampleArray = 0x906C,
        Sampler2DMultisample = 0x9108,
        IntSampler2DMultisample = 0x9109,
        UnsignedIntSampler2DMultisample = 0x910A,
        Sampler2DMultisampleArray = 0x910B,
        IntSampler2DMultisampleArray = 0x910C,
        UnsignedIntSampler2DMultisampleArray = 0x910D,
        UnsignedIntAtomicCounter = 0x92DB,
    }
    public enum ActiveUniformBlockParameter : int {
        UniformBlockReferencedByTessControlShader = 0x84F0,
        UniformBlockReferencedByTessEvaluationShader = 0x84F1,
        UniformBlockBinding = 0x8A3F,
        UniformBlockDataSize = 0x8A40,
        UniformBlockNameLength = 0x8A41,
        UniformBlockActiveUniforms = 0x8A42,
        UniformBlockActiveUniformIndices = 0x8A43,
        UniformBlockReferencedByVertexShader = 0x8A44,
        UniformBlockReferencedByGeometryShader = 0x8A45,
        UniformBlockReferencedByFragmentShader = 0x8A46,
        UniformBlockReferencedByComputeShader = 0x90EC,
    }
    public enum ActiveUniformParameter : int {
        UniformType = 0x8A37,
        UniformSize = 0x8A38,
        UniformNameLength = 0x8A39,
        UniformBlockIndex = 0x8A3A,
        UniformOffset = 0x8A3B,
        UniformArrayStride = 0x8A3C,
        UniformMatrixStride = 0x8A3D,
        UniformIsRowMajor = 0x8A3E,
        UniformAtomicCounterBufferIndex = 0x92DA,
    }

    public enum DebugSeverity : int {
        DebugSeverityNotification = ((int)0x826B),
        DebugSeverityHigh = ((int)0x9146),
        DebugSeverityMedium = ((int)0x9147),
        DebugSeverityLow = ((int)0x9148),
    }
    public enum DebugType : int {
        DebugTypeError = ((int)0x824C),
        DebugTypeDeprecatedBehavior = ((int)0x824D),
        DebugTypeUndefinedBehavior = ((int)0x824E),
        DebugTypePortability = ((int)0x824F),
        DebugTypePerformance = ((int)0x8250),
        DebugTypeOther = ((int)0x8251),
        DebugTypeMarker = ((int)0x8268),
        DebugTypePushGroup = ((int)0x8269),
        DebugTypePopGroup = ((int)0x826A),
    }
    public enum DebugSource : int {
        DebugSourceApi = ((int)0x8246),
        DebugSourceWindowSystem = ((int)0x8247),
        DebugSourceShaderCompiler = ((int)0x8248),
        DebugSourceThirdParty = ((int)0x8249),
        DebugSourceApplication = ((int)0x824A),
        DebugSourceOther = ((int)0x824B),
    }

    public enum ShaderSubroutine : int {
        UniformSize = ((int)0x8A38),
        UniformNameLength = ((int)0x8A39),
        ActiveSubroutines = ((int)0x8DE5),
        ActiveSubroutineUniforms = ((int)0x8DE6),
        MaxSubroutines = ((int)0x8DE7),
        MaxSubroutineUniformLocations = ((int)0x8DE8),
        ActiveSubroutineUniformLocations = ((int)0x8E47),
        ActiveSubroutineMaxLength = ((int)0x8E48),
        ActiveSubroutineUniformMaxLength = ((int)0x8E49),
        NumCompatibleSubroutines = ((int)0x8E4A),
        CompatibleSubroutines = ((int)0x8E4B),
    }
}