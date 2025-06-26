using System.Windows;

namespace MetricClock
{
    public partial class ClickThroughNoticeWindow : Window
    {
        public ClickThroughNoticeWindow()
        {
            InitializeComponent();
            AlwaysShowCheckBox.IsChecked = ClockSettings.Instance.ShowClickThroughNotice;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Update the setting based on checkbox state
            ClockSettings.Instance.ShowClickThroughNotice = AlwaysShowCheckBox.IsChecked ?? true;
            ClockSettings.Instance.SaveSettings();
            
            DialogResult = true;
            Close();
        }
    }
}