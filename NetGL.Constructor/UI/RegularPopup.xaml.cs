using System.Windows.Controls.Primitives;

namespace NetGL.Constructor.UI {
    public partial class RegularPopup : Popup {
        public new object Child {
            get {
                return MainContentControl.Content;
            }
            set {
                if (MainContentControl.Content == value)
                    return;

                MainContentControl.Content = value;
            }
        }        

        public RegularPopup() {
            InitializeComponent();

            Placement = PlacementMode.MousePoint;
            StaysOpen = false;
        }
        public RegularPopup(object content)
            : this() {
            Child = content;
            IsOpen = true;
        }

        public void Show() {
            IsOpen = true;
        }
        public void Close() {
            IsOpen = false;
        }
    }
}
