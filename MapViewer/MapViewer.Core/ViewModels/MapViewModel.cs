using MapViewer.Core.Commands;
using MapViewer.Core.Exceptions;
using MapViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapViewer.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the map view.
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        private Map? _map;
        private bool _showMapOverlay = false;
        private string _mapOverlayText = "";
        private Circle? _circle;
        private Camera? _camera;
        private Vector3? _cursor;

        /// <summary>
        /// Map model for rendering.
        /// </summary>
        public Map? Map
        {
            get => _map;
            set
            {
                _map = value;
                OnPropertyChanged(nameof(Map));
                if (value is not null)
                {
                    Camera = new Camera(
                        value.Data.Width,
                        value.Data.XCenter,
                        value.Data.YCenter,
                        value.Data.MaxAltitude);
                } 
                else
                {
                    Camera = null;
                }
            }
        }

        /// <summary>
        /// Control field for a map overlay.
        /// </summary>
        public bool ShowMapOverlay
        {
            get => _showMapOverlay;
            set
            {
                _showMapOverlay = value;
                OnPropertyChanged(nameof(ShowMapOverlay));
            }
        }

        /// <summary>
        /// Text to display on the map overlay.
        /// </summary>
        public string MapOverlayText
        {
            get => _mapOverlayText;
            set
            {
                _mapOverlayText = value;
                OnPropertyChanged(nameof(MapOverlayText));
            }
        }

        /// <summary>
        /// Circle model for redering.
        /// </summary>
        public Circle? Circle
        {
            get => _circle;
            set
            {
                _circle = value;
                OnPropertyChanged(nameof(Circle));
            }
        }

        /// <summary>
        /// Camera model for rendering.
        /// </summary>
        public Camera? Camera {
            get => _camera;
            set
            {
                _camera = value;
                OnPropertyChanged(nameof(Camera));
            }
        }

        /// <summary>
        /// Cursor position to be displayed on the status panel.
        /// </summary>
        public Vector3? Cursor { 
            get => _cursor; 
            private set
            { 
                _cursor = value;
                OnPropertyChanged(nameof(Cursor));
            } 
        }

        /// <summary>
        /// Command for handling mouse events.
        /// </summary>
        public ICommand HandleMouseCommand { get; }

        /// <summary>
        /// Command for handling Keyboard events.
        /// </summary>
        public ICommand HandleKeyCommand { get; }

        /// <summary>
        /// Hierarchical menu model.
        /// </summary>
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        /// <summary>
        /// ViewModel for the map view.
        /// </summary>
        /// <param name="loadMapCommand">Function creating command for loading a map from a file.</param>
        /// <param name="saveMapCommand">Function creating commandfor saving a map to a file.</param>
        /// <param name="handleMouseCommand">Function creatin command for handling mouse commands.</param>
        /// <param name="handleKeyCommand">Function creatin command for handling keyboard commands.</param>
        public MapViewModel(Func<MapViewModel, ICommand> loadMapCommand, 
            Func<MapViewModel, ICommand> saveMapCommand,
            Func<MapViewModel, ICommand> handleMouseCommand,
            Func<MapViewModel, ICommand> handleKeyCommand)
        {
            HandleMouseCommand = handleMouseCommand(this);
            HandleKeyCommand = handleKeyCommand(this);
            MenuItems = [];
            SetupMenu(loadMapCommand(this), saveMapCommand(this), new ResetViewCommand(this));
        }

        /// <summary>
        /// Update Vector3D <see cref="Cursor"/> field based on a 2D location.
        /// </summary>
        /// <param name="cursor">2D location of a cursor</param>
        /// <returns>True if update was successful, or false otherwise.</returns>
        public bool UpdateCursor(Vector2 cursor)
        {
            if (Map is null) return false;
            try
            {
                Cursor = new Vector3(cursor.X, cursor.Y, Map.Data.GetAltitude(cursor));
                return true;
            } 
            catch (MapCoordinatesException e)
            {
                Debug.WriteLine("Cursor update failed - cursor out outside of the map.");
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Cursor update failed unexpectedly.");
                Debug.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// Setup hierarchical menu model.
        /// </summary>
        /// <param name="loadMapCommand">Command for loading a map from a file.</param>
        /// <param name="saveMapCommand">Command for saving a map to a file.</param>
        /// <param name="resetViewCommand">Command for reseting the <see cref="Camera"/>. </param>
        private void SetupMenu(ICommand loadMapCommand, ICommand saveMapCommand, ICommand resetViewCommand)
        {
            // map menu
            var mapMenu = new MenuItemViewModel("Map");
            var loadMap = new MenuItemViewModel("Load", loadMapCommand);
            mapMenu.MenuItems.Add(loadMap);
            var saveMap = new MenuItemViewModel("Save", saveMapCommand);
            mapMenu.MenuItems.Add(saveMap);
            MenuItems.Add(mapMenu);
            // view menu
            var viewMenu = new MenuItemViewModel("View");
            var resetView = new MenuItemViewModel("Reset", resetViewCommand);
            viewMenu.MenuItems.Add(resetView);
            MenuItems.Add(viewMenu);
        }
    }
}
