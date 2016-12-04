using System;
using System.Windows;
using System.Windows.Controls;

namespace NetGL.Constructor.UI
{
    public partial class ErrorControl : UserControl
    {
        public Exception Error { get; private set; }

        public event EventHandler OKButtonClick;

        public ErrorControl(Exception ex)
        {
            this.Error = ex;

            InitializeComponent();

            exceptionText.Text = ex.ToString();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (OKButtonClick != null)
                OKButtonClick(sender, e);
        }
    }
}
