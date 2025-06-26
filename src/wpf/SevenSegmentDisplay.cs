using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MetricClock
{
    public class SevenSegmentDisplay : Canvas
    {
        private readonly Path[] segments = new Path[7];
        private readonly SolidColorBrush onBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
        private readonly SolidColorBrush offBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 40, 0));

        public SevenSegmentDisplay()
        {
            Width = 40;
            Height = 60;
            CreateSegments();
        }

        private void CreateSegments()
        {
            // Segment definitions (points for each segment polygon)
            System.Windows.Point[][] segmentPoints = new System.Windows.Point[][]
            {
                // Top (0)
                new[] { new System.Windows.Point(5, 2), new System.Windows.Point(35, 2), new System.Windows.Point(32, 5), new System.Windows.Point(8, 5) },
                // Top Right (1)
                new[] { new System.Windows.Point(36, 5), new System.Windows.Point(36, 28), new System.Windows.Point(33, 25), new System.Windows.Point(33, 8) },
                // Bottom Right (2)
                new[] { new System.Windows.Point(36, 32), new System.Windows.Point(36, 55), new System.Windows.Point(33, 52), new System.Windows.Point(33, 35) },
                // Bottom (3)
                new[] { new System.Windows.Point(5, 58), new System.Windows.Point(35, 58), new System.Windows.Point(32, 55), new System.Windows.Point(8, 55) },
                // Bottom Left (4)
                new[] { new System.Windows.Point(4, 32), new System.Windows.Point(4, 55), new System.Windows.Point(7, 52), new System.Windows.Point(7, 35) },
                // Top Left (5)
                new[] { new System.Windows.Point(4, 5), new System.Windows.Point(4, 28), new System.Windows.Point(7, 25), new System.Windows.Point(7, 8) },
                // Middle (6)
                new[] { new System.Windows.Point(8, 29), new System.Windows.Point(32, 29), new System.Windows.Point(32, 31), new System.Windows.Point(8, 31) }
            };

            for (int i = 0; i < 7; i++)
            {
                PathFigure figure = new PathFigure
                {
                    StartPoint = segmentPoints[i][0],
                    IsClosed = true,
                    IsFilled = true
                };

                for (int j = 1; j < segmentPoints[i].Length; j++)
                {
                    figure.Segments.Add(new LineSegment(segmentPoints[i][j], true));
                }

                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);

                segments[i] = new Path
                {
                    Data = geometry,
                    Fill = offBrush
                };

                Children.Add(segments[i]);
            }
        }

        public void SetDigit(char digit)
        {
            // Segment patterns for each digit
            bool[][] patterns = new bool[][]
            {
                new[] { true, true, true, true, true, true, false },     // 0
                new[] { false, true, true, false, false, false, false }, // 1
                new[] { true, true, false, true, true, false, true },    // 2
                new[] { true, true, true, true, false, false, true },    // 3
                new[] { false, true, true, false, false, true, true },   // 4
                new[] { true, false, true, true, false, true, true },    // 5
                new[] { true, false, true, true, true, true, true },     // 6
                new[] { true, true, true, false, false, false, false },  // 7
                new[] { true, true, true, true, true, true, true },      // 8
                new[] { true, true, true, true, false, true, true }      // 9
            };

            int digitValue = digit - '0';
            if (digitValue >= 0 && digitValue <= 9)
            {
                for (int i = 0; i < 7; i++)
                {
                    segments[i].Fill = patterns[digitValue][i] ? onBrush : offBrush;
                }
            }
            else if (digit == ':' || digit == ' ')
            {
                // Turn off all segments for colon or space
                for (int i = 0; i < 7; i++)
                {
                    segments[i].Fill = offBrush;
                }
            }
        }
    }

    public class SevenSegmentColon : Canvas
    {
        public SevenSegmentColon()
        {
            Width = 20;
            Height = 60;

            Ellipse topDot = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0))
            };
            Canvas.SetLeft(topDot, 7);
            Canvas.SetTop(topDot, 17);
            Children.Add(topDot);

            Ellipse bottomDot = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0))
            };
            Canvas.SetLeft(bottomDot, 7);
            Canvas.SetTop(bottomDot, 37);
            Children.Add(bottomDot);
        }
    }
}