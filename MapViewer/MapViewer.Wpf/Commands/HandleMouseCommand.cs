using MapViewer.Core.Commands;
using MapViewer.Core.Models;
using MapViewer.Core.Stores;
using MapViewer.Core.ViewModels;
using MapViewer.Wpf.EventArgs;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MapViewer.Wpf.Commands
{

    /// <summary>
    /// Command for handling all map mouse inputs.
    /// </summary>
    /// <param name="_mapViewModel">ViewModel for the map view.</param>
    public class HandleMouseCommand : BaseCommand
    {
        private readonly SettingsStore _settingsStore;
        private readonly MapViewModel _mapViewModel;

        /// <summary>
        /// Last mouse position for circle drawing.
        /// </summary>
        private Point? _lastCirclePosition = null;

        /// <summary>
        /// Last mouse position for camera panning.
        /// </summary>
        private Point? _lastPanPosition = null;

        public HandleMouseCommand(SettingsStore settingsStore, MapViewModel mapViewModel)
        {
            _settingsStore = settingsStore;
            _mapViewModel = mapViewModel;
        }

        public override void Execute(object? parameter)
        {
            // Return if missing necessary data
            if (parameter == null || _mapViewModel.Camera == null || _mapViewModel.Map == null || _mapViewModel.Map.Data == null)
            {
                return;
            }

            // Get MapMouseEventArgs if available
            try
            {
                MapMouseEventArgs mapMouseEventArgs = (MapMouseEventArgs)parameter;
                UpdateCursor(mapMouseEventArgs, _mapViewModel.Camera);
                DrawCircle(mapMouseEventArgs, _mapViewModel.Map.Data, _mapViewModel.Camera);
                PanMap(mapMouseEventArgs, _mapViewModel.Camera);
                ZoomMap(mapMouseEventArgs, _mapViewModel.Map.Data, _mapViewModel.Camera);
                mapMouseEventArgs.MouseEventArgs.Handled = true;
            }
            catch (InvalidCastException e)
            {
                Debug.WriteLine($"AddCircleCommand called with unsupported parameter. {e}");
                return;
            }
        }

        /// <summary>
        /// Draw circle based on two mouse clicks - the first designates circle's center,
        /// and distance to the second click establishes the radius.
        /// </summary>
        /// <param name="mapMouseEventArgs">Mouse event args with a relative viewport click position.</param>
        /// <param name="mapData">Map data to determine circle's altitude.</param>
        /// <param name="camera">Camera for conversion of a position to the camera space.</param>
        private void DrawCircle(MapMouseEventArgs mapMouseEventArgs, MapData mapData, Camera camera)
        {
            // Redraw circle
            if (_lastCirclePosition.HasValue)
            {
                var newPosition = GetMapSystemPoint(mapMouseEventArgs, camera);
                //TODO Move constans to config
                _mapViewModel.Circle = new Circle(
                    new Vector2((float)_lastCirclePosition.Value.X, (float)_lastCirclePosition.Value.Y),
                    (float)Math.Sqrt(Math.Pow(newPosition.X - _lastCirclePosition.Value.X, 2) + Math.Pow(newPosition.Y - _lastCirclePosition.Value.Y, 2)),
                    mapData.MaxAltitude,
                    (float)((_settingsStore.Settings.CenterSize / mapMouseEventArgs.MapWidth) * camera.Width),
                    (float)((_settingsStore.Settings.LineWidth / mapMouseEventArgs.MapWidth) * camera.Width),
                    _settingsStore.Settings.SegmentCount
                    );
            }
            // Record circle center for drawing
            if (mapMouseEventArgs.MouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                _lastCirclePosition = _lastCirclePosition == null ? global::MapViewer.Wpf.Commands.HandleMouseCommand.GetMapSystemPoint(mapMouseEventArgs, camera) : null;
            }
        }

        /// <summary>
        /// Pan camera based on mouse movement.
        /// </summary>
        /// <param name="mapMouseEventArgs">Mouse event args with a relative viewport click position.</param>
        /// <param name="camera">Previous camera state.</param>
        private void PanMap(MapMouseEventArgs mapMouseEventArgs, Camera camera)
        {
            if (mapMouseEventArgs.MouseEventArgs.RightButton == MouseButtonState.Pressed)
            {
                _lastPanPosition = _lastPanPosition.HasValue ? _lastPanPosition : GetMapSystemPoint(mapMouseEventArgs, camera);
                Point newPosition = GetMapSystemPoint(mapMouseEventArgs, camera);
                var movement = new Vector2((float)(newPosition.X - _lastPanPosition.Value.X), (float)(newPosition.Y - _lastPanPosition.Value.Y));

                _mapViewModel.Camera = new Camera(camera.Width, camera.Position.X - movement.X, camera.Position.Y - movement.Y, camera.Position.Z);
            }
            else if (mapMouseEventArgs.MouseEventArgs.RightButton == MouseButtonState.Released)
            {
                _lastPanPosition = null;
            }
        }

        /// <summary>
        /// Zoom based on mouse scroll.
        /// It also redraws the circle to keep line 
        /// </summary>
        /// <param name="mapMouseEventArgs">Mouse event args with a scroll delta.</param>
        /// <param name="mapData">Map data to determine circle's altitude.</param>
        /// <param name="camera">Previous camera state.</param>
        private void ZoomMap(MapMouseEventArgs mapMouseEventArgs, MapData mapData, Camera camera)
        {
            if (mapMouseEventArgs.MouseEventArgs is MouseWheelEventArgs mouseWheelEventArgs)
            {
                // Zoom by adjusting camera width, adjust position to zoom towareds mouse
                // Do not let zoomed with get negative - it causes circle to disappear
                //TODO Move constans to config
                float zoomedWidth = (float)Math.Max(0.5, camera.Width - mouseWheelEventArgs.Delta * _settingsStore.Settings.ZoomSensitivity);
                Point mapPoint = GetMapSystemPoint(mapMouseEventArgs, camera);
                _mapViewModel.Camera = new Camera(zoomedWidth,
                    (float)(camera.Position.X + (mapPoint.X - camera.Position.X) * (1 - zoomedWidth / camera.Width)),
                    (float)(camera.Position.Y + (mapPoint.Y - camera.Position.Y) * (1 - zoomedWidth / camera.Width)),
                    camera.Position.Z);
                // Update Circle to keep line width and center size
                //TODO Move constans to config
                _mapViewModel.Circle = _mapViewModel.Circle == null ? null : new Circle(
                    _mapViewModel.Circle.Center,
                    _mapViewModel.Circle.Radius,
                    mapData.MaxAltitude,
                    (float)((_settingsStore.Settings.CenterSize / mapMouseEventArgs.MapWidth) * camera.Width),
                    (float)((_settingsStore.Settings.LineWidth / mapMouseEventArgs.MapWidth) * camera.Width),
                    _settingsStore.Settings.SegmentCount
                    );

            }
        }

        /// <summary>
        /// Update cursor reading.
        /// </summary>
        /// <param name="mapMouseEventArgs">Mouse event args with a relative viewport mouse position.</param>
        /// <param name="camera">Camera for conversion of a position to the camera space.</param>
        private void UpdateCursor(MapMouseEventArgs mapMouseEventArgs, Camera camera)
        {
            Point mapPoint = GetMapSystemPoint(mapMouseEventArgs, camera);
            _mapViewModel.UpdateCursor(new Vector2((float)mapPoint.X, (float)mapPoint.Y));
        }

        /// <summary>
        /// Get cursor postion in camera/map coordinate system.
        /// </summary>
        /// <param name="mapMouseEventArgs">Mouse event args with a relative viewport mouse position.</param>
        /// <param name="camera">Camera for conversion of a position to the camera space.</param>
        /// <returns>Cursor position in the camera space.</returns>
        private static Point GetMapSystemPoint(MapMouseEventArgs mapMouseEventArgs, Camera camera)
        {
            Matrix cameraTransform = new(camera.Width, 0, 0, camera.Width / mapMouseEventArgs.AspectRatio, camera.Position.X, camera.Position.Y);
            return mapMouseEventArgs.MapViewportPosition * cameraTransform;
        }
    }
}
