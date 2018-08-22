namespace Gu.Wpf.Geometry.Demo.Windows
{
    using System.Windows;
    using System.Windows.Data;

    public partial class ZoomboxContentChanged : Window
    {
        public ZoomboxContentChanged()
        {
            this.InitializeComponent();
        }

        private void OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            this.Zoombox.ZoomUniform();
        }
    }
}
