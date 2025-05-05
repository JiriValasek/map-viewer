using MapViewer.Core.Commands;
using MapViewer.Core.Models;
using MapViewer.Core.Services;
using MapViewer.Core.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapViewer.Core.ViewModels
{
    public class SettingsViewModel : WindowViewModel, INotifyDataErrorInfo
    {
        private string FOREGROUND_SUFFIX = "Foreground";
        private string BACKGROUND_SUFFIX = "Background";


        private readonly SettingsStore _settingsStore;
        private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary;
        private string _minAltitudeColor, _maxAltitudeColor, _circleColor, _centerColor, _lineWidth, 
            _centerSize, _segmentCount, _zoomSensitivity, _rotationStep, _movementStep;
        private bool _isSaving;

        /// <summary>
        /// Title overwrite for the window's title.
        /// </summary>
        public override string Title { get => "MapViewer - Settings"; }

        public Settings Settings { get => _settingsStore.Settings; }

        // Map settings
        public string MinAltitudeColor 
        {
            get => _minAltitudeColor; 
            set
            {
                _minAltitudeColor = value;
                OnPropertyChanged(nameof(MinAltitudeColor));
                CheckColor(nameof(MinAltitudeColor));
            }
        }
        public Color MinAltitudeColorForeground { get; set; }
        public Color MinAltitudeColorBackground { get; set; }

        public string MaxAltitudeColor 
        {
            get => _maxAltitudeColor;
            set
            {
                _maxAltitudeColor = value;
                OnPropertyChanged(nameof(MaxAltitudeColor));
                CheckColor(nameof(MaxAltitudeColor));
            }
        }
        public Color MaxAltitudeColorForeground { get; set; }
        public Color MaxAltitudeColorBackground { get; set; }

        // Circle settings
        public string CircleColor
        {
            get => _circleColor;
            set
            {
                _circleColor = value;
                OnPropertyChanged(nameof(CircleColor));
                CheckColor(nameof(CircleColor));
            }
        }

        public Color CircleColorForeground { get; set; }
        public Color CircleColorBackground { get; set; }

        public string CenterColor
        {
            get => _centerColor;
            set
            {
                _centerColor = value;
                OnPropertyChanged(nameof(CenterColor));
                CheckColor(nameof(CenterColor));
            }
        }
        public Color CenterColorForeground { get; set; }
        public Color CenterColorBackground { get; set; }

        public string LineWidth 
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                OnPropertyChanged(nameof(LineWidth));
                CheckFloat(nameof(LineWidth));
            }
        }
        public string CenterSize 
        {
            get => _centerSize;
            set
            {
                _centerSize = value;
                OnPropertyChanged(nameof(CenterSize));
                CheckFloat(nameof(CenterSize));
            }
        }
        public string SegmentCount 
        {
            get => _segmentCount;
            set
            {
                _segmentCount = value;
                OnPropertyChanged(SegmentCount);
                CheckInt(nameof(SegmentCount));
            }
        }

        // Controls settings
        public string ZoomSensitivity 
        {
            get => _zoomSensitivity;
            set
            {
                _zoomSensitivity = value;
                OnPropertyChanged(nameof(ZoomSensitivity));
                CheckFloat(nameof(ZoomSensitivity));
            }
        }
        public string RotationStep 
        {
            get => _rotationStep;
            set
            {
                _rotationStep = value;
                OnPropertyChanged(nameof(RotationStep));
                CheckFloat(nameof(RotationStep));
            }
        }
        public string MovementStep 
        {
            get => _movementStep;
            set
            {
                _movementStep = value;
                OnPropertyChanged(nameof(MovementStep));
                CheckFloat(nameof(MovementStep));
            }
        }

        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                OnPropertyChanged(nameof(IsSaving));
            }
        }

        /// <summary>
        /// Command to navigate back to the map.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Command to save changes & navigate back to the map.
        /// </summary>
        public ICommand SaveCommand { get; }
        public bool CanSave => !HasErrors;

        // TOOD Language??

        public SettingsViewModel(SettingsStore settingsStore, NavigationService navigateToMap) 
        {
            _settingsStore = settingsStore;
            _minAltitudeColor = Settings.MinAltitudeColor.Name;
            _maxAltitudeColor = Settings.MaxAltitudeColor.Name;
            _circleColor = Settings.CircleColor.Name;
            _centerColor = Settings.CenterColor.Name;
            _lineWidth = Settings.LineWidth.ToString();
            _centerSize = Settings.CenterSize.ToString();
            _segmentCount = Settings.SegmentCount.ToString();
            _zoomSensitivity = Settings.ZoomSensitivity.ToString();
            _rotationStep = Settings.RotationStep.ToString();
            _movementStep = Settings.MovementStep.ToString();
            ShowColors(nameof(MinAltitudeColor), Settings.MinAltitudeColor);
            ShowColors(nameof(MaxAltitudeColor), Settings.MaxAltitudeColor);
            ShowColors(nameof(CircleColor), Settings.CircleColor);
            ShowColors(nameof(CenterColor), Settings.CenterColor);
            _propertyNameToErrorsDictionary = [];
            CancelCommand = new NavigateCommand(navigateToMap);
            SaveCommand = new SaveSettingsCommand(navigateToMap, this, settingsStore);
        }


        public bool HasErrors => _propertyNameToErrorsDictionary.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return String.IsNullOrWhiteSpace(propertyName) ? [] : _propertyNameToErrorsDictionary.GetValueOrDefault(propertyName, []) ;
        }

        private void CheckColor(string propertyName)
        {
            ClearErrors(propertyName);
            var colorString = GetPropStringByPropName(propertyName);
            Color color = Color.FromName(colorString); // Does not throw error for invalid names
            if (color.IsKnownColor)
            {
                ShowColors(propertyName, color);
                OnPropertyChanged(nameof(CanSave));
                return;
            }

            try
            {
                int a,r,g,b;
                switch (colorString.Length)
                {
                    case 3:
                        a = 0xff;
                        r = 0xff * Convert.ToInt32(colorString[0].ToString(), 16) / 0xf;
                        g = 0xff * Convert.ToInt32(colorString[1].ToString(), 16) / 0xf;
                        b = 0xff * Convert.ToInt32(colorString[2].ToString(), 16) / 0xf;
                        break;
                    case 4:
                        a = 0xff * Convert.ToInt32("0x" + colorString[0], 16) / 0xf;
                        r = 0xff * Convert.ToInt32("0x" + colorString[1], 16) / 0xf;
                        g = 0xff * Convert.ToInt32("0x" + colorString[2], 16) / 0xf;
                        b = 0xff * Convert.ToInt32("0x" + colorString[3], 16) / 0xf;
                        break;
                    case 6:
                        a = 0xff;
                        r = Convert.ToInt32("0x" + colorString.Substring(0, 2), 16);
                        g = Convert.ToInt32("0x" + colorString.Substring(2, 2), 16);
                        b = Convert.ToInt32("0x" + colorString.Substring(4, 2), 16);
                        break;
                    case 8:
                        a = Convert.ToInt32("0x" + colorString.Substring(0, 2), 16);
                        r = Convert.ToInt32("0x" + colorString.Substring(2, 2), 16);
                        g = Convert.ToInt32("0x" + colorString.Substring(4, 2), 16);
                        b = Convert.ToInt32("0x" + colorString.Substring(6, 2), 16);
                        color = Color.FromArgb(a, r, g, b); // Does throw errors for invalid values
                        break;
                    default:
                        a = -1;
                        r = -1;
                        g = -1;
                        b = -1;
                        break;
                }
                color = Color.FromArgb(a, r, g, b);
                ShowColors(propertyName, color);
                OnPropertyChanged(nameof(CanSave));
                return;
            }
            catch (Exception e) { }
            ShowColors(propertyName, null);
            AddError("Unknown color - use name starting with a capital letter or an ARGB hexadecimal value.", propertyName);
            OnPropertyChanged(nameof(CanSave));
        }

        private void CheckFloat(string propertyName)
        {
            ClearErrors(propertyName);
            var floatString = GetPropStringByPropName(propertyName);
            try
            {
                Single.Parse(floatString);
            } 
            catch
            {
                AddError("String must be a valid real number.", propertyName);
            }
            OnPropertyChanged(nameof(CanSave));
        }

        private void CheckInt(string propertyName)
        {
            ClearErrors(propertyName);
            var floatString = GetPropStringByPropName(propertyName);
            try
            {
                Int32.Parse(floatString);
            }
            catch
            {
                AddError("Number must be an integer.", propertyName);
            }
            OnPropertyChanged(nameof(CanSave));
        }

        private void ShowColors(string propertyName, Color? color)
        {
            if (color is null)
            {
                this.GetType().GetProperty(propertyName + BACKGROUND_SUFFIX)?.SetValue(this, Color.Transparent);
                this.GetType().GetProperty(propertyName + FOREGROUND_SUFFIX)?.SetValue(this, Color.Red);
            }
            else
            {
                this.GetType().GetProperty(propertyName + BACKGROUND_SUFFIX)?.SetValue(this, color.Value);
                this.GetType().GetProperty(propertyName + FOREGROUND_SUFFIX)?.SetValue(this, color.Value.GetBrightness() > 0.5 ? Color.Black : Color.White);
            }
            OnPropertyChanged(propertyName + BACKGROUND_SUFFIX);
            OnPropertyChanged(propertyName + FOREGROUND_SUFFIX);
        }

        public string GetPropStringByPropName(string propertyName)
        {
            object? value = this.GetType().GetProperty(propertyName)?.GetValue(this, null);
            if (value is string stringValue)
            {
                return stringValue;
            }
            Debug.WriteLine($"Cannot get color string from {propertyName}.");
            return "";
        }

        private void AddError(string errorMessage, string propertyName)
        {
            if (!_propertyNameToErrorsDictionary.ContainsKey(propertyName))
            {
                _propertyNameToErrorsDictionary.Add(propertyName, []);
            }

            _propertyNameToErrorsDictionary[propertyName].Add(errorMessage);

            OnErrorsChanged(propertyName);
        }

        private void ClearErrors(string propertyName)
        {
            _propertyNameToErrorsDictionary.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public override void Dispose(){}
    }
}
