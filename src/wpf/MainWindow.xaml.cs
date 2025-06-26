using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace MetricClock
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = null!;
        private bool isClickThrough = false;
        private HwndSource? hwndSource;
        private System.Windows.Forms.NotifyIcon? trayIcon;
        private System.Windows.Forms.Timer? singleClickTimer;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;
        const int HOTKEY_ID = 9000;
        const uint MOD_CONTROL = 0x0002;
        const uint MOD_SHIFT = 0x0004;
        const uint VK_C = 0x43;
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTBOTTOMRIGHT = 17;

        public MainWindow()
        {
            InitializeComponent();
            ClockSettings.Instance.LoadSettings();
            ClockSettings.Instance.PropertyChanged += Settings_PropertyChanged;
            RestoreWindowPosition();
            InitializeTrayIcon();
            InitializeTimer();
            InitializeResizeGrip();
            LoadClockFromSettings();
            UpdateOpacity();
            UpdateToggleButtons();
            
            // Set initial taskbar visibility
            ShowInTaskbar = !ClockSettings.Instance.HideFromTaskbarNormal;
            
            // Restore click-through state
            isClickThrough = ClockSettings.Instance.IsClickThrough;
            if (isClickThrough)
            {
                // SetClickThrough will be called after the window is loaded
                Loaded += (s, e) => SetClickThrough(isClickThrough);
            }
            
            // Handle title bar opacity slider changes
            TitleBarOpacitySlider.ValueChanged += (s, e) =>
            {
                UpdateWindowOpacity();
                ClockSettings.Instance.SaveSettings();
            };
            
            // Handle backdrop opacity slider changes
            TitleBarBackdropSlider.ValueChanged += (s, e) =>
            {
                UpdateBackdropOpacity();
                ClockSettings.Instance.SaveSettings();
            };
            
            // Handle clock elements opacity slider changes
            TitleBarElementsSlider.ValueChanged += (s, e) =>
            {
                RefreshCurrentClock();
                ClockSettings.Instance.SaveSettings();
            };
        }
        
        private void RestoreWindowPosition()
        {
            if (ClockSettings.Instance.WindowMaximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = ClockSettings.Instance.WindowLeft;
                Top = ClockSettings.Instance.WindowTop;
                Width = ClockSettings.Instance.WindowWidth;
                Height = ClockSettings.Instance.WindowHeight;
            }
        }
        
        private void InitializeResizeGrip()
        {
            ResizeGrip.MouseLeftButtonDown += ResizeGrip_MouseLeftButtonDown;
        }
        
        private void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SendMessage(hwnd, WM_NCLBUTTONDOWN, new IntPtr(HTBOTTOMRIGHT), IntPtr.Zero);
            }
        }

        public void LoadClockFromSettings()
        {
            LoadClock(ClockSettings.Instance.GetEffectiveClockType());
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClockSettings.Opacity))
            {
                UpdateWindowOpacity();
            }
            else if (e.PropertyName == nameof(ClockSettings.BackdropOpacity))
            {
                UpdateBackdropOpacity();
            }
            else if (e.PropertyName == nameof(ClockSettings.TimeSystem) || 
                     e.PropertyName == nameof(ClockSettings.TimeFormat) ||
                     e.PropertyName == nameof(ClockSettings.ClockDisplayType) ||
                     e.PropertyName == nameof(ClockSettings.ShowSecondsHand) ||
                     e.PropertyName == nameof(ClockSettings.AnalogStyle) ||
                     e.PropertyName == nameof(ClockSettings.ShowNumbersInCircles) ||
                     e.PropertyName == nameof(ClockSettings.ShowDate))
            {
                UpdateToggleButtons();
            }
            else if (e.PropertyName == nameof(ClockSettings.SmoothSecondsHand) ||
                     e.PropertyName == nameof(ClockSettings.UpdatesPerSecond))
            {
                UpdateTimerInterval();
            }
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            UpdateTimerInterval();
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        
        private void UpdateTimerInterval()
        {
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                // Use the updates per second setting for smooth animation
                double milliseconds = 1000.0 / ClockSettings.Instance.UpdatesPerSecond;
                timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            }
            else
            {
                // Default to 1000ms (1 update per second) for non-smooth animation
                timer.Interval = TimeSpan.FromMilliseconds(1000);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Check analog container first
            if (AnalogClockContainer.Children.Count > 0 && AnalogClockContainer.Children[0] is IClockView analogClockView)
            {
                analogClockView.UpdateTime();
            }
            // Then check digital container
            else if (DigitalContainer.Children.Count > 0 && DigitalContainer.Children[0] is IClockView digitalClockView)
            {
                digitalClockView.UpdateTime();
            }
        }

        private void LoadClock(ClockType type)
        {
            // Clear both containers
            AnalogClockContainer.Children.Clear();
            DigitalContainer.Children.Clear();

            UIElement clockControl = type switch
            {
                ClockType.Analog => new AnalogClock(),
                ClockType.Analog24 => new Analog24Clock(),
                ClockType.Digital => new DigitalClock(),
                ClockType.Digital24 => new Digital24Clock(),
                ClockType.Metric => new MetricClockView(),
                ClockType.AnalogMetric => new AnalogMetricClock(),
                _ => new AnalogClock()
            };

            // Add to appropriate container based on type
            bool isDigital = type == ClockType.Digital || type == ClockType.Digital24 || type == ClockType.Metric;
            
            if (isDigital)
            {
                AnalogClockContainer.Visibility = Visibility.Collapsed;
                DigitalViewbox.Visibility = Visibility.Visible;
                DigitalContainer.Children.Add(clockControl);
            }
            else
            {
                AnalogClockContainer.Visibility = Visibility.Visible;
                DigitalViewbox.Visibility = Visibility.Collapsed;
                AnalogClockContainer.Children.Add(clockControl);
            }
        }


        private void ControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
                DragMove();
        }

        private void ToggleClickThrough_Click(object sender, RoutedEventArgs e)
        {
            isClickThrough = !isClickThrough;
            SetClickThrough(isClickThrough);
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = LoadAppIcon(),
                Text = "HudClock - Click to toggle click-through",
                Visible = ClockSettings.Instance.ShowTrayIconNormal
            };
            
            trayIcon.MouseClick += TrayIcon_MouseClick;
            trayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, (s, e) => Settings_Click(s!, new RoutedEventArgs()));
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => Close());
            trayIcon.ContextMenuStrip = contextMenu;
        }
        
        private Icon LoadAppIcon()
        {
            try
            {
                // Try to extract icon from the current executable
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    var extractedIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                    if (extractedIcon != null)
                    {
                        return extractedIcon;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to extract icon from exe: {ex.Message}");
            }
            
            try
            {
                // Try to load the app icon from resources
                var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/HudClock.ico"))?.Stream;
                if (iconStream != null)
                {
                    return new Icon(iconStream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load app icon from resources: {ex.Message}");
            }
            
            // Fallback to creating a simple icon
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.Transparent);
                g.DrawEllipse(Pens.White, 1, 1, 14, 14);
                g.DrawLine(Pens.White, 8, 8, 8, 4); // Hour hand
                g.DrawLine(Pens.White, 8, 8, 12, 8); // Minute hand
            }
            return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        }
        
        private void TrayIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Start a timer to detect if this is a single click or part of a double-click
                if (singleClickTimer == null)
                {
                    singleClickTimer = new System.Windows.Forms.Timer();
                    singleClickTimer.Interval = System.Windows.Forms.SystemInformation.DoubleClickTime;
                    singleClickTimer.Tick += (s, args) =>
                    {
                        singleClickTimer.Stop();
                        singleClickTimer.Dispose();
                        singleClickTimer = null;
                        
                        // This is a single click - toggle click-through
                        isClickThrough = !isClickThrough;
                        SetClickThrough(isClickThrough);
                    };
                }
                singleClickTimer.Start();
            }
        }
        
        private void TrayIcon_MouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Cancel the single click timer
            if (singleClickTimer != null)
            {
                singleClickTimer.Stop();
                singleClickTimer.Dispose();
                singleClickTimer = null;
            }
            
            // Open settings on double-click
            Settings_Click(this, new RoutedEventArgs());
        }

        private void SetClickThrough(bool clickThrough)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            if (clickThrough)
            {
                // Show notice if enabled
                if (ClockSettings.Instance.ShowClickThroughNotice)
                {
                    var noticeWindow = new ClickThroughNoticeWindow
                    {
                        Owner = this
                    };
                    noticeWindow.ShowDialog();
                }
                
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                // Hide control bar and resize grip in click-through mode
                ControlBar.Visibility = Visibility.Collapsed;
                ResizeGrip.Visibility = Visibility.Collapsed;
                ShowInTaskbar = !ClockSettings.Instance.HideFromTaskbarClickThrough;
                trayIcon!.Text = "HudClock - Click-through enabled";
                trayIcon!.Visible = ClockSettings.Instance.ShowTrayIconClickThrough;
            }
            else
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
                ControlBar.Visibility = Visibility.Visible;
                ResizeGrip.Visibility = Visibility.Visible;
                ShowInTaskbar = !ClockSettings.Instance.HideFromTaskbarNormal;
                trayIcon!.Text = "HudClock - Click to toggle click-through";
                trayIcon!.Visible = ClockSettings.Instance.ShowTrayIconNormal;
            }
            
            // Apply both opacity settings
            UpdateWindowOpacity();
            UpdateBackdropOpacity();
            
            // Save the click-through state
            ClockSettings.Instance.IsClickThrough = clickThrough;
            ClockSettings.Instance.SaveSettings();
        }

        public void UpdateWindowOpacity()
        {
            // Set only the main content opacity, not the control bar
            MainContent.Opacity = ClockSettings.Instance.Opacity;
        }
        
        public void UpdateBackdropOpacity()
        {
            // Set the dark backdrop rectangle opacity
            DarkBackdrop.Opacity = ClockSettings.Instance.BackdropOpacity;
        }
        
        // Legacy method for compatibility
        public void UpdateOpacity()
        {
            UpdateWindowOpacity();
            UpdateBackdropOpacity();
        }

        public void UpdateTaskbarVisibility()
        {
            if (isClickThrough)
            {
                ShowInTaskbar = !ClockSettings.Instance.HideFromTaskbarClickThrough;
            }
            else
            {
                ShowInTaskbar = !ClockSettings.Instance.HideFromTaskbarNormal;
            }
        }

        public void UpdateTrayIconVisibility()
        {
            if (trayIcon != null)
            {
                if (isClickThrough)
                {
                    trayIcon.Visible = ClockSettings.Instance.ShowTrayIconClickThrough;
                }
                else
                {
                    trayIcon.Visible = ClockSettings.Instance.ShowTrayIconNormal;
                }
            }
        }

        public void RefreshCurrentClock()
        {
            UIElement? currentClock = null;
            
            // Check which container has the clock
            if (AnalogClockContainer.Children.Count > 0)
            {
                currentClock = AnalogClockContainer.Children[0];
            }
            else if (DigitalContainer.Children.Count > 0)
            {
                currentClock = DigitalContainer.Children[0];
            }
            
            if (currentClock != null)
            {
                // Call RefreshStyle method if it exists
                var refreshMethod = currentClock.GetType().GetMethod("RefreshStyle");
                if (refreshMethod != null)
                {
                    refreshMethod.Invoke(currentClock, null);
                }
                else
                {
                    // Fallback: reload the clock
                    LoadClockFromSettings();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private SettingsWindow? settingsWindow;

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null || !settingsWindow.IsLoaded)
            {
                settingsWindow = new SettingsWindow(this)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                settingsWindow.Closed += (s, args) => settingsWindow = null;
            }
            
            settingsWindow.Show();
            settingsWindow.Activate();
        }
        
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeButton.Content = "🔳";
                MaximizeButton.ToolTip = "Maximize";
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaximizeButton.Content = "🔲";
                MaximizeButton.ToolTip = "Restore";
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED);

            hwndSource = HwndSource.FromHwnd(hwnd);
            hwndSource.AddHook(HwndHook);
            RegisterHotKey(hwnd, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_C);
        }
        
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (WindowState == WindowState.Normal)
            {
                SaveWindowPosition();
            }
        }
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (WindowState == WindowState.Normal)
            {
                SaveWindowPosition();
            }
        }
        
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            ClockSettings.Instance.WindowMaximized = (WindowState == WindowState.Maximized);
            ClockSettings.Instance.SaveSettings();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                isClickThrough = !isClickThrough;
                SetClickThrough(isClickThrough);
                handled = true;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveWindowPosition();
            
            if (hwndSource != null)
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                UnregisterHotKey(hwnd, HOTKEY_ID);
                hwndSource.RemoveHook(HwndHook);
                hwndSource.Dispose();
            }
            
            if (trayIcon != null)
            {
                trayIcon.Dispose();
            }
            
            if (singleClickTimer != null)
            {
                singleClickTimer.Stop();
                singleClickTimer.Dispose();
            }
            
            ClockSettings.Instance.PropertyChanged -= Settings_PropertyChanged;
            base.OnClosed(e);
        }
        
        private void SaveWindowPosition()
        {
            if (WindowState == WindowState.Normal)
            {
                ClockSettings.Instance.WindowLeft = Left;
                ClockSettings.Instance.WindowTop = Top;
                ClockSettings.Instance.WindowWidth = Width;
                ClockSettings.Instance.WindowHeight = Height;
                ClockSettings.Instance.SaveSettings();
            }
        }

        private void UpdateToggleButtons()
        {
            // Update time toggle button
            if (ClockSettings.Instance.TimeSystem == TimeSystem.Metric)
            {
                TimeToggleButton.Content = "10";
            }
            else if (ClockSettings.Instance.TimeFormat == TimeFormat.TwentyFourHour)
            {
                TimeToggleButton.Content = "24";
            }
            else
            {
                TimeToggleButton.Content = "12";
            }

            // Update display toggle button
            DisplayToggleButton.Content = ClockSettings.Instance.ClockDisplayType == ClockDisplayType.Analog ? "A" : "D";
            
            // Update seconds toggle button (filled vs outline)
            SecondsToggleButton.Background = ClockSettings.Instance.ShowSecondsHand 
                ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                : System.Windows.Media.Brushes.Transparent;
            
            // Show/hide analog-only buttons
            bool isAnalog = ClockSettings.Instance.ClockDisplayType == ClockDisplayType.Analog;
            HandsToggleButton.Visibility = isAnalog ? Visibility.Visible : Visibility.Collapsed;
            CirclesToggleButton.Visibility = isAnalog ? Visibility.Visible : Visibility.Collapsed;
            
            // Update hands toggle button state
            if (isAnalog)
            {
                var style = ClockSettings.Instance.AnalogStyle;
                HandsToggleButton.Background = (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
                    ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                    : System.Windows.Media.Brushes.Transparent;
                
                // Update circles toggle button state
                CirclesToggleButton.Background = (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
                    ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                    : System.Windows.Media.Brushes.Transparent;
                
                // Show numbers toggle only when circles are visible
                bool showCircles = style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands;
                NumbersToggleButton.Visibility = showCircles ? Visibility.Visible : Visibility.Collapsed;
                
                // Update numbers toggle button state
                if (showCircles)
                {
                    NumbersToggleButton.Background = ClockSettings.Instance.ShowNumbersInCircles
                        ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                        : System.Windows.Media.Brushes.Transparent;
                }
            }
            else
            {
                NumbersToggleButton.Visibility = Visibility.Collapsed;
            }
            
            // Update date toggle button
            DateToggleButton.Background = ClockSettings.Instance.ShowDate
                ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                : System.Windows.Media.Brushes.Transparent;
                
            // Update minutes toggle button
            MinutesToggleButton.Background = ClockSettings.Instance.ShowMinutes
                ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102))
                : System.Windows.Media.Brushes.Transparent;
        }

        private void TimeToggle_Click(object sender, RoutedEventArgs e)
        {
            // Cycle through: 12-hour -> 24-hour -> Metric -> 12-hour
            if (ClockSettings.Instance.TimeSystem == TimeSystem.Metric)
            {
                // Metric -> 12-hour standard
                ClockSettings.Instance.TimeSystem = TimeSystem.Standard;
                ClockSettings.Instance.TimeFormat = TimeFormat.TwelveHour;
            }
            else if (ClockSettings.Instance.TimeFormat == TimeFormat.TwelveHour)
            {
                // 12-hour -> 24-hour
                ClockSettings.Instance.TimeFormat = TimeFormat.TwentyFourHour;
            }
            else
            {
                // 24-hour -> Metric
                ClockSettings.Instance.TimeSystem = TimeSystem.Metric;
            }

            UpdateToggleButtons();
            LoadClockFromSettings();
            ClockSettings.Instance.SaveSettings();
        }

        private void DisplayToggle_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between analog and digital
            ClockSettings.Instance.ClockDisplayType = 
                ClockSettings.Instance.ClockDisplayType == ClockDisplayType.Analog 
                    ? ClockDisplayType.Digital 
                    : ClockDisplayType.Analog;

            UpdateToggleButtons();
            LoadClockFromSettings();
            ClockSettings.Instance.SaveSettings();
        }

        private void SecondsToggle_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.ShowSecondsHand = !ClockSettings.Instance.ShowSecondsHand;
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void HandsToggle_Click(object sender, RoutedEventArgs e)
        {
            var currentStyle = ClockSettings.Instance.AnalogStyle;
            
            // Toggle hands on/off while preserving circles state
            if (currentStyle == AnalogClockStyle.Traditional)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Circles;
            }
            else if (currentStyle == AnalogClockStyle.Circles)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Traditional;
            }
            else if (currentStyle == AnalogClockStyle.CirclesWithHands)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Circles;
            }
            
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void CirclesToggle_Click(object sender, RoutedEventArgs e)
        {
            var currentStyle = ClockSettings.Instance.AnalogStyle;
            
            // Toggle circles on/off while preserving hands state
            if (currentStyle == AnalogClockStyle.Traditional)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.CirclesWithHands;
            }
            else if (currentStyle == AnalogClockStyle.CirclesWithHands)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Traditional;
            }
            else if (currentStyle == AnalogClockStyle.Circles)
            {
                ClockSettings.Instance.AnalogStyle = AnalogClockStyle.Traditional;
            }
            
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void NumbersToggle_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.ShowNumbersInCircles = !ClockSettings.Instance.ShowNumbersInCircles;
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void DateToggle_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.ShowDate = !ClockSettings.Instance.ShowDate;
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void TitleBarScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }

        private void MinutesToggle_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Instance.ShowMinutes = !ClockSettings.Instance.ShowMinutes;
            UpdateToggleButtons();
            RefreshCurrentClock();
            ClockSettings.Instance.SaveSettings();
        }
    }

    public enum ClockType
    {
        Analog,
        Digital,
        Digital24,
        Metric,
        AnalogMetric,
        Analog24
    }

    public interface IClockView
    {
        void UpdateTime();
    }
}