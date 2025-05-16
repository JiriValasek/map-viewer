using MapViewer.Core.Models;

namespace MapViewer.Core.Stores
{
    public class MapStore
    {
        // Map

        private Map? _currentMap;

        public Map? Map
        {
            get => _currentMap;
            set
            {
                _currentMap = value;
                OnCurrentMapChanged();
            }
        }

        public event Action? CurrentMapChanged;

        private void OnCurrentMapChanged()
        {
            CurrentMapChanged?.Invoke();
        }

        // Camera

        private Camera? _currentCamera;

        public Camera? Camera
        {
            get => _currentCamera;
            set
            {
                _currentCamera = value;
                OnCurrentCameraChanged();
            }
        }

        public event Action? CurrentCameraChanged;

        private void OnCurrentCameraChanged()
        {
            CurrentCameraChanged?.Invoke();
        }

        // Circle

        private Circle? _currentCircle;

        public Circle? Circle
        {
            get => _currentCircle;
            set
            {
                _currentCircle = value;
                OnCurrentCircleChanged();
            }
        }

        public event Action? CurrentCircleChanged;

        private void OnCurrentCircleChanged()
        {
            CurrentCircleChanged?.Invoke();
        }
    }
}
