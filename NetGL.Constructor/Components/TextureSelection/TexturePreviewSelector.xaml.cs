using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NetGL.Constructor.Infrastructure;
using NetGL.Constructor.Scene;
using NetGL.Core.Types;

namespace NetGL.Constructor.Components.TextureSelection {
    public partial class TexturePreviewSelector : UserControl, IDisposable {
        private readonly ObservableCollection<TextureViewModel> _textures = new ObservableCollection<TextureViewModel>();

        public event EventHandler<EventArgs<Texture2>> TextureSelected;
        public event EventHandler Canceled;

        public TexturePreviewSelector() {
            InitializeComponent();

            TexturesListBox.ItemsSource = _textures;

            var textures = SceneViewModel.Instance.TextureFactory
                .GetCreatedTextures()
                .Select(_ => new TextureViewModel(_))
                .ForEach(_ => _.TextureDisposed += TextureDisposedHandler);
            textures.ForEach(_ => _textures.Add(_));
        }

        public void Dispose() { }

        private void TextureDisposedHandler(object sender, EventArgs ea) {
            _textures.Remove((TextureViewModel)sender);
        }

        private void Ok_Click(object sender, RoutedEventArgs e) {
            var textureViewModel = (TextureViewModel)TexturesListBox.SelectedItem;
            if (textureViewModel == null)
                return;

            if (TextureSelected != null)
                TextureSelected(this, new EventArgs<Texture2>(textureViewModel.Texture));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            if (Canceled != null)
                Canceled(this, EventArgs.Empty);
        }

        private void New_Click(object sender, RoutedEventArgs e) {
            var openFileDialog = FileDialogHelper.CreateOpenImageDialog();

            if (openFileDialog.ShowDialog(this.GetWindow()) != true)
                return;

            var texture = new Texture2(openFileDialog.FileName, false);
            var viewModel = new TextureViewModel(texture);
            viewModel.TextureDisposed += TextureDisposedHandler;
            _textures.Add(viewModel);
        }
    }
}