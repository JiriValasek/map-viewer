using MapViewer.Core.Commands;
using MapViewer.Core.Exceptions;
using MapViewer.Core.Models;
using MapViewer.Core.Services;
using MapViewer.Core.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Input;

namespace MapViewer.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the map view.
    /// </summary>
    public class MapViewModel : WindowViewModel
    {
        private readonly MapStore _mapStore;
        private readonly SettingsStore _settingsStore;
        private bool _showMapOverlay = false;
        private string _mapOverlayText = "";
        private Vector3? _cursor;

        /// <summary>
        /// Title overwrite for the window's title.
        /// </summary>
        public override string Title { get => "MapViewer - Map"; }

        /// <summary>
        /// Map model for rendering.
        /// </summary>
        public Map? Map
        {
            get => _mapStore.Map;
            set
            {
                _mapStore.Map = value;
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
            get => _mapStore.Circle;
            set
            {
                _mapStore.Circle = value;
                OnPropertyChanged(nameof(Circle));
            }
        }

        /// <summary>
        /// Camera model for rendering.
        /// </summary>
        public Camera? Camera
        {
            get => _mapStore.Camera;
            set
            {
                _mapStore.Camera = value;
                OnPropertyChanged(nameof(Camera));
            }
        }

        /// <summary>
        /// Cursor position to be displayed on the status panel.
        /// </summary>
        public Vector3? Cursor
        {
            get => _cursor;
            private set
            {
                _cursor = value;
                OnPropertyChanged(nameof(Cursor));
            }
        }

        public Color MinAltitudeColor
        {
            get
            {
                return _settingsStore.Settings.MinAltitudeColor;
            }
        }

        public Color MaxAltitudeColor
        {
            get
            {
                return _settingsStore.Settings.MaxAltitudeColor;
            }
        }

        public Color CircleColor
        {
            get => _settingsStore.Settings.CircleColor;
        }

        public Color CenterColor
        {
            get => _settingsStore.Settings.CenterColor;
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
        public MapViewModel(
            MapStore mapStore,
            SettingsStore settingsStore,
            NavigationService navigateToSettings,
            Func<MapViewModel, ICommand> loadMapCommand,
            Func<MapViewModel, ICommand> saveMapCommand,
            Func<MapViewModel, ICommand> handleMouseCommand,
            Func<MapViewModel, ICommand> handleKeyCommand)
        {
            _mapStore = mapStore;
            _settingsStore = settingsStore;
            _settingsStore.CurrentSettingsChanged += OnCurrentSettingsChanged;
            HandleMouseCommand = handleMouseCommand(this);
            HandleKeyCommand = handleKeyCommand(this);
            MenuItems = [];
            SetupMenu(loadMapCommand(this), saveMapCommand(this), new NavigateCommand(navigateToSettings), new ResetViewCommand(this));
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
        private void SetupMenu(ICommand loadMapCommand, ICommand saveMapCommand, ICommand settingsCommand, ICommand resetViewCommand)
        {
            // map menu
            var mapMenu = new MenuItemViewModel("Map");
            var loadMap = new MenuItemViewModel("Load", loadMapCommand);
            mapMenu.MenuItems.Add(loadMap);
            var saveMap = new MenuItemViewModel("Save", saveMapCommand);
            mapMenu.MenuItems.Add(saveMap);
            var settigns = new MenuItemViewModel("Settings", settingsCommand);
            mapMenu.MenuItems.Add(settigns);
            MenuItems.Add(mapMenu);
            // view menu
            var viewMenu = new MenuItemViewModel("View");
            var resetView = new MenuItemViewModel("Reset", resetViewCommand);
            viewMenu.MenuItems.Add(resetView);
            MenuItems.Add(viewMenu);
        }

        private void OnCurrentSettingsChanged()
        {
            OnPropertyChanged(nameof(MinAltitudeColor));
            OnPropertyChanged(nameof(MaxAltitudeColor));
            OnPropertyChanged(nameof(CircleColor));
            OnPropertyChanged(nameof(CenterColor));
        }


        public override void Dispose()
        {
            _settingsStore.CurrentSettingsChanged -= OnCurrentSettingsChanged;
        }
    }
}
