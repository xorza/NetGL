using System;
using System.Drawing;
using System.Windows.Input;
using NetGL.Constructor.Infrastructure;
using NetGL.Core.Types;

namespace NetGL.Constructor.Components.TextureSelection {
    public class TextureViewModel : NotifyPropertyChange {
        public string Name { get; private set; }
        public Texture2 Texture { get; private set; }
        public Bitmap Image { get; private set; }

        public event EventHandler TextureDisposed;

        public ICommand DisposeCommand {
            get {
                return new Command(() => {
                    Texture.Dispose();
                    if (TextureDisposed != null)
                        TextureDisposed(this, EventArgs.Empty);
                });
            }
        }

        public TextureViewModel(Texture2 texture) {
            Assert.NotNull(texture);

            this.Name = texture.Name;
            this.Texture = texture;
            this.Image = texture.Image;
        }
    }
}
