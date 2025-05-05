using MapViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
