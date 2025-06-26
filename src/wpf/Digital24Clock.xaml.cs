using System.Windows.Controls;
using System.Windows.Media;

namespace MetricClock
{
    public partial class Digital24Clock : System.Windows.Controls.UserControl, IClockView
    {
        private StackPanel? sevenSegmentPanel;

        public Digital24Clock()
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
                    TimeDisplay.Text = now.ToString("HH");
                }
                else
                {
                    TimeDisplay.Text = ClockSettings.Instance.ShowSecondsHand 
                        ? now.ToString("HH:mm:ss")
                        : now.ToString("HH:mm");
                }
            }
            
            if (ClockSettings.Instance.ShowDate)
            {
                DateDisplay.Text = now.ToString("yyyy-MM-dd");
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
                timeString = time.ToString("HH");
            }
            else
            {
                timeString = ClockSettings.Instance.ShowSecondsHand 
                    ? time.ToString("HH:mm:ss")
                    : time.ToString("HH:mm");
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