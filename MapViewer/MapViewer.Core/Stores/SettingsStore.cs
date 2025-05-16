using MapViewer.Core.Models;

namespace MapViewer.Core.Stores
{
    public class SettingsStore
    {
        private Settings _currentSettings;
        private Settings _previousSettings;

        public Settings Settings
        {
            get => _currentSettings;
            set
            {
                _previousSettings = _currentSettings;
                _currentSettings = value;
                OnCurrentSettingsChanged();
            }
        }

        public Settings PreviousSettings
        {
            get => _previousSettings;
        }

        public SettingsStore(Settings settings)
        {
            _currentSettings = settings;
            // At the start there are no changes so previous settings is the same as the current one
            _previousSettings = settings; 
        }

        public event Action? CurrentSettingsChanged;

        private void OnCurrentSettingsChanged()
        {
            CurrentSettingsChanged?.Invoke();
        }
    }
}
