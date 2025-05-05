using MapViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraViewer.Core.Stores
{
    public class CameraStore
    {
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
    }
}
