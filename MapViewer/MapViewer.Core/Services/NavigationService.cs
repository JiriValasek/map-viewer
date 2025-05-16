using MapViewer.Core.Stores;
using MapViewer.Core.ViewModels;

namespace MapViewer.Core.Services
{
    public class NavigationService
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<WindowViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<WindowViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
