using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapViewer.Core.ViewModels
{
    /// <summary>
    /// ViewModel for a hierarchical menu structure.
    /// </summary>
    public class MenuItemViewModel : BaseViewModel
    {
        /// <summary>
        /// Label on a menu item.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Command to be executed on clicking menu item.
        /// </summary>
        public ICommand? Command { get; }

        /// <summary>
        /// Collection of models for submenuitems.
        /// </summary>
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        /// <summary>
        /// ViewModel for a hierarchical menu structure.
        /// </summary>
        /// <param name="displayName">Label on a menu item.</param>
        public MenuItemViewModel(string displayName)
        {
            DisplayName = displayName;
            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }

        /// <summary>
        /// ViewModel for a hierarchical menu structure.
        /// </summary>
        /// <param name="displayName">Label on a menu item.</param>
        /// <param name="command">Command to be executed on clicking menu item.</param>
        public MenuItemViewModel(string displayName, ICommand command)
        {
            DisplayName = displayName;
            Command = command;
            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }

    }
}
