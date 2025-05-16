using MapViewer.Core.Commands;
using MapViewer.Core.Exceptions;
using MapViewer.Core.Models;
using MapViewer.Core.Utils;
using MapViewer.Core.ViewModels;
using Microsoft.Win32;
using System.Windows.Threading;

namespace MapViewer.Wpf.Commands
{

    /// <summary>
    /// Command for map loading.
    /// </summary>
    /// <param name="mapViewModel">ViewModel for the map view.</param>
    public class LoadMapCommand(MapViewModel mapViewModel) : BaseAsyncCommand
    {

        /// <summary>
        /// Modulus for <see cref="_dotCounter"/>.
        /// </summary>
        private readonly int MAX_DOT_COUNT_MODULUS = 4;

        /// <summary>
        /// Default text to show while loading.
        /// </summary>
        private readonly string LOADING_TEXT = "Map is loading";

        /// <summary>
        /// Counter for loading dots.
        /// </summary>
        private int _dotCounter = 1;

        /// <summary>
        /// Timer for loading dots.
        /// </summary>
        private readonly DispatcherTimer loadingTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(250),
        };

        public override async Task ExecuteAsync(object? parameter)
        {
            OpenFileDialog openDialog = new()
            {
                Filter = null,
                RestoreDirectory = true,
                Multiselect = false,
                CheckPathExists = true,
                CheckFileExists = true,
                ValidateNames = true,
                AddToRecent = true,
            };

            if (openDialog.ShowDialog() == true)
            {
                ShowLoadingText();
                mapViewModel.Map = null;
                mapViewModel.Circle = null;
                try
                {
                    MapData mapData = await Dispatcher.CurrentDispatcher.Invoke(
                        async () =>
                        {
                            return await MapFileUtils.LoadMapAsync(openDialog.FileName);
                        },
                        DispatcherPriority.Background);
                    mapViewModel.Map = new Map(mapData);
                    HideLoadingText();
                }
                catch (MapFileException e)
                {
                    ShowError(e);
                }

            }
        }

        /// <summary>
        /// Show loading text, start timer for adding dots.
        /// </summary>
        private void ShowLoadingText()
        {
            loadingTimer.Tick += OnLoadingTimerEvent;
            loadingTimer.Start();
            mapViewModel.MapOverlayText = LOADING_TEXT.PadRight(LOADING_TEXT.Length + MAX_DOT_COUNT_MODULUS); ;
            mapViewModel.ShowMapOverlay = true;
        }

        /// <summary>
        /// Hide loading text, stop timer for adding dots.
        /// </summary>
        private void HideLoadingText()
        {
            mapViewModel.MapOverlayText = "";
            mapViewModel.ShowMapOverlay = false;
            loadingTimer.Tick -= OnLoadingTimerEvent;
            loadingTimer?.Stop();
        }

        /// <summary>
        /// Show error that occured while loading a file
        /// </summary>
        /// <param name="e">Error that occurred.</param>
        private void ShowError(Exception e)
        {
            loadingTimer.Tick -= OnLoadingTimerEvent;
            loadingTimer?.Stop();
            mapViewModel.MapOverlayText = e is MapFileException me ?
                $"Line {me.OffendingLine}: {me.Message}" : e.Message;
            mapViewModel.ShowMapOverlay = true;
        }

        /// <summary>
        /// Handler for DispatchTimer adding dots.
        /// </summary>
        private void OnLoadingTimerEvent(object? sender, System.EventArgs e)
        {
            mapViewModel.MapOverlayText = (LOADING_TEXT + new String('.', _dotCounter)).PadRight(LOADING_TEXT.Length + MAX_DOT_COUNT_MODULUS);
            _dotCounter = (_dotCounter + 1) % MAX_DOT_COUNT_MODULUS;
        }

    }

}
