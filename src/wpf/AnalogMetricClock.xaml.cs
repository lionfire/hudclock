using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;

namespace MetricClock
{
    public partial class AnalogMetricClock : System.Windows.Controls.UserControl, IClockView
    {
        private const double CenterX = 125;
        private const double CenterY = 125;
        private Ellipse? hourCircle;
        private Ellipse? minuteCircle;
        private Ellipse? secondCircle;
        private TextBlock? hourNumber;
        private TextBlock? minuteNumber;
        private TextBlock? secondNumber;

        public AnalogMetricClock()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            DrawHourMarkers();
            ApplyStyle();
            UpdateTime();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClockCanvas.Width = ActualWidth;
            ClockCanvas.Height = ActualHeight;
            HourMarkers.Width = ActualWidth;
            HourMarkers.Height = ActualHeight;
            
            UpdateClockFacePosition();
            DrawHourMarkers();
            UpdateTime();
        }
        
        private void UpdateClockFacePosition()
        {
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            double centerX = width / 2;
            double centerY = height / 2;
            
            if (ClockSettings.Instance.ClockShape == ClockShape.Circle)
            {
                var clockFace = ClockCanvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name != "CenterDot" && e.Fill != null);
                if (clockFace != null)
                {
                    double size = Math.Min(width, height) * 0.96;
                    clockFace.Width = size;
                    clockFace.Height = size;
                    Canvas.SetLeft(clockFace, centerX - size / 2);
                    Canvas.SetTop(clockFace, centerY - size / 2);
                    clockFace.Visibility = Visibility.Visible;
                }
                
                if (CenterDot != null)
                {
                    double scale = Math.Min(width, height) / 250.0;
                    CenterDot.Width = 10 * scale;
                    CenterDot.Height = 10 * scale;
                    Canvas.SetLeft(CenterDot, centerX - CenterDot.Width / 2);
                    Canvas.SetTop(CenterDot, centerY - CenterDot.Height / 2);
                }
            }
            else
            {
                var clockFace = ClockCanvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name != "CenterDot" && e.Fill != null);
                if (clockFace != null)
                {
                    clockFace.Visibility = Visibility.Collapsed;
                }
                
                if (CenterDot != null)
                {
                    double scale = Math.Min(width, height) / 250.0;
                    CenterDot.Width = 10 * scale;
                    CenterDot.Height = 10 * scale;
                    Canvas.SetLeft(CenterDot, centerX - CenterDot.Width / 2);
                    Canvas.SetTop(CenterDot, centerY - CenterDot.Height / 2);
                }
            }
        }

        private void DrawHourMarkers()
        {
            HourMarkers.Children.Clear();
            
            // Dynamic sizing based on window dimensions
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            double centerX = width / 2;
            double centerY = height / 2;
            
            // Scale factor for different clock shapes
            double scale = 1.0;
            double maxRadius = 115.0;
            
            if (ClockSettings.Instance.ClockShape == ClockShape.Circle)
            {
                scale = Math.Min(width, height) / 250.0;
            }
            else
            {
                // For rectangle modes, scale to fit within bounds with padding
                double marginFactor = 0.85;
                double scaleX = (width * marginFactor) / (2 * maxRadius);
                double scaleY = (height * marginFactor) / (2 * maxRadius);
                scale = Math.Min(scaleX, scaleY);
            }
            
            for (int i = 0; i < 10; i++)
            {
                double angle = i * 36 - 90;
                double radians = angle * Math.PI / 180;

                double innerRadius = 105 * scale;
                double outerRadius = 115 * scale;

                double x1 = centerX + innerRadius * Math.Cos(radians);
                double y1 = centerY + innerRadius * Math.Sin(radians);
                double x2 = centerX + outerRadius * Math.Cos(radians);
                double y2 = centerY + outerRadius * Math.Sin(radians);

                Line marker = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 204, 255)),
                    StrokeThickness = 3 * scale
                };

                HourMarkers.Children.Add(marker);

                TextBlock number = new TextBlock
                {
                    Text = i.ToString(),
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 204, 255)),
                    FontSize = 16 * scale,
                    FontWeight = FontWeights.Bold
                };

                double textRadius = 90 * scale;
                double textX = centerX + textRadius * Math.Cos(radians);
                double textY = centerY + textRadius * Math.Sin(radians);

                Canvas.SetLeft(number, textX - 8 * scale);
                Canvas.SetTop(number, textY - 10 * scale);
                HourMarkers.Children.Add(number);
            }

            for (int i = 0; i < 100; i++)
            {
                if (i % 10 != 0)
                {
                    double angle = i * 3.6 - 90;
                    double radians = angle * Math.PI / 180;

                    double innerRadius = 110 * scale;
                    double outerRadius = 115 * scale;

                    double x1 = centerX + innerRadius * Math.Cos(radians);
                    double y1 = centerY + innerRadius * Math.Sin(radians);
                    double x2 = centerX + outerRadius * Math.Cos(radians);
                    double y2 = centerY + outerRadius * Math.Sin(radians);

                    Line marker = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 204, 255)),
                        StrokeThickness = 1 * scale,
                        Opacity = 0.5
                    };

                    HourMarkers.Children.Add(marker);
                }
            }
        }

        private void ApplyStyle()
        {
            var style = ClockSettings.Instance.AnalogStyle;
            bool showSeconds = ClockSettings.Instance.ShowSecondsHand;
            bool showMinutes = ClockSettings.Instance.ShowMinutes;
            double staticOpacity = ClockSettings.Instance.ClockElementOpacity;
            double elementScale = ClockSettings.Instance.ElementScale;
            
            // Apply opacity only to static elements (NOT moving hands)
            // Apply opacity to hour markers
            foreach (UIElement child in HourMarkers.Children)
            {
                child.Opacity = staticOpacity;
            }
            
            // Apply opacity to clock face and center dot
            if (ClockFace != null)
            {
                ClockFace.Opacity = staticOpacity;
            }
            if (CenterDot != null)
            {
                CenterDot.Opacity = staticOpacity;
            }
            
            // Moving hands always stay at full opacity (1.0)
            HourHand.Opacity = 1.0;
            MinuteHand.Opacity = 1.0;
            SecondHand.Opacity = 1.0;
            
            // Scale hand thickness
            HourHand.StrokeThickness = 6 * elementScale;
            MinuteHand.StrokeThickness = 4 * elementScale;
            SecondHand.StrokeThickness = 2 * elementScale;
            
            // Update center dot visibility based on settings and whether hands are visible
            bool handsVisible = (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands);
            if (CenterDot != null)
            {
                CenterDot.Visibility = (ClockSettings.Instance.ShowCenterDot && handsVisible) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
            
            if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
            {
                if (hourCircle == null)
                {
                    var cyanBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 204, 255));
                    
                    hourCircle = new Ellipse
                    {
                        Width = 20 * elementScale,
                        Height = 20 * elementScale,
                        Fill = cyanBrush,
                        Opacity = 1.0  // Moving circles stay at full opacity
                    };
                    ClockCanvas.Children.Add(hourCircle);
                    
                    minuteCircle = new Ellipse
                    {
                        Width = 16 * elementScale,
                        Height = 16 * elementScale,
                        Fill = cyanBrush,
                        Opacity = 1.0  // Moving circles stay at full opacity
                    };
                    ClockCanvas.Children.Add(minuteCircle);
                    
                    secondCircle = new Ellipse
                    {
                        Width = 12 * elementScale,
                        Height = 12 * elementScale,
                        Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 68, 68)),
                        Opacity = 1.0  // Moving circles stay at full opacity
                    };
                    ClockCanvas.Children.Add(secondCircle);
                    
                    // Create number text blocks for circles if enabled
                    if (ClockSettings.Instance.ShowNumbersInCircles)
                    {
                        hourNumber = new TextBlock
                        {
                            Text = "",
                            Foreground = System.Windows.Media.Brushes.Black,
                            FontSize = 10 * elementScale,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Opacity = 1.0
                        };
                        ClockCanvas.Children.Add(hourNumber);
                        
                        minuteNumber = new TextBlock
                        {
                            Text = "",
                            Foreground = System.Windows.Media.Brushes.Black,
                            FontSize = 9 * elementScale,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Opacity = 1.0
                        };
                        ClockCanvas.Children.Add(minuteNumber);
                        
                        secondNumber = new TextBlock
                        {
                            Text = "",
                            Foreground = System.Windows.Media.Brushes.Black,
                            FontSize = 8 * elementScale,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Opacity = 1.0
                        };
                        ClockCanvas.Children.Add(secondNumber);
                    }
                }
                else
                {
                    // Moving circles always stay at full opacity
                    hourCircle.Opacity = 1.0;
                    minuteCircle!.Opacity = 1.0;
                    secondCircle!.Opacity = 1.0;
                    
                    // Update sizes based on current scale
                    hourCircle.Width = 20 * elementScale;
                    hourCircle.Height = 20 * elementScale;
                    minuteCircle.Width = 16 * elementScale;
                    minuteCircle.Height = 16 * elementScale;
                    secondCircle.Width = 12 * elementScale;
                    secondCircle.Height = 12 * elementScale;
                    
                    // Update number visibility based on settings
                    if (ClockSettings.Instance.ShowNumbersInCircles)
                    {
                        if (hourNumber == null)
                        {
                            hourNumber = new TextBlock
                            {
                                Text = "",
                                Foreground = System.Windows.Media.Brushes.Black,
                                FontSize = 10 * elementScale,
                                FontWeight = FontWeights.Bold,
                                TextAlignment = TextAlignment.Center,
                                Opacity = 1.0
                            };
                            ClockCanvas.Children.Add(hourNumber);
                        }
                        
                        if (minuteNumber == null)
                        {
                            minuteNumber = new TextBlock
                            {
                                Text = "",
                                Foreground = System.Windows.Media.Brushes.Black,
                                FontSize = 9 * elementScale,
                                FontWeight = FontWeights.Bold,
                                TextAlignment = TextAlignment.Center,
                                Opacity = 1.0
                            };
                            ClockCanvas.Children.Add(minuteNumber);
                        }
                        
                        if (secondNumber == null)
                        {
                            secondNumber = new TextBlock
                            {
                                Text = "",
                                Foreground = System.Windows.Media.Brushes.Black,
                                FontSize = 8 * elementScale,
                                FontWeight = FontWeights.Bold,
                                TextAlignment = TextAlignment.Center,
                                Opacity = 1.0
                            };
                            ClockCanvas.Children.Add(secondNumber);
                        }
                    }
                    else
                    {
                        // Hide numbers if disabled
                        if (hourNumber != null) hourNumber.Visibility = Visibility.Collapsed;
                        if (minuteNumber != null) minuteNumber.Visibility = Visibility.Collapsed;
                        if (secondNumber != null) secondNumber.Visibility = Visibility.Collapsed;
                    }
                    
                    // Update font sizes if numbers exist
                    if (hourNumber != null) hourNumber.FontSize = 10 * elementScale;
                    if (minuteNumber != null) minuteNumber.FontSize = 9 * elementScale;
                    if (secondNumber != null) secondNumber.FontSize = 8 * elementScale;
                }
            }
            
            // Show/hide elements based on style
            if (style == AnalogClockStyle.Traditional)
            {
                HourHand.Visibility = Visibility.Visible;
                MinuteHand.Visibility = showMinutes ? Visibility.Visible : Visibility.Collapsed;
                SecondHand.Visibility = showSeconds ? Visibility.Visible : Visibility.Collapsed;
                
                if (hourCircle != null) hourCircle.Visibility = Visibility.Collapsed;
                if (minuteCircle != null) minuteCircle.Visibility = Visibility.Collapsed;
                if (secondCircle != null) secondCircle.Visibility = Visibility.Collapsed;
                if (hourNumber != null) hourNumber.Visibility = Visibility.Collapsed;
                if (minuteNumber != null) minuteNumber.Visibility = Visibility.Collapsed;
                if (secondNumber != null) secondNumber.Visibility = Visibility.Collapsed;
            }
            else if (style == AnalogClockStyle.Circles)
            {
                HourHand.Visibility = Visibility.Collapsed;
                MinuteHand.Visibility = Visibility.Collapsed;
                SecondHand.Visibility = Visibility.Collapsed;
                
                if (hourCircle != null) hourCircle.Visibility = Visibility.Visible;
                if (minuteCircle != null) minuteCircle.Visibility = showMinutes ? Visibility.Visible : Visibility.Collapsed;
                if (secondCircle != null) secondCircle.Visibility = showSeconds ? Visibility.Visible : Visibility.Collapsed;
                
                // Show/hide numbers in circles based on setting
                bool showNumbers = ClockSettings.Instance.ShowNumbersInCircles;
                if (hourNumber != null) hourNumber.Visibility = showNumbers ? Visibility.Visible : Visibility.Collapsed;
                if (minuteNumber != null) minuteNumber.Visibility = (showNumbers && showMinutes) ? Visibility.Visible : Visibility.Collapsed;
                if (secondNumber != null) secondNumber.Visibility = (showNumbers && showSeconds) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (style == AnalogClockStyle.CirclesWithHands)
            {
                HourHand.Visibility = Visibility.Visible;
                MinuteHand.Visibility = showMinutes ? Visibility.Visible : Visibility.Collapsed;
                SecondHand.Visibility = showSeconds ? Visibility.Visible : Visibility.Collapsed;
                
                if (hourCircle != null) hourCircle.Visibility = Visibility.Visible;
                if (minuteCircle != null) minuteCircle.Visibility = showMinutes ? Visibility.Visible : Visibility.Collapsed;
                if (secondCircle != null) secondCircle.Visibility = showSeconds ? Visibility.Visible : Visibility.Collapsed;
                
                // Show/hide numbers in circles based on setting
                bool showNumbers = ClockSettings.Instance.ShowNumbersInCircles;
                if (hourNumber != null) hourNumber.Visibility = showNumbers ? Visibility.Visible : Visibility.Collapsed;
                if (minuteNumber != null) minuteNumber.Visibility = (showNumbers && showMinutes) ? Visibility.Visible : Visibility.Collapsed;
                if (secondNumber != null) secondNumber.Visibility = (showNumbers && showSeconds) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void UpdateTime()
        {
            DateTime now = DateTime.Now;
            MetricTime metricTime = TimeHelpers.GetMetricTime(now);

            var shape = ClockSettings.Instance.ClockShape;
            var style = ClockSettings.Instance.AnalogStyle;

            if (shape == ClockShape.Circle)
            {
                // Normal circular clock
                double hourAngle = (metricTime.Hours + metricTime.Minutes / 100.0) * 36 - 90;
                
                // Calculate minute angle based on smooth setting
                double minuteAngle;
                if (ClockSettings.Instance.SmoothSecondsHand)
                {
                    // Get the fractional part of the current metric time including milliseconds
                    double totalStandardSeconds = now.TimeOfDay.TotalSeconds;
                    double totalMetricSeconds = totalStandardSeconds / 0.864;
                    double totalMetricMinutes = totalMetricSeconds / 100.0;
                    double metricMinutesInHour = totalMetricMinutes % 100.0;
                    minuteAngle = metricMinutesInHour * 3.6 - 90;
                }
                else
                {
                    // Discrete seconds only
                    minuteAngle = (metricTime.Minutes + metricTime.Seconds / 100.0) * 3.6 - 90;
                }
                
                double secondAngle;
                if (ClockSettings.Instance.SmoothSecondsHand)
                {
                    // Get the fractional part of the current metric time
                    double totalStandardSeconds = now.TimeOfDay.TotalSeconds;
                    double totalMetricSeconds = totalStandardSeconds / 0.864;
                    double metricSecondsWithFraction = totalMetricSeconds % 100.0; // Get seconds within current metric minute
                    secondAngle = metricSecondsWithFraction * 3.6 - 90;
                }
                else
                {
                    secondAngle = metricTime.Seconds * 3.6 - 90;
                }

                if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
                {
                    double width = ActualWidth > 0 ? ActualWidth : 250;
                    double height = ActualHeight > 0 ? ActualHeight : 250;
                    double scale = Math.Min(width, height) / 250.0;
                    
                    SetHandAngle(HourHand, hourAngle, 55 * scale);
                    SetHandAngle(MinuteHand, minuteAngle, 85 * scale);
                    SetHandAngle(SecondHand, secondAngle, 100 * scale);
                }
                
                if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
                {
                    double width = ActualWidth > 0 ? ActualWidth : 250;
                    double height = ActualHeight > 0 ? ActualHeight : 250;
                    double scale = Math.Min(width, height) / 250.0;
                    
                    UpdateCirclePosition(hourCircle, hourAngle, 55 * scale);
                    UpdateCirclePosition(minuteCircle, minuteAngle, 85 * scale);
                    UpdateCirclePosition(secondCircle, secondAngle, 100 * scale);
                    
                    // Update number positions to match circles
                    if (ClockSettings.Instance.ShowNumbersInCircles)
                    {
                        UpdateNumberPosition(hourNumber, hourAngle, 55 * scale, metricTime.Hours);
                        UpdateNumberPosition(minuteNumber, minuteAngle, 85 * scale, (int)(metricTime.Minutes / 10)); // One significant digit
                        UpdateNumberPosition(secondNumber, secondAngle, 100 * scale, (int)(metricTime.Seconds / 10)); // One significant digit
                    }
                }
            }
            else if (shape == ClockShape.ProjectedRectangle)
            {
                UpdateProjectedRectangle(metricTime, style);
            }
            else if (shape == ClockShape.RectanglePath)
            {
                UpdateRectanglePath(metricTime, style);
            }
        }

        private void UpdateCirclePosition(Ellipse? circle, double angleDegrees, double radius)
        {
            if (circle == null) return;
            
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            double centerX = width / 2;
            double centerY = height / 2;
            
            double radians = angleDegrees * Math.PI / 180;
            double x = centerX + radius * Math.Cos(radians) - circle.Width / 2;
            double y = centerY + radius * Math.Sin(radians) - circle.Height / 2;
            
            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
        }

        private void UpdateNumberPosition(TextBlock? number, double angleDegrees, double radius, int value)
        {
            if (number == null) return;
            
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            double centerX = width / 2;
            double centerY = height / 2;
            
            // Update the text to show the actual time value
            number.Text = value.ToString();
            
            // Force measurement of the text
            number.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = number.DesiredSize.Width;
            double textHeight = number.DesiredSize.Height;
            
            double radians = angleDegrees * Math.PI / 180;
            
            // Calculate position to center the text exactly at the circle position
            double x = centerX + radius * Math.Cos(radians) - textWidth / 2;
            double y = centerY + radius * Math.Sin(radians) - textHeight / 2;
            
            Canvas.SetLeft(number, x);
            Canvas.SetTop(number, y);
        }

        private void SetHandAngle(Line hand, double angleDegrees, double length)
        {
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            double centerX = width / 2;
            double centerY = height / 2;
            
            double radians = angleDegrees * Math.PI / 180;
            hand.X1 = centerX;
            hand.Y1 = centerY;
            hand.X2 = centerX + length * Math.Cos(radians);
            hand.Y2 = centerY + length * Math.Sin(radians);
        }

        public void RefreshStyle()
        {
            ApplyStyle();
            UpdateClockFacePosition();
            UpdateTime();
        }

        private void UpdateProjectedRectangle(MetricTime metricTime, AnalogClockStyle style)
        {
            DateTime now = DateTime.Now;
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            
            double hourAngle = (metricTime.Hours + metricTime.Minutes / 100.0) * 36 - 90;
            
            // Calculate minute angle based on smooth setting
            double minuteAngle;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                // Get the fractional part of the current metric time including milliseconds
                double totalStandardSeconds = now.TimeOfDay.TotalSeconds;
                double totalMetricSeconds = totalStandardSeconds / 0.864;
                double totalMetricMinutes = totalMetricSeconds / 100.0;
                double metricMinutesInHour = totalMetricMinutes % 100.0;
                minuteAngle = metricMinutesInHour * 3.6 - 90;
            }
            else
            {
                // Discrete seconds only
                minuteAngle = (metricTime.Minutes + metricTime.Seconds / 100.0) * 3.6 - 90;
            }
            
            double secondAngle;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                double totalSeconds = metricTime.Seconds + (now.Millisecond / 1000.0) * 100.0 / 0.864;
                secondAngle = totalSeconds * 3.6 - 90;
            }
            else
            {
                secondAngle = metricTime.Seconds * 3.6 - 90;
            }

            if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
            {
                SetHandToRectangleBorder(HourHand, hourAngle, width, height);
                SetHandToRectangleBorder(MinuteHand, minuteAngle, width, height);
                SetHandToRectangleBorder(SecondHand, secondAngle, width, height);
            }

            if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
            {
                var hourPos = GetRectangleBorderPoint(hourAngle, width, height);
                var minutePos = GetRectangleBorderPoint(minuteAngle, width, height);
                var secondPos = GetRectangleBorderPoint(secondAngle, width, height);

                if (hourCircle != null)
                {
                    double radius = hourCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, hourPos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, hourPos.Y));
                    Canvas.SetLeft(hourCircle, x - radius);
                    Canvas.SetTop(hourCircle, y - radius);
                }
                if (minuteCircle != null)
                {
                    double radius = minuteCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                    Canvas.SetLeft(minuteCircle, x - radius);
                    Canvas.SetTop(minuteCircle, y - radius);
                }
                if (secondCircle != null)
                {
                    double radius = secondCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                    Canvas.SetLeft(secondCircle, x - radius);
                    Canvas.SetTop(secondCircle, y - radius);
                }

                if (ClockSettings.Instance.ShowNumbersInCircles)
                {
                    if (hourNumber != null && hourCircle != null)
                    {
                        double radius = hourCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, hourPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, hourPos.Y));
                        Canvas.SetLeft(hourNumber, x - 5);
                        Canvas.SetTop(hourNumber, y - 4.5);
                        hourNumber.Text = metricTime.Hours.ToString();
                    }
                    if (minuteNumber != null && minuteCircle != null)
                    {
                        double radius = minuteCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                        Canvas.SetLeft(minuteNumber, x - 5);
                        Canvas.SetTop(minuteNumber, y - 4.5);
                        minuteNumber.Text = ((int)(metricTime.Minutes / 10)).ToString();
                    }
                    if (secondNumber != null && secondCircle != null)
                    {
                        double radius = secondCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                        Canvas.SetLeft(secondNumber, x - 5);
                        Canvas.SetTop(secondNumber, y - 4.5);
                        secondNumber.Text = ((int)(metricTime.Seconds / 10)).ToString();
                    }
                }
            }
        }

        private void UpdateRectanglePath(MetricTime metricTime, AnalogClockStyle style)
        {
            DateTime now = DateTime.Now;
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            
            var hourPos = GetRectanglePathPosition(metricTime.Hours + metricTime.Minutes / 100.0, 10, width, height);
            
            // Calculate minute position based on smooth setting
            System.Windows.Point minutePos;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                // Get the fractional part of the current metric time including milliseconds
                double totalStandardSeconds = now.TimeOfDay.TotalSeconds;
                double totalMetricSeconds = totalStandardSeconds / 0.864;
                double totalMetricMinutes = totalMetricSeconds / 100.0;
                double metricMinutesInHour = totalMetricMinutes % 100.0;
                minutePos = GetRectanglePathPosition(metricMinutesInHour, 100, width, height);
            }
            else
            {
                // Discrete seconds only
                minutePos = GetRectanglePathPosition(metricTime.Minutes + metricTime.Seconds / 100.0, 100, width, height);
            }
            
            System.Windows.Point secondPos;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                double totalSeconds = metricTime.Seconds + (now.Millisecond / 1000.0) * 100.0 / 0.864;
                secondPos = GetRectanglePathPosition(totalSeconds, 100, width, height);
            }
            else
            {
                secondPos = GetRectanglePathPosition(metricTime.Seconds, 100, width, height);
            }

            if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
            {
                double centerX = width / 2;
                double centerY = height / 2;
                
                HourHand.X1 = centerX;
                HourHand.Y1 = centerY;
                HourHand.X2 = hourPos.X;
                HourHand.Y2 = hourPos.Y;
                
                MinuteHand.X1 = centerX;
                MinuteHand.Y1 = centerY;
                MinuteHand.X2 = minutePos.X;
                MinuteHand.Y2 = minutePos.Y;
                
                SecondHand.X1 = centerX;
                SecondHand.Y1 = centerY;
                SecondHand.X2 = secondPos.X;
                SecondHand.Y2 = secondPos.Y;
            }

            if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
            {
                if (hourCircle != null)
                {
                    double radius = hourCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, hourPos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, hourPos.Y));
                    Canvas.SetLeft(hourCircle, x - radius);
                    Canvas.SetTop(hourCircle, y - radius);
                }
                if (minuteCircle != null)
                {
                    double radius = minuteCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                    Canvas.SetLeft(minuteCircle, x - radius);
                    Canvas.SetTop(minuteCircle, y - radius);
                }
                if (secondCircle != null)
                {
                    double radius = secondCircle.Width / 2;
                    double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                    double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                    Canvas.SetLeft(secondCircle, x - radius);
                    Canvas.SetTop(secondCircle, y - radius);
                }

                if (ClockSettings.Instance.ShowNumbersInCircles)
                {
                    if (hourNumber != null && hourCircle != null)
                    {
                        double radius = hourCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, hourPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, hourPos.Y));
                        Canvas.SetLeft(hourNumber, x - 5);
                        Canvas.SetTop(hourNumber, y - 4.5);
                        hourNumber.Text = metricTime.Hours.ToString();
                    }
                    if (minuteNumber != null && minuteCircle != null)
                    {
                        double radius = minuteCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                        Canvas.SetLeft(minuteNumber, x - 5);
                        Canvas.SetTop(minuteNumber, y - 4.5);
                        minuteNumber.Text = ((int)(metricTime.Minutes / 10)).ToString();
                    }
                    if (secondNumber != null && secondCircle != null)
                    {
                        double radius = secondCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                        Canvas.SetLeft(secondNumber, x - 5);
                        Canvas.SetTop(secondNumber, y - 4.5);
                        secondNumber.Text = ((int)(metricTime.Seconds / 10)).ToString();
                    }
                }
            }
        }

        private void SetHandToRectangleBorder(Line hand, double angleDegrees, double width, double height)
        {
            var point = GetRectangleBorderPoint(angleDegrees, width, height);
            hand.X1 = width / 2;
            hand.Y1 = height / 2;
            hand.X2 = point.X;
            hand.Y2 = point.Y;
        }

        private System.Windows.Point GetRectangleBorderPoint(double angleDegrees, double width, double height)
        {
            double radians = angleDegrees * Math.PI / 180;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            
            double centerX = width / 2;
            double centerY = height / 2;
            
            double t;
            if (Math.Abs(cos) < 0.0001)
            {
                t = sin > 0 ? (height - centerY) / sin : -centerY / sin;
            }
            else if (Math.Abs(sin) < 0.0001)
            {
                t = cos > 0 ? (width - centerX) / cos : -centerX / cos;
            }
            else
            {
                double tRight = (width - centerX) / cos;
                double tLeft = -centerX / cos;
                double tBottom = (height - centerY) / sin;
                double tTop = -centerY / sin;
                
                t = double.MaxValue;
                if (tRight > 0 && tRight < t) t = tRight;
                if (tLeft > 0 && tLeft < t) t = tLeft;
                if (tBottom > 0 && tBottom < t) t = tBottom;
                if (tTop > 0 && tTop < t) t = tTop;
            }
            
            return new System.Windows.Point(centerX + t * cos, centerY + t * sin);
        }

        private System.Windows.Point GetRectanglePathPosition(double value, double max, double width, double height)
        {
            double perimeter = 2 * (width + height);
            double distance = (value / max) * perimeter;
            
            double x, y;
            
            if (distance < width / 2)
            {
                x = width / 2 + distance;
                y = 0;
            }
            else if (distance < width / 2 + height)
            {
                x = width;
                y = distance - width / 2;
            }
            else if (distance < width / 2 + height + width)
            {
                x = width - (distance - width / 2 - height);
                y = height;
            }
            else if (distance < width / 2 + 2 * height + width)
            {
                x = 0;
                y = height - (distance - width / 2 - height - width);
            }
            else
            {
                x = distance - width / 2 - 2 * height - width;
                y = 0;
            }
            
            return new System.Windows.Point(x, y);
        }
    }
}