using MapViewer.Wpf.Commands;
using MapViewer.Wpf.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapViewer.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MapProjectionView.xaml
    /// </summary>
    public partial class MapViewportView : UserControl
    {
        private Window? _window = null;

        /// <summary>
        /// Dependency property for all mouse commands - zoom, pan, altitude reading, circle drawing.
        /// </summary>
        public ICommand HandleMouseCommand
        {
            get { return (ICommand)GetValue(HandleMouseCommandProperty); }
            set { SetValue(HandleMouseCommandProperty, value); }
        }

        public static readonly DependencyProperty HandleMouseCommandProperty =
            DependencyProperty.Register("HandleMouseCommand", typeof(ICommand), typeof(MapViewportView), new PropertyMetadata(null));


        /// <summary>
        /// Dependency property for all key commands - move, rotate camera.
        /// </summary>
        public ICommand HandleKeyCommand
        {
            get { return (ICommand)GetValue(HandleKeyCommandProperty); }
            set { SetValue(HandleKeyCommandProperty, value); }
        }

        public static readonly DependencyProperty HandleKeyCommandProperty =
            DependencyProperty.Register("HandleKeyCommand", typeof(ICommand), typeof(MapViewportView), new PropertyMetadata(null));

        public MapViewportView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle mouse events by executing bound mouse command.
        /// </summary>
        private void Viewport3D_HandleMouse(object sender, MouseEventArgs e)
        {
            Point positionInViewportSystem = e.GetPosition(mapViewport);
            MapMouseEventArgs mapMouseEventArgs = new(TransformToRelativeSystem(positionInViewportSystem),
                mapViewport.ActualWidth / mapViewport.ActualHeight, mapViewport.ActualWidth, e);
            HandleMouseCommand?.Execute(mapMouseEventArgs);
            Focus();
        }

        /// <summary>
        /// Handle key events by executing bound key command.
        /// </summary>
        private void Viewport3D_HandleKey(object sender, KeyEventArgs e)
        {
            HandleKeyCommand?.Execute(e);
        }

        /// <summary>
        /// Sign up for window's KeyDown events.
        /// </summary>
        private void Viewport3D_Loaded(object sender, RoutedEventArgs e)
        {
            _window = Window.GetWindow(this);
            _window.KeyDown += Viewport3D_HandleKey;
        }

        /// <summary>
        /// Sign out from window's KeyDown events.
        /// </summary>
        private void Viewport3D_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_window is not null)
            {
                _window.KeyDown -= Viewport3D_HandleKey;
            }
        }

        /// <summary>
        /// Transform point from a viewport system (units are pixels and origin is in the top right corner of the map viewport) 
        /// to a relative viewport system (1 unit is viewport width and origin is in the middle of the map viewport).
        /// </summary>
        /// <param name="p">Point in the viewport system.</param>
        /// <returns>Point in the relative viewport system.</returns>
        private Point TransformToRelativeSystem(Point p)
        {
            Matrix transform = new(1f / mapViewport.ActualWidth, 0, 0, -1f / mapViewport.ActualHeight, -0.5, 0.5);
            return p * transform;
        }

    }
}
