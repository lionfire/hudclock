using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace MetricClock
{
    public class ClockSettings : INotifyPropertyChanged
    {
        private static ClockSettings? _instance;
        public static ClockSettings Instance => _instance ??= new ClockSettings();
        
        private bool _isLoading = false;
        private bool _isInitialized = false;

        private double _opacity = 0.8;
        private double _backdropOpacity = 0.8;
        private double _clockElementOpacity = 1.0;
        private DigitalClockStyle _digitalStyle = DigitalClockStyle.Modern;
        private AnalogClockStyle _analogStyle = AnalogClockStyle.Traditional;
        private ClockShape _clockShape = ClockShape.Circle;
        private bool _showSecondsHand = true;
        private bool _smoothSecondsHand = false;
        private bool _showNumbersInCircles = false;
        private bool _showNormalTimeInMetric = true;
        private bool _showDate = false;
        private bool _showMinutes = true;
        private bool _showCenterDot = false;
        private bool _hideFromTaskbarNormal = false;
        private bool _hideFromTaskbarClickThrough = true;
        private bool _showTrayIconNormal = true;
        private bool _showTrayIconClickThrough = true;
        private bool _showClickThroughNotice = true;
        private ClockDisplayType _clockDisplayType = ClockDisplayType.Analog;
        private TimeFormat _timeFormat = TimeFormat.TwelveHour;
        private TimeSystem _timeSystem = TimeSystem.Standard;
        private double _windowLeft = 100;
        private double _windowTop = 100;
        private double _windowWidth = 350;
        private double _windowHeight = 350;
        private bool _windowMaximized = false;
        private double _elementScale = 1.0;
        private int _updatesPerSecond = 60;
        private string _digitalFontFamily = "Segoe UI";
        private double _digitalFontSize = 48;
        private string _digitalFontWeight = "Normal";
        private string _analogFontFamily = "Segoe UI";
        private double _analogFontSize = 14;
        private string _analogFontWeight = "Normal";
        private bool _runAtStartup = false;
        private bool _isClickThrough = false;

        public double Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged();
                }
            }
        }

        public DigitalClockStyle DigitalStyle
        {
            get => _digitalStyle;
            set
            {
                if (_digitalStyle != value)
                {
                    _digitalStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        public AnalogClockStyle AnalogStyle
        {
            get => _analogStyle;
            set
            {
                if (_analogStyle != value)
                {
                    _analogStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClockShape ClockShape
        {
            get => _clockShape;
            set
            {
                if (_clockShape != value)
                {
                    _clockShape = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowSecondsHand
        {
            get => _showSecondsHand;
            set
            {
                if (_showSecondsHand != value)
                {
                    _showSecondsHand = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool SmoothSecondsHand
        {
            get => _smoothSecondsHand;
            set
            {
                if (_smoothSecondsHand != value)
                {
                    _smoothSecondsHand = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowNumbersInCircles
        {
            get => _showNumbersInCircles;
            set
            {
                if (_showNumbersInCircles != value)
                {
                    _showNumbersInCircles = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowNormalTimeInMetric
        {
            get => _showNormalTimeInMetric;
            set
            {
                if (_showNormalTimeInMetric != value)
                {
                    _showNormalTimeInMetric = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowDate
        {
            get => _showDate;
            set
            {
                if (_showDate != value)
                {
                    _showDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowMinutes
        {
            get => _showMinutes;
            set
            {
                if (_showMinutes != value)
                {
                    _showMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowCenterDot
        {
            get => _showCenterDot;
            set
            {
                if (_showCenterDot != value)
                {
                    _showCenterDot = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HideFromTaskbarNormal
        {
            get => _hideFromTaskbarNormal;
            set
            {
                if (_hideFromTaskbarNormal != value)
                {
                    _hideFromTaskbarNormal = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HideFromTaskbarClickThrough
        {
            get => _hideFromTaskbarClickThrough;
            set
            {
                if (_hideFromTaskbarClickThrough != value)
                {
                    _hideFromTaskbarClickThrough = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowTrayIconNormal
        {
            get => _showTrayIconNormal;
            set
            {
                if (_showTrayIconNormal != value)
                {
                    _showTrayIconNormal = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowTrayIconClickThrough
        {
            get => _showTrayIconClickThrough;
            set
            {
                if (_showTrayIconClickThrough != value)
                {
                    _showTrayIconClickThrough = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowClickThroughNotice
        {
            get => _showClickThroughNotice;
            set
            {
                if (_showClickThroughNotice != value)
                {
                    _showClickThroughNotice = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ClockElementOpacity
        {
            get => _clockElementOpacity;
            set
            {
                if (_clockElementOpacity != value)
                {
                    _clockElementOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        public double BackdropOpacity
        {
            get => _backdropOpacity;
            set
            {
                if (_backdropOpacity != value)
                {
                    _backdropOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        public ClockDisplayType ClockDisplayType
        {
            get => _clockDisplayType;
            set
            {
                if (_clockDisplayType != value)
                {
                    _clockDisplayType = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeFormat TimeFormat
        {
            get => _timeFormat;
            set
            {
                if (_timeFormat != value)
                {
                    _timeFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSystem TimeSystem
        {
            get => _timeSystem;
            set
            {
                if (_timeSystem != value)
                {
                    _timeSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowLeft
        {
            get => _windowLeft;
            set
            {
                if (_windowLeft != value)
                {
                    _windowLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowTop
        {
            get => _windowTop;
            set
            {
                if (_windowTop != value)
                {
                    _windowTop = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value)
                {
                    _windowWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                if (_windowHeight != value)
                {
                    _windowHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool WindowMaximized
        {
            get => _windowMaximized;
            set
            {
                if (_windowMaximized != value)
                {
                    _windowMaximized = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ElementScale
        {
            get => _elementScale;
            set
            {
                if (_elementScale != value)
                {
                    _elementScale = Math.Max(0.5, Math.Min(10.0, value)); // Clamp between 0.5 and 10.0
                    OnPropertyChanged();
                }
            }
        }

        public int UpdatesPerSecond
        {
            get => _updatesPerSecond;
            set
            {
                if (_updatesPerSecond != value)
                {
                    _updatesPerSecond = Math.Max(1, Math.Min(120, value)); // Clamp between 1 and 120
                    OnPropertyChanged();
                }
            }
        }

        public string DigitalFontFamily
        {
            get => _digitalFontFamily;
            set
            {
                if (_digitalFontFamily != value)
                {
                    _digitalFontFamily = value;
                    OnPropertyChanged();
                }
            }
        }

        public double DigitalFontSize
        {
            get => _digitalFontSize;
            set
            {
                if (_digitalFontSize != value)
                {
                    _digitalFontSize = Math.Max(10, Math.Min(200, value)); // Clamp between 10 and 200
                    OnPropertyChanged();
                }
            }
        }

        public string DigitalFontWeight
        {
            get => _digitalFontWeight;
            set
            {
                if (_digitalFontWeight != value)
                {
                    _digitalFontWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AnalogFontFamily
        {
            get => _analogFontFamily;
            set
            {
                if (_analogFontFamily != value)
                {
                    _analogFontFamily = value;
                    OnPropertyChanged();
                }
            }
        }

        public double AnalogFontSize
        {
            get => _analogFontSize;
            set
            {
                if (_analogFontSize != value)
                {
                    _analogFontSize = Math.Max(8, Math.Min(72, value)); // Clamp between 8 and 72
                    OnPropertyChanged();
                }
            }
        }

        public string AnalogFontWeight
        {
            get => _analogFontWeight;
            set
            {
                if (_analogFontWeight != value)
                {
                    _analogFontWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunAtStartup
        {
            get => _runAtStartup;
            set
            {
                if (_runAtStartup != value)
                {
                    _runAtStartup = value;
                    OnPropertyChanged();
                    UpdateStartupRegistry(value);
                }
            }
        }

        public bool IsClickThrough
        {
            get => _isClickThrough;
            set
            {
                if (_isClickThrough != value)
                {
                    _isClickThrough = value;
                    OnPropertyChanged();
                }
            }
        }

        // Helper property to get the effective clock type
        public ClockType GetEffectiveClockType()
        {
            if (_clockDisplayType == ClockDisplayType.Analog)
            {
                if (_timeSystem == TimeSystem.Metric)
                    return ClockType.AnalogMetric;
                else
                    return _timeFormat == TimeFormat.TwentyFourHour ? ClockType.Analog24 : ClockType.Analog;
            }
            else
            {
                if (_timeSystem == TimeSystem.Metric)
                    return ClockType.Metric;
                else
                    return _timeFormat == TimeFormat.TwentyFourHour ? ClockType.Digital24 : ClockType.Digital;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string GetSettingsFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = Path.Combine(appDataPath, "HudClock");
            var settingsFile = Path.Combine(settingsDir, "settings.json");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(settingsDir);
            
            return settingsFile;
        }

        public void SaveSettings()
        {
            // Don't save during loading or before initialization
            if (_isLoading || !_isInitialized) 
            {
                System.Diagnostics.Debug.WriteLine($"Skipping save - Loading: {_isLoading}, Initialized: {_isInitialized}");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"Saving settings - Opacity: {_opacity}, ElementScale: {_elementScale}, AnalogStyle: {_analogStyle}");
            
            try
            {
                var settings = new
                {
                    Opacity = _opacity,
                    BackdropOpacity = _backdropOpacity,
                    ClockElementOpacity = _clockElementOpacity,
                    DigitalStyle = (int)_digitalStyle,
                    AnalogStyle = (int)_analogStyle,
                    ClockShape = (int)_clockShape,
                    ShowSecondsHand = _showSecondsHand,
                    SmoothSecondsHand = _smoothSecondsHand,
                    ShowNumbersInCircles = _showNumbersInCircles,
                    ShowNormalTimeInMetric = _showNormalTimeInMetric,
                    ShowDate = _showDate,
                    ShowMinutes = _showMinutes,
                    ShowCenterDot = _showCenterDot,
                    ClockDisplayType = (int)_clockDisplayType,
                    TimeFormat = (int)_timeFormat,
                    TimeSystem = (int)_timeSystem,
                    WindowLeft = _windowLeft,
                    WindowTop = _windowTop,
                    WindowWidth = _windowWidth,
                    WindowHeight = _windowHeight,
                    WindowMaximized = _windowMaximized,
                    HideFromTaskbarNormal = _hideFromTaskbarNormal,
                    HideFromTaskbarClickThrough = _hideFromTaskbarClickThrough,
                    ShowTrayIconNormal = _showTrayIconNormal,
                    ShowTrayIconClickThrough = _showTrayIconClickThrough,
                    ShowClickThroughNotice = _showClickThroughNotice,
                    ElementScale = _elementScale,
                    UpdatesPerSecond = _updatesPerSecond,
                    DigitalFontFamily = _digitalFontFamily,
                    DigitalFontSize = _digitalFontSize,
                    DigitalFontWeight = _digitalFontWeight,
                    AnalogFontFamily = _analogFontFamily,
                    AnalogFontSize = _analogFontSize,
                    AnalogFontWeight = _analogFontWeight,
                    RunAtStartup = _runAtStartup,
                    IsClickThrough = _isClickThrough
                };
                
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetSettingsFilePath(), json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        public void LoadSettings()
        {
            _isLoading = true;
            try
            {
                var settingsFile = GetSettingsFilePath();
                System.Diagnostics.Debug.WriteLine($"Loading settings from: {settingsFile}");
                
                if (File.Exists(settingsFile))
                {
                    var json = File.ReadAllText(settingsFile);
                    System.Diagnostics.Debug.WriteLine($"Settings content: {json.Substring(0, Math.Min(200, json.Length))}...");
                    
                    using var document = JsonDocument.Parse(json);
                    var root = document.RootElement;
                    
                    if (root.TryGetProperty("Opacity", out var opacityElement))
                        Opacity = opacityElement.GetDouble();
                    
                    if (root.TryGetProperty("BackdropOpacity", out var backdropOpacityElement))
                        BackdropOpacity = backdropOpacityElement.GetDouble();
                    
                    if (root.TryGetProperty("ClockElementOpacity", out var clockOpacityElement))
                        ClockElementOpacity = clockOpacityElement.GetDouble();
                    
                    if (root.TryGetProperty("DigitalStyle", out var digitalStyleElement))
                        DigitalStyle = (DigitalClockStyle)digitalStyleElement.GetInt32();
                    
                    if (root.TryGetProperty("AnalogStyle", out var analogStyleElement))
                        AnalogStyle = (AnalogClockStyle)analogStyleElement.GetInt32();
                    
                    if (root.TryGetProperty("ClockShape", out var clockShapeElement))
                        ClockShape = (ClockShape)clockShapeElement.GetInt32();
                    
                    if (root.TryGetProperty("ShowSecondsHand", out var showSecondsElement))
                        ShowSecondsHand = showSecondsElement.GetBoolean();
                    
                    if (root.TryGetProperty("SmoothSecondsHand", out var smoothSecondsElement))
                        SmoothSecondsHand = smoothSecondsElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowNumbersInCircles", out var showNumbersElement))
                        ShowNumbersInCircles = showNumbersElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowNormalTimeInMetric", out var showNormalTimeElement))
                        ShowNormalTimeInMetric = showNormalTimeElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowDate", out var showDateElement))
                        ShowDate = showDateElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowMinutes", out var showMinutesElement))
                        ShowMinutes = showMinutesElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowCenterDot", out var showCenterDotElement))
                        ShowCenterDot = showCenterDotElement.GetBoolean();
                    
                    if (root.TryGetProperty("ClockDisplayType", out var displayTypeElement))
                        ClockDisplayType = (ClockDisplayType)displayTypeElement.GetInt32();
                    
                    if (root.TryGetProperty("TimeFormat", out var timeFormatElement))
                        TimeFormat = (TimeFormat)timeFormatElement.GetInt32();
                    
                    if (root.TryGetProperty("TimeSystem", out var timeSystemElement))
                        TimeSystem = (TimeSystem)timeSystemElement.GetInt32();
                    
                    if (root.TryGetProperty("WindowLeft", out var windowLeftElement))
                        WindowLeft = windowLeftElement.GetDouble();
                    
                    if (root.TryGetProperty("WindowTop", out var windowTopElement))
                        WindowTop = windowTopElement.GetDouble();
                    
                    if (root.TryGetProperty("WindowWidth", out var windowWidthElement))
                        WindowWidth = windowWidthElement.GetDouble();
                    
                    if (root.TryGetProperty("WindowHeight", out var windowHeightElement))
                        WindowHeight = windowHeightElement.GetDouble();
                    
                    if (root.TryGetProperty("WindowMaximized", out var windowMaximizedElement))
                        WindowMaximized = windowMaximizedElement.GetBoolean();
                    
                    if (root.TryGetProperty("HideFromTaskbarNormal", out var hideTaskbarNormalElement))
                        HideFromTaskbarNormal = hideTaskbarNormalElement.GetBoolean();
                    
                    if (root.TryGetProperty("HideFromTaskbarClickThrough", out var hideTaskbarClickThroughElement))
                        HideFromTaskbarClickThrough = hideTaskbarClickThroughElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowTrayIconNormal", out var showTrayNormalElement))
                        ShowTrayIconNormal = showTrayNormalElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowTrayIconClickThrough", out var showTrayClickThroughElement))
                        ShowTrayIconClickThrough = showTrayClickThroughElement.GetBoolean();
                    
                    if (root.TryGetProperty("ShowClickThroughNotice", out var showClickThroughNoticeElement))
                        ShowClickThroughNotice = showClickThroughNoticeElement.GetBoolean();
                    
                    if (root.TryGetProperty("ElementScale", out var elementScaleElement))
                        ElementScale = elementScaleElement.GetDouble();
                    
                    if (root.TryGetProperty("UpdatesPerSecond", out var updatesPerSecondElement))
                        UpdatesPerSecond = updatesPerSecondElement.GetInt32();
                    
                    if (root.TryGetProperty("DigitalFontFamily", out var digitalFontFamilyElement))
                        DigitalFontFamily = digitalFontFamilyElement.GetString() ?? "Segoe UI";
                    
                    if (root.TryGetProperty("DigitalFontSize", out var digitalFontSizeElement))
                        DigitalFontSize = digitalFontSizeElement.GetDouble();
                    
                    if (root.TryGetProperty("DigitalFontWeight", out var digitalFontWeightElement))
                        DigitalFontWeight = digitalFontWeightElement.GetString() ?? "Normal";
                    
                    if (root.TryGetProperty("AnalogFontFamily", out var analogFontFamilyElement))
                        AnalogFontFamily = analogFontFamilyElement.GetString() ?? "Segoe UI";
                    
                    if (root.TryGetProperty("AnalogFontSize", out var analogFontSizeElement))
                        AnalogFontSize = analogFontSizeElement.GetDouble();
                    
                    if (root.TryGetProperty("AnalogFontWeight", out var analogFontWeightElement))
                        AnalogFontWeight = analogFontWeightElement.GetString() ?? "Normal";
                    
                    if (root.TryGetProperty("RunAtStartup", out var runAtStartupElement))
                        _runAtStartup = runAtStartupElement.GetBoolean();
                    
                    if (root.TryGetProperty("IsClickThrough", out var isClickThroughElement))
                        _isClickThrough = isClickThroughElement.GetBoolean();
                }
                
                // Always check the actual registry status
                CheckStartupStatus();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                // Use defaults if loading fails
            }
            finally
            {
                _isLoading = false;
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("Settings loaded and initialized");
            }
        }
        
        private void UpdateStartupRegistry(bool runAtStartup)
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        if (runAtStartup)
                        {
                            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
                            if (!string.IsNullOrEmpty(appPath))
                            {
                                key.SetValue("HudClock", $"\"{appPath}\"");
                            }
                        }
                        else
                        {
                            key.DeleteValue("HudClock", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating startup registry: {ex.Message}");
            }
        }
        
        public void CheckStartupStatus()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("HudClock");
                        _runAtStartup = value != null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking startup registry: {ex.Message}");
                _runAtStartup = false;
            }
        }
    }

    public enum DigitalClockStyle
    {
        Modern,
        SevenSegment
    }

    public enum AnalogClockStyle
    {
        Traditional,
        Circles,
        CirclesWithHands
    }

    public enum ClockDisplayType
    {
        Analog,
        Digital
    }

    public enum TimeFormat
    {
        TwelveHour,
        TwentyFourHour
    }

    public enum TimeSystem
    {
        Standard,
        Metric
    }

    public enum ClockShape
    {
        Circle,
        ProjectedRectangle,
        RectanglePath
    }
}