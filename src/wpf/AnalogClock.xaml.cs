using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;

namespace MetricClock
{
    public partial class AnalogClock : System.Windows.Controls.UserControl, IClockView
    {
        private const double CenterX = 125;
        private const double CenterY = 125;
        private Ellipse? hourCircle;
        private Ellipse? minuteCircle;
        private Ellipse? secondCircle;
        private TextBlock? hourNumber;
        private TextBlock? minuteNumber;
        private TextBlock? secondNumber;

        public AnalogClock()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            DrawHourMarkers();
            ApplyStyle();
            UpdateTime();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update canvas size to match control size
            ClockCanvas.Width = ActualWidth;
            ClockCanvas.Height = ActualHeight;
            HourMarkers.Width = ActualWidth;
            HourMarkers.Height = ActualHeight;
            
            // Update clock face and center dot position
            UpdateClockFacePosition();
            
            // Redraw elements for new size
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
                // For circle mode, keep the clock face visible and scaled
                var clockFace = ClockCanvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name != "CenterDot" && e.Fill != null);
                if (clockFace != null)
                {
                    double size = Math.Min(width, height) * 0.96; // 96% of smaller dimension
                    clockFace.Width = size;
                    clockFace.Height = size;
                    Canvas.SetLeft(clockFace, centerX - size / 2);
                    Canvas.SetTop(clockFace, centerY - size / 2);
                    clockFace.Visibility = Visibility.Visible;
                }
                
                // Scale center dot
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
                // For rectangle modes, hide the circular clock face
                var clockFace = ClockCanvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name != "CenterDot" && e.Fill != null);
                if (clockFace != null)
                {
                    clockFace.Visibility = Visibility.Collapsed;
                }
                
                // Scale center dot for rectangle modes too
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
            
            // Scale factors for different clock shapes
            double scale = 1.0;
            double maxRadius = 115.0; // Maximum radius for hour markers
            
            if (ClockSettings.Instance.ClockShape == ClockShape.Circle)
            {
                // For circle mode, use the smaller dimension to maintain circle
                scale = Math.Min(width, height) / 250.0;
            }
            else
            {
                // For rectangle modes, scale to fit within bounds with padding
                // Calculate scale based on ensuring the clock face fits with some margin
                double marginFactor = 0.85; // Use 85% of available space
                double scaleX = (width * marginFactor) / (2 * maxRadius);
                double scaleY = (height * marginFactor) / (2 * maxRadius);
                scale = Math.Min(scaleX, scaleY);
            }
            
            for (int i = 0; i < 12; i++)
            {
                double angle = i * 30 - 90;
                double radians = angle * Math.PI / 180;

                double innerRadius = (i % 3 == 0 ? 105 : 110) * scale;
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
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeThickness = (i % 3 == 0 ? 3 : 1) * scale
                };

                HourMarkers.Children.Add(marker);

                if (i % 3 == 0)
                {
                    int hour = i == 0 ? 12 : i;
                    TextBlock number = new TextBlock
                    {
                        Text = hour.ToString(),
                        Foreground = System.Windows.Media.Brushes.White,
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
                    hourCircle = new Ellipse
                    {
                        Width = 20 * elementScale,
                        Height = 20 * elementScale,
                        Fill = System.Windows.Media.Brushes.White,
                        Opacity = 1.0  // Moving circles stay at full opacity
                    };
                    ClockCanvas.Children.Add(hourCircle);
                    
                    minuteCircle = new Ellipse
                    {
                        Width = 16 * elementScale,
                        Height = 16 * elementScale,
                        Fill = System.Windows.Media.Brushes.White,
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

            var shape = ClockSettings.Instance.ClockShape;
            var style = ClockSettings.Instance.AnalogStyle;

            if (shape == ClockShape.Circle)
            {
                // Normal circular clock
                double width = ActualWidth > 0 ? ActualWidth : 250;
                double height = ActualHeight > 0 ? ActualHeight : 250;
                double scale = Math.Min(width, height) / 250.0;
                
                double hourAngle = (now.Hour % 12 + now.Minute / 60.0) * 30 - 90;
                
                // Calculate minute angle based on smooth setting
                double minuteAngle;
                if (ClockSettings.Instance.SmoothSecondsHand)
                {
                    // Include milliseconds for smooth movement
                    double totalSeconds = now.Second + now.Millisecond / 1000.0;
                    minuteAngle = (now.Minute + totalSeconds / 60.0) * 6 - 90;
                }
                else
                {
                    // Discrete seconds only
                    minuteAngle = (now.Minute + now.Second / 60.0) * 6 - 90;
                }
                
                // Calculate second angle based on smooth setting
                double secondAngle;
                if (ClockSettings.Instance.SmoothSecondsHand)
                {
                    // Include milliseconds for smooth movement
                    double totalSeconds = now.Second + now.Millisecond / 1000.0;
                    secondAngle = totalSeconds * 6 - 90;
                }
                else
                {
                    // Discrete seconds only
                    secondAngle = now.Second * 6 - 90;
                }

                if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
                {
                    SetHandAngle(HourHand, hourAngle, 55 * scale);
                    SetHandAngle(MinuteHand, minuteAngle, 85 * scale);
                    SetHandAngle(SecondHand, secondAngle, 100 * scale);
                }
                
                if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
                {
                    UpdateCirclePosition(hourCircle, hourAngle, 55 * scale);
                    UpdateCirclePosition(minuteCircle, minuteAngle, 85 * scale);
                    UpdateCirclePosition(secondCircle, secondAngle, 100 * scale);
                    
                    // Update number positions to match circles
                    if (ClockSettings.Instance.ShowNumbersInCircles)
                    {
                        UpdateNumberPosition(hourNumber, hourAngle, 55 * scale, now.Hour % 12);
                        UpdateNumberPosition(minuteNumber, minuteAngle, 85 * scale, now.Minute);
                        UpdateNumberPosition(secondNumber, secondAngle, 100 * scale, now.Second);
                    }
                }
            }
            else if (shape == ClockShape.ProjectedRectangle)
            {
                // Projected rectangle - extend lines to window border
                UpdateProjectedRectangle(now, style);
            }
            else if (shape == ClockShape.RectanglePath)
            {
                // Rectangle path - hands follow perimeter
                UpdateRectanglePath(now, style);
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

        private void UpdateProjectedRectangle(DateTime now, AnalogClockStyle style)
        {
            // Get the actual bounds from the parent container
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            
            double hourAngle = (now.Hour % 12 + now.Minute / 60.0) * 30 - 90;
            
            // Calculate minute angle based on smooth setting
            double minuteAngle;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                // Include milliseconds for smooth movement
                double totalSeconds = now.Second + now.Millisecond / 1000.0;
                minuteAngle = (now.Minute + totalSeconds / 60.0) * 6 - 90;
            }
            else
            {
                // Discrete seconds only
                minuteAngle = (now.Minute + now.Second / 60.0) * 6 - 90;
            }
            
            double secondAngle;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                double totalSeconds = now.Second + now.Millisecond / 1000.0;
                secondAngle = totalSeconds * 6 - 90;
            }
            else
            {
                secondAngle = now.Second * 6 - 90;
            }

            if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
            {
                // Project hands to rectangle border
                SetHandToRectangleBorder(HourHand, hourAngle, width, height);
                SetHandToRectangleBorder(MinuteHand, minuteAngle, width, height);
                SetHandToRectangleBorder(SecondHand, secondAngle, width, height);
            }

            if (style == AnalogClockStyle.Circles || style == AnalogClockStyle.CirclesWithHands)
            {
                // Project circles to rectangle border
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
                        Canvas.SetLeft(hourNumber, x - 6);
                        Canvas.SetTop(hourNumber, y - 5);
                        hourNumber.Text = (now.Hour % 12).ToString();
                    }
                    if (minuteNumber != null && minuteCircle != null)
                    {
                        double radius = minuteCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                        Canvas.SetLeft(minuteNumber, x - 6);
                        Canvas.SetTop(minuteNumber, y - 5);
                        minuteNumber.Text = now.Minute.ToString();
                    }
                    if (secondNumber != null && secondCircle != null)
                    {
                        double radius = secondCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                        Canvas.SetLeft(secondNumber, x - 6);
                        Canvas.SetTop(secondNumber, y - 5);
                        secondNumber.Text = now.Second.ToString();
                    }
                }
            }
        }

        private void UpdateRectanglePath(DateTime now, AnalogClockStyle style)
        {
            // Get the actual bounds from the parent container
            double width = ActualWidth > 0 ? ActualWidth : 250;
            double height = ActualHeight > 0 ? ActualHeight : 250;
            
            // Calculate positions along rectangle perimeter
            var hourPos = GetRectanglePathPosition(now.Hour % 12 + now.Minute / 60.0, 12, width, height);
            
            // Calculate minute position based on smooth setting
            System.Windows.Point minutePos;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                // Include milliseconds for smooth movement
                double totalSeconds = now.Second + now.Millisecond / 1000.0;
                minutePos = GetRectanglePathPosition(now.Minute + totalSeconds / 60.0, 60, width, height);
            }
            else
            {
                // Discrete seconds only
                minutePos = GetRectanglePathPosition(now.Minute + now.Second / 60.0, 60, width, height);
            }
            
            System.Windows.Point secondPos;
            if (ClockSettings.Instance.SmoothSecondsHand)
            {
                double totalSeconds = now.Second + now.Millisecond / 1000.0;
                secondPos = GetRectanglePathPosition(totalSeconds, 60, width, height);
            }
            else
            {
                secondPos = GetRectanglePathPosition(now.Second, 60, width, height);
            }

            if (style == AnalogClockStyle.Traditional || style == AnalogClockStyle.CirclesWithHands)
            {
                // Set hands from center to perimeter positions
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
                        Canvas.SetLeft(hourNumber, x - 6);
                        Canvas.SetTop(hourNumber, y - 5);
                        hourNumber.Text = (now.Hour % 12).ToString();
                    }
                    if (minuteNumber != null && minuteCircle != null)
                    {
                        double radius = minuteCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, minutePos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, minutePos.Y));
                        Canvas.SetLeft(minuteNumber, x - 6);
                        Canvas.SetTop(minuteNumber, y - 5);
                        minuteNumber.Text = now.Minute.ToString();
                    }
                    if (secondNumber != null && secondCircle != null)
                    {
                        double radius = secondCircle.Width / 2;
                        double x = Math.Max(radius, Math.Min(width - radius, secondPos.X));
                        double y = Math.Max(radius, Math.Min(height - radius, secondPos.Y));
                        Canvas.SetLeft(secondNumber, x - 6);
                        Canvas.SetTop(secondNumber, y - 5);
                        secondNumber.Text = now.Second.ToString();
                    }
                }
            }
        }

        private void SetHandToRectangleBorder(Line hand, double angleDegrees, double width, double height)
        {
            var point = GetRectangleBorderPoint(angleDegrees, width, height);
            // Update hand start position to actual center
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
            
            // Ray from actual center of window
            double centerX = width / 2;
            double centerY = height / 2;
            
            // Find intersection with rectangle border
            double t;
            if (Math.Abs(cos) < 0.0001) // Vertical line
            {
                t = sin > 0 ? (height - centerY) / sin : -centerY / sin;
            }
            else if (Math.Abs(sin) < 0.0001) // Horizontal line
            {
                t = cos > 0 ? (width - centerX) / cos : -centerX / cos;
            }
            else
            {
                // Find which border we hit first
                double tRight = (width - centerX) / cos;
                double tLeft = -centerX / cos;
                double tBottom = (height - centerY) / sin;
                double tTop = -centerY / sin;
                
                // We want the smallest positive t
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
            // Map value to position along rectangle perimeter
            // Start at top center (12 o'clock), go clockwise
            double perimeter = 2 * (width + height);
            double distance = (value / max) * perimeter;
            
            double x, y;
            
            if (distance < width / 2)
            {
                // Top edge, right from center
                x = width / 2 + distance;
                y = 0;
            }
            else if (distance < width / 2 + height)
            {
                // Right edge
                x = width;
                y = distance - width / 2;
            }
            else if (distance < width / 2 + height + width)
            {
                // Bottom edge
                x = width - (distance - width / 2 - height);
                y = height;
            }
            else if (distance < width / 2 + 2 * height + width)
            {
                // Left edge
                x = 0;
                y = height - (distance - width / 2 - height - width);
            }
            else
            {
                // Top edge, left from center
                x = distance - width / 2 - 2 * height - width;
                y = 0;
            }
            
            return new System.Windows.Point(x, y);
        }
    }
}