using System.Windows;
using System.Windows.Navigation;

namespace MetricClock
{
    public partial class SettingsWindow : Window
    {
        private MainWindow? mainWindow;

        public SettingsWindow(MainWindow parent)
        {
            InitializeComponent();
            mainWindow = parent;
            DataContext = ClockSettings.Instance;
            
            // Set the settings path text
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = System.IO.Path.Combine(appDataPath, "HudClock");
            SettingsPathText.Text = settingsDir;
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainWindow?.UpdateWindowOpacity();
            ClockSettings.Instance.SaveSettings();
        }

        private void ClockElementOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void BackdropOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainWindow?.UpdateBackdropOpacity();
            ClockSettings.Instance.SaveSettings();
        }

        private void ClockDisplayType_Changed(object sender, RoutedEventArgs e)
        {
            if (AnalogRadio.IsChecked == true)
                ClockSettings.Instance.ClockDisplayType = ClockDisplayType.Analog;
            else if (DigitalRadio.IsChecked == true)
                ClockSettings.Instance.ClockDisplayType = ClockDisplayType.Digital;

            mainWindow?.LoadClockFromSettings();
            ClockSettings.Instance.SaveSettings();
        }

        private void TimeFormat_Changed(object sender, RoutedEventArgs e)
        {
            if (TwelveHourRadio.IsChecked == true)
                ClockSettings.Instance.TimeFormat = TimeFormat.TwelveHour;
            else if (TwentyFourHourRadio.IsChecked == true)
                ClockSettings.Instance.TimeFormat = TimeFormat.TwentyFourHour;

            mainWindow?.LoadClockFromSettings();
            ClockSettings.Instance.SaveSettings();
        }

        private void TimeSystem_Changed(object sender, RoutedEventArgs e)
        {
            if (StandardRadio.IsChecked == true)
                ClockSettings.Instance.TimeSystem = TimeSystem.Standard;
            else if (MetricRadio.IsChecked == true)
                ClockSettings.Instance.TimeSystem = TimeSystem.Metric;

            mainWindow?.LoadClockFromSettings();
            ClockSettings.Instance.SaveSettings();
        }

        private void FontFamily_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }
        
        private void FontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }
        
        private void FontWeight_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void AnalogStyle_Changed(object sender, RoutedEventArgs e)
        {
            if (TraditionalRadio.IsChecked == true)
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Traditional;
            else if (CirclesRadio.IsChecked == true)
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Circles;
            else if (CirclesWithHandsRadio.IsChecked == true)
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.CirclesWithHands;

            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ShowSeconds_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ShowNumbersInCircles_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ShowCenterDot_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ClockShape_Changed(object sender, RoutedEventArgs e)
        {
            if (CircleShapeRadio.IsChecked == true)
                ClockSettings.Instance.ClockShape = ClockShape.Circle;
            else if (ProjectedRectangleRadio.IsChecked == true)
                ClockSettings.Instance.ClockShape = ClockShape.ProjectedRectangle;
            else if (RectanglePathRadio.IsChecked == true)
                ClockSettings.Instance.ClockShape = ClockShape.RectanglePath;

            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ShowNormalTime_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ShowDate_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }
        
        private void ShowMinutes_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void SmoothSeconds_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void ElementScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainWindow?.RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }
        
        private void UpdatesPerSecond_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ClockSettings.Instance.SaveSettings();
        }

        private void TaskbarVisibility_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.UpdateTaskbarVisibility();
            ClockSettings.Instance.SaveSettings();
        }

        private void TrayIconVisibility_Changed(object sender, RoutedEventArgs e)
        {
            mainWindow?.UpdateTrayIconVisibility();
            ClockSettings.Instance.SaveSettings();
        }

        private void ClickThroughNotice_Changed(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.SaveSettings();
        }

        private void ResetDefaults_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.Opacity = 0.8;
            ClockSettings.Instance.BackdropOpacity = 0.8;
            ClockSettings.Instance.ClockElementOpacity = 1.0;
            ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Traditional;
            ClockSettings.Instance.ClockShape = ClockShape.Circle;
            ClockSettings.Instance.ShowSecondsHand = true;
            ClockSettings.Instance.ShowNumbersInCircles = false;
            ClockSettings.Instance.ShowNormalTimeInMetric = true;
            ClockSettings.Instance.ShowDate = false;
            ClockSettings.Instance.ShowMinutes = true;
            ClockSettings.Instance.ShowCenterDot = false;
            ClockSettings.Instance.ClockDisplayType = ClockDisplayType.Analog;
            ClockSettings.Instance.TimeFormat = TimeFormat.TwelveHour;
            ClockSettings.Instance.TimeSystem = TimeSystem.Standard;
            ClockSettings.Instance.HideFromTaskbarNormal = false;
            ClockSettings.Instance.HideFromTaskbarClickThrough = true;
            ClockSettings.Instance.ShowTrayIconNormal = true;
            ClockSettings.Instance.ShowTrayIconClickThrough = true;
            ClockSettings.Instance.ShowClickThroughNotice = true;
            ClockSettings.Instance.ElementScale = 1.0;
            ClockSettings.Instance.UpdatesPerSecond = 60;
            ClockSettings.Instance.DigitalFontFamily = "Segoe UI";
            ClockSettings.Instance.DigitalFontSize = 48;
            ClockSettings.Instance.DigitalFontWeight = "Normal";
            
            mainWindow?.UpdateOpacity();
            mainWindow?.LoadClockFromSettings();
            mainWindow?.UpdateTaskbarVisibility();
            mainWindow?.UpdateTrayIconVisibility();
            ClockSettings.Instance.SaveSettings();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void OpenSettingsDirectory_Click(object sender, RoutedEventArgs e)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = System.IO.Path.Combine(appDataPath, "HudClock");
            
            // Create directory if it doesn't exist
            System.IO.Directory.CreateDirectory(settingsDir);
            
            // Open the directory in Windows Explorer
            System.Diagnostics.Process.Start("explorer.exe", settingsDir);
        }
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Open the URL in the default browser
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}