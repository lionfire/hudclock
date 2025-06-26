using System.Windows.Controls;
using System.Windows.Media;

namespace MetricClock
{
    public partial class DigitalClock : System.Windows.Controls.UserControl, IClockView
    {
        private StackPanel? sevenSegmentPanel;

        public DigitalClock()
        {
            InitializeComponent();
            ApplyStyle();
            UpdateTime();
        }

        private void ApplyStyle()
        {
            double staticOpacity = ClockSettings.Instance.ClockElementOpacity;
            
            // For digital clocks, apply opacity to the background border only
            // Text content (time/date) stays at full opacity since it's the main content
            var border = TimeDisplay.Parent as Border;
            if (border != null)
            {
                border.Opacity = staticOpacity;
            }
            
            TimeDisplay.Opacity = 1.0;  // Keep time text fully visible
            DateDisplay.Opacity = 1.0;  // Keep date text fully visible
            
            // Apply font settings
            TimeDisplay.FontFamily = new System.Windows.Media.FontFamily(ClockSettings.Instance.DigitalFontFamily);
            TimeDisplay.FontSize = ClockSettings.Instance.DigitalFontSize;
            TimeDisplay.FontWeight = GetFontWeight(ClockSettings.Instance.DigitalFontWeight);
            
            // Apply proportional font size to date display
            DateDisplay.FontFamily = new System.Windows.Media.FontFamily(ClockSettings.Instance.DigitalFontFamily);
            DateDisplay.FontSize = ClockSettings.Instance.DigitalFontSize * 0.4; // 40% of main font size
            DateDisplay.FontWeight = GetFontWeight(ClockSettings.Instance.DigitalFontWeight);
            
            if (ClockSettings.Instance.DigitalStyle == DigitalClockStyle.SevenSegment)
            {
                TimeDisplay.Visibility = System.Windows.Visibility.Collapsed;
                
                if (sevenSegmentPanel == null)
                {
                    sevenSegmentPanel = new StackPanel
                    {
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Opacity = staticOpacity
                    };
                    
                    Grid parent = (Grid)TimeDisplay.Parent;
                    parent.Children.Add(sevenSegmentPanel);
                }
                else
                {
                    sevenSegmentPanel.Opacity = staticOpacity;
                }
                
                sevenSegmentPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TimeDisplay.Visibility = System.Windows.Visibility.Visible;
                if (sevenSegmentPanel != null)
                    sevenSegmentPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void UpdateTime()
        {
            DateTime now = DateTime.Now;
            
            if (ClockSettings.Instance.DigitalStyle == DigitalClockStyle.SevenSegment)
            {
                UpdateSevenSegmentDisplay(now);
            }
            else
            {
                if (!ClockSettings.Instance.ShowMinutes)
                {
                    TimeDisplay.Text = now.ToString("h tt");
                }
                else
                {
                    TimeDisplay.Text = ClockSettings.Instance.ShowSecondsHand 
                        ? now.ToString("h:mm:ss tt")
                        : now.ToString("h:mm tt");
                }
            }
            
            if (ClockSettings.Instance.ShowDate)
            {
                DateDisplay.Text = now.ToString("dddd, MMMM d, yyyy");
                DateDisplay.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                DateDisplay.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void UpdateSevenSegmentDisplay(DateTime time)
        {
            if (sevenSegmentPanel == null) return;
            
            string timeString;
            if (!ClockSettings.Instance.ShowMinutes)
            {
                timeString = time.ToString("h tt");
            }
            else
            {
                timeString = ClockSettings.Instance.ShowSecondsHand 
                    ? time.ToString("h:mm:ss tt")
                    : time.ToString("h:mm tt");
            }
            sevenSegmentPanel.Children.Clear();
            
            foreach (char c in timeString)
            {
                if (c == ':')
                {
                    sevenSegmentPanel.Children.Add(new SevenSegmentColon());
                }
                else if (c == ' ')
                {
                    sevenSegmentPanel.Children.Add(new Canvas { Width = 20 });
                }
                else if (char.IsDigit(c))
                {
                    SevenSegmentDisplay digit = new SevenSegmentDisplay();
                    digit.SetDigit(c);
                    sevenSegmentPanel.Children.Add(digit);
                }
                else
                {
                    // AM/PM text
                    TextBlock ampm = new TextBlock
                    {
                        Text = c.ToString(),
                        Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0)),
                        FontSize = 20,
                        FontWeight = System.Windows.FontWeights.Bold,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new System.Windows.Thickness(2, 0, 2, 0)
                    };
                    sevenSegmentPanel.Children.Add(ampm);
                }
            }
        }

        public void RefreshStyle()
        {
            ApplyStyle();
            UpdateTime();
        }
        
        private System.Windows.FontWeight GetFontWeight(string weight)
        {
            return weight switch
            {
                "Thin" => System.Windows.FontWeights.Thin,
                "ExtraLight" => System.Windows.FontWeights.ExtraLight,
                "Light" => System.Windows.FontWeights.Light,
                "Normal" => System.Windows.FontWeights.Normal,
                "Medium" => System.Windows.FontWeights.Medium,
                "SemiBold" => System.Windows.FontWeights.SemiBold,
                "Bold" => System.Windows.FontWeights.Bold,
                "ExtraBold" => System.Windows.FontWeights.ExtraBold,
                "Black" => System.Windows.FontWeights.Black,
                _ => System.Windows.FontWeights.Normal
            };
        }
    }
}