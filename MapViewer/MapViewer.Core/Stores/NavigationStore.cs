using MapViewer.Core.ViewModels;

namespace MapViewer.Core.Stores
{
    public class NavigationStore()
    {
        private WindowViewModel? _currentViewModel;

        public WindowViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public event Action? CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
