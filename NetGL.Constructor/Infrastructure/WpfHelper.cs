using NetGL.Core.Infrastructure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NetGL.Constructor.Infrastructure {
    public static class WpfHelper {
        public static BitmapSource GetBitmapSource(this Bitmap bmp, BitmapSizeOptions sizeOptions) {
            var bmpHandle = IntPtr.Zero;

            try {
                bmpHandle = bmp.GetHbitmap();
                var source = Imaging.CreateBitmapSourceFromHBitmap(
                     bmpHandle,
                     IntPtr.Zero,
                     Int32Rect.Empty,
                     sizeOptions);

                return source;
            }
            catch (Exception ex) {
                Log.Exception(ex);
                return null;
            }
            finally {
                if (bmpHandle != IntPtr.Zero)
                    DeleteObject(bmpHandle);
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
