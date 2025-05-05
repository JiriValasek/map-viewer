using MapViewer.Core.Stores;
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
    public class MainViewModel : WindowViewModel
    {
        private readonly NavigationStore _navigationStore;

        /// <summary>
        /// Title overwrite for the window's title.
        /// </summary>
        public override string Title
        {
            get => CurrentViewModel != null && !String.IsNullOrEmpty(CurrentViewModel.Title) ?
                    CurrentViewModel.Title :
                    "MapViewer";
        }

        public WindowViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        }
    }
}
