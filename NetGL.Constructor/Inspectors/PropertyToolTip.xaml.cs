using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetGL.Constructor.Infrastructure;
using NetGL.Constructor.UI;

namespace NetGL.Constructor.Inspectors {
    public partial class PropertyToolTip : UserControl {
        public PropertyToolTip() {
            InitializeComponent();
        }

        public static void Add(FrameworkElement element, PropertyTooltipViewModel viewModel) {
            Assert.NotNull(viewModel);
            Assert.NotNull(element);

            var toolTipControl = new PropertyToolTip();
            toolTipControl.DataContext = viewModel;

            element.ToolTip = toolTipControl;

            element.MouseRightButtonUp -= Element_MouseRightButtonUp;
            if (viewModel.OnlineHelpURL == null && viewModel.Description == null)
                return;

            element.MouseRightButtonUp += Element_MouseRightButtonUp;
        }

        private static void Element_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            var element = (FrameworkElement)sender;

            var popupControl = new PropertyHelpPopup();
            popupControl.DataContext = element.DataContext;

            var popup = new RegularPopup(popupControl);
            popup.StaysOpen = false;
            popup.IsOpen = true;
        }
    }
}
