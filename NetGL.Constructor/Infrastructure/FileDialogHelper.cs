using Microsoft.Win32;

namespace NetGL.Constructor.Infrastructure {
    public static class FileDialogHelper {
        public static OpenFileDialog CreateOpenImageDialog() {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.tga;*.tiff;*.gif|AllFiles|*.*";
            return ofd;
        }
    }
}
