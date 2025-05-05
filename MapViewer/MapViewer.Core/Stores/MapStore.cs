using MapViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Stores
{
    public class MapStore
    {
        private Map? _currentMap;

        public Map? Map
        {
            get => _currentMap;
            set {
                _currentMap = value;
                OnCurrentMapChanged();
            }
        }

        public event Action? CurrentMapChanged;

        private void OnCurrentMapChanged()
        {
            CurrentMapChanged?.Invoke();
        }
    }
}
