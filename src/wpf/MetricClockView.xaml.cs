using System.Windows.Controls;
using System.Windows.Media;

namespace MetricClock
{
    public partial class MetricClockView : System.Windows.Controls.UserControl, IClockView
    {
        private StackPanel? sevenSegmentPanel;

        public MetricClockView()
        {
            InitializeComponent();
            ApplyStyle();
            UpdateTime();
        }

        private void ApplyStyle()
        {
            double staticOpacity = ClockSettings.Instance.ClockElementOpacity;
            
            // For metric clock, apply opacity to the background border only
            // Time content stays at full opacity since it's the main content
            var border = MetricTimeDisplay.Parent as Border;
            if (border != null)
            {
                border.Opacity = staticOpacity;
            }
            
            // Keep all time-related text fully visible
            MetricTimeDisplay.Opacity = 1.0;
            StandardTimeDisplay.Opacity = 1.0;
            
            // Apply opacity only to static labels (like "Metric Time")
            var parentPanel = MetricTimeDisplay.Parent as StackPanel;
            if (parentPanel != null)
            {
                foreach (System.Windows.UIElement child in parentPanel.Children)
                {
                    if (child is TextBlock textBlock && textBlock.Text == "Metric Time")
                    {
                        textBlock.Opacity = staticOpacity;
                    }
                }
            }
            
            if (ClockSettings.Instance.DigitalStyle == DigitalClockStyle.SevenSegment)
            {
                MetricTimeDisplay.Visibility = System.Windows.Visibility.Collapsed;
                
                if (sevenSegmentPanel == null)
                {
                    sevenSegmentPanel = new StackPanel
                    {
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Opacity = staticOpacity
                    };
                    
                    Grid parent = (Grid)MetricTimeDisplay.Parent;
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
                MetricTimeDisplay.Visibility = System.Windows.Visibility.Visible;
                if (sevenSegmentPanel != null)
                    sevenSegmentPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void UpdateTime()
        {
            DateTime now = DateTime.Now;
            MetricTime metricTime = TimeHelpers.GetMetricTime(now);
            
            if (ClockSettings.Instance.DigitalStyle == DigitalClockStyle.SevenSegment)
            {
                UpdateSevenSegmentDisplay(metricTime);
            }
            else
            {
                if (!ClockSettings.Instance.ShowMinutes)
                {
                    MetricTimeDisplay.Text = metricTime.Hours.ToString();
                }
                else
                {
                    MetricTimeDisplay.Text = ClockSettings.Instance.ShowSecondsHand 
                        ? metricTime.ToString()
                        : metricTime.ToStringNoSeconds();
                }
            }
            
            // Show/hide normal time based on setting
            if (ClockSettings.Instance.ShowNormalTimeInMetric)
            {
                string timeFormat;
                if (!ClockSettings.Instance.ShowMinutes)
                {
                    timeFormat = $"({now:HH})";
                }
                else
                {
                    timeFormat = ClockSettings.Instance.ShowSecondsHand 
                        ? $"({now:HH:mm:ss})"
                        : $"({now:HH:mm})";
                }
                StandardTimeDisplay.Text = timeFormat;
                StandardTimeDisplay.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StandardTimeDisplay.Visibility = System.Windows.Visibility.Collapsed;
            }
            
            // Show/hide date based on setting
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

        private void UpdateSevenSegmentDisplay(MetricTime metricTime)
        {
            if (sevenSegmentPanel == null) return;
            
            string timeString;
            if (!ClockSettings.Instance.ShowMinutes)
            {
                timeString = metricTime.Hours.ToString();
            }
            else
            {
                timeString = ClockSettings.Instance.ShowSecondsHand 
                    ? metricTime.ToString()
                    : metricTime.ToStringNoSeconds();
            }
            sevenSegmentPanel.Children.Clear();
            
            foreach (char c in timeString)
            {
                if (c == ':')
                {
                    sevenSegmentPanel.Children.Add(new SevenSegmentColon());
                }
                else if (char.IsDigit(c))
                {
                    SevenSegmentDisplay digit = new SevenSegmentDisplay();
                    digit.SetDigit(c);
                    sevenSegmentPanel.Children.Add(digit);
                }
            }
        }

        public void RefreshStyle()
        {
            ApplyStyle();
            UpdateTime();
        }
    }
}