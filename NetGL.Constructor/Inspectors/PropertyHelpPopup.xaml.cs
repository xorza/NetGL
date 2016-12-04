using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace NetGL.Constructor.Inspectors {
    public partial class PropertyHelpPopup : UserControl {
        public PropertyHelpPopup() {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
