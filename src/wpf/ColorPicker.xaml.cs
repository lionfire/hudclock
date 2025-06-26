using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetricClock
{
    public partial class ColorPicker : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(System.Windows.Media.Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

        public System.Windows.Media.Color SelectedColor
        {
            get => (System.Windows.Media.Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private bool _isUpdating = false;

        public ColorPicker()
        {
            InitializeComponent();
            InitializeQuickColors();
            UpdateSlidersFromColor();
        }

        private void InitializeQuickColors()
        {
            var quickColors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Black),
                new SolidColorBrush(Colors.White),
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Cyan),
                new SolidColorBrush(Colors.Magenta),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.Purple),
                new SolidColorBrush(Colors.Pink),
                new SolidColorBrush(Colors.Gray),
                new SolidColorBrush(Colors.DarkGray),
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Brown),
                new SolidColorBrush(Colors.Lime),
                new SolidColorBrush(Colors.Navy),
                new SolidColorBrush(Colors.Teal),
                new SolidColorBrush(Colors.Indigo),
                new SolidColorBrush(Colors.Gold)
            };
            QuickColors.ItemsSource = quickColors;
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
            {
                picker.UpdateSlidersFromColor();
                picker.UpdateHexTextBox();
            }
        }

        private void UpdateSlidersFromColor()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            var color = SelectedColor;
            AlphaSlider.Value = color.A;
            RedSlider.Value = color.R;
            GreenSlider.Value = color.G;
            BlueSlider.Value = color.B;

            _isUpdating = false;
        }

        private void UpdateHexTextBox()
        {
            HexTextBox.Text = $"#{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            SelectedColor = System.Windows.Media.Color.FromArgb(
                (byte)AlphaSlider.Value,
                (byte)RedSlider.Value,
                (byte)GreenSlider.Value,
                (byte)BlueSlider.Value);

            UpdateHexTextBox();
            _isUpdating = false;
        }

        private void QuickColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.Tag is SolidColorBrush brush)
            {
                SelectedColor = brush.Color;
                ColorPopup.IsOpen = false;
            }
        }

        private void HexTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ParseHexColor();
        }

        private void HexTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ParseHexColor();
                ColorPopup.IsOpen = false;
            }
        }

        private void ParseHexColor()
        {
            try
            {
                var hex = HexTextBox.Text.Trim();
                if (!hex.StartsWith("#"))
                    hex = "#" + hex;

                if (hex.Length == 7) // #RRGGBB
                    hex = "#FF" + hex.Substring(1); // Add full alpha

                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
                SelectedColor = color;
            }
            catch
            {
                // Invalid color, reset to current
                UpdateHexTextBox();
            }
        }
    }

    // Value converter for displaying color as hex
    public class ColorToHexConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is System.Windows.Media.Color color)
            {
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return "#FFFFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Value converter for converting between string (hex) and Color
    public class StringToColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
                }
                catch
                {
                    return Colors.Black;
                }
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is System.Windows.Media.Color color)
            {
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return "#FF000000";
        }
    }
}