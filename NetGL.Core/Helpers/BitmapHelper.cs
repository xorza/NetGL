using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging {
    public static class BitmapHelper {
        public static void FlipVertically(this BitmapData data) {
            var height = data.Height;
            var stride = data.Stride;
            var ptr = data.Scan0;
            var lineTemp = Marshal.AllocHGlobal(data.Stride);

            for (int i = 0; i < height / 2; i++) {
                var row1 = ptr + stride * i;
                var row2 = ptr + stride * (height - 1 - i);

                CopyMemory(lineTemp, row1, stride);
                CopyMemory(row1, row2, stride);
                CopyMemory(row2, lineTemp, stride);
            }

            Marshal.FreeHGlobal(lineTemp);
        }

        public static void FlipGreen(this Bitmap bmp) {
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            try {
                FlipGreen(bitmapData);
            }
            finally {
                bmp.UnlockBits(bitmapData);
            }
        }
        public static unsafe void FlipGreen(this BitmapData data) {
            switch (data.PixelFormat) {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    if (BitConverter.IsLittleEndian)
                        for (int i = 0; i < data.Height; i++) {
                            var clr = (Color4LittleEndian*)(data.Scan0 + data.Stride * i);
                            for (int j = 0; j < data.Width; j++)
                                clr[j].G = (byte)(255 - clr[j].G);
                        }
                    else
                        throw new NotSupportedException("Bigendian");
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    if (BitConverter.IsLittleEndian)
                        for (int i = 0; i < data.Height; i++) {
                            var clr = (Color3LittleEndian*)(data.Scan0 + data.Stride * i);
                            for (int j = 0; j < data.Width; j++)
                                clr[j].G = (byte)(255 - clr[j].G);
                        }
                    else
                        throw new NotSupportedException("Bigendian");
                    break;
                default:
                    throw new NotSupportedException(data.PixelFormat.ToString());
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        private static void CopyMemory(IntPtr dest, IntPtr src, int count) {
            CopyMemory(dest, src, (uint)count);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Color3LittleEndian {
            [FieldOffset(0)]
            public byte R;
            [FieldOffset(1)]
            public byte G;
            [FieldOffset(2)]
            public byte B;
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct Color4LittleEndian {
            [FieldOffset(0)]
            public byte R;
            [FieldOffset(1)]
            public byte G;
            [FieldOffset(2)]
            public byte B;
            [FieldOffset(3)]
            public byte A;
        }
    }
}
