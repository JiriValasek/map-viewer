using MapViewer.Core.Models;

namespace MapViewer.Core.Stores
{
    public class SettingsStore
    {
        private Settings _currentSettings;

        public Settings Settings
        {
            get => _currentSettings;
            set
            {
                _currentSettings = value;
                OnCurrentSettingsChanged();
            }
        }

        public SettingsStore(Settings settings)
        {
            _currentSettings = settings;
        }

        public event Action? CurrentSettingsChanged;

        private void OnCurrentSettingsChanged()
        {
            CurrentSettingsChanged?.Invoke();
        }
    }
}
