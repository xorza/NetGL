using System.Windows;
using System.Windows.Input;
using NetGL.Constructor.Inspectors;
using NetGL.Constructor.UI;

namespace NetGL.Constructor.Infrastructure {
    public static class PropertyTooltipBehaviour {
        public static void SetShowTooltipViewModel(DependencyObject d, bool value) {
            d.SetValue(ShowTooltipViewModelProperty, value);
        }
        public static bool GetShowTooltipViewModel(DependencyObject d) {
            return (bool)d.GetValue(ShowTooltipViewModelProperty);
        }

        public static readonly DependencyProperty ShowTooltipViewModelProperty =
            DependencyProperty.RegisterAttached(
            "ShowTooltipViewModel",
            typeof(bool),
            typeof(PropertyTooltipBehaviour),
            new UIPropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var element = (FrameworkElement)d;
            element.MouseRightButtonUp -= Element_MouseRightButtonUp;

            var value = (bool)e.NewValue;
            var viewModel = element.ToolTip as PropertyTooltipViewModel;

            if (viewModel == null)
                return;
            if (value == false)
                return;

            Add(element, viewModel);
        }


        public static void Add(FrameworkElement element, PropertyTooltipViewModel viewModel) {
            Assert.NotNull(viewModel);
            Assert.NotNull(element);

            var toolTipControl = new PropertyToolTip();
            toolTipControl.DataContext = viewModel;
            element.ToolTip = toolTipControl;

            if (viewModel.OnlineHelpURL == null && viewModel.Description == null)
                return;

            element.MouseRightButtonUp += Element_MouseRightButtonUp;
        }

        private static void Element_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            var element = (FrameworkElement)sender;

            var popupControl = new PropertyHelpPopup();
            popupControl.DataContext = ((FrameworkElement)element.ToolTip).DataContext;

            var popup = new RegularPopup(popupControl);
            popup.StaysOpen = false;
            popup.IsOpen = true;
        }
    }
}
