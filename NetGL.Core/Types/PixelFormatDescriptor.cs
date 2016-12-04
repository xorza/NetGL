using System;
using System.Runtime.InteropServices;

namespace NetGL.Core.Types {
    public enum LayerType : sbyte {
        PFD_MAIN_PLANE = 0,
        PFD_OVERLAY_PLANE = 1,
        PFD_UNDERLAY_PLANE = -1
    }
    public enum PixelFormaDescriptorPixelType : byte {
        PFD_TYPE_RGBA = 0,
        PFD_TYPE_COLORINDEX = 1
    }
    [Flags]
    public enum DrawWindowFlag : uint {
        PFD_DOUBLEBUFFER = 0x00000001,
        PFD_STEREO = 0x00000002,
        PFD_DRAW_TO_WINDOW = 0x00000004,
        PFD_DRAW_TO_BITMAP = 0x00000008,
        PFD_SUPPORT_GDI = 0x00000010,
        PFD_SUPPORT_OPENGL = 0x00000020,
        PFD_GENERIC_FORMAT = 0x00000040,
        PFD_NEED_PALETTE = 0x00000080,
        PFD_NEED_SYSTEM_PALETTE = 0x00000100,
        PFD_SWAP_EXCHANGE = 0x00000200,
        PFD_SWAP_COPY = 0x00000400,
        PFD_SWAP_LAYER_BUFFERS = 0x00000800,
        PFD_GENERIC_ACCELERATED = 0x00001000,
        PFD_SUPPORT_DIRECTDRAW = 0x00002000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PixelFormatDescriptor {
        public ushort
            Size,
            Version;

        public DrawWindowFlag
            Flags;

        public uint
            LayerMask,
            VisibleMask,
            DamageMask;

        public PixelFormaDescriptorPixelType
            PixelType;

        public byte
            ColorBits,
            RedBits,
            RedShift,
            GreenBits,
            GreenShift,
            BlueBits,
            BlueShift,
            AlphaBits,
            AlphaShift,
            AccumBits,
            AccumRedBits,
            AccumGreenBits,
            AccumBlueBits,
            AccumAlphaBits,
            DepthBits,
            StencilBits,
            AuxBuffers,
            Reserved;

        public LayerType
            LayerType;

        public static PixelFormatDescriptor CreateDefault() {
            var pfd = new PixelFormatDescriptor();
            pfd.Size = (ushort)Marshal.SizeOf(pfd);
            pfd.Version = 1;
            pfd.Flags = (DrawWindowFlag.PFD_DRAW_TO_WINDOW | DrawWindowFlag.PFD_SUPPORT_OPENGL | DrawWindowFlag.PFD_DOUBLEBUFFER);
            pfd.PixelType = PixelFormaDescriptorPixelType.PFD_TYPE_RGBA;
            pfd.ColorBits = 24;
            pfd.AlphaBits = 8;
            pfd.LayerType = LayerType.PFD_MAIN_PLANE;
            pfd.DepthBits = 24;

            return pfd;
        }
    }
}