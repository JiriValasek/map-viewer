using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapViewer.Core.ViewModels
{
    /// <summary>
    /// Main view model for navigation.
    /// </summary>
    /// <param name="loadMapCommand">Command to load map from a file to the <see cref="MapViewModel"/>.</param>
    /// <param name="saveMapCommand">Command to save map from the <see cref="MapViewModel"/> to a file.</param>
    /// <param name="handleMouseCommand">Command for all mouse interactions.</param>
    /// <param name="handleKeyCommand">Command for all keyboard interactions.</param>
    public class MainViewModel(
        Func<MapViewModel, ICommand> loadMapCommand,
        Func<MapViewModel, ICommand> saveMapCommand,
        Func<MapViewModel, ICommand> handleMouseCommand,
        Func<MapViewModel, ICommand> handleKeyCommand) : BaseViewModel
    {
        public BaseViewModel CurrentViewModel { get; } = new MapViewModel(loadMapCommand, saveMapCommand, handleMouseCommand, handleKeyCommand);
    }
}
