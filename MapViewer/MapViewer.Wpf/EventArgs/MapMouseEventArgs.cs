using System.Windows;
using System.Windows.Input;

namespace MapViewer.Wpf.EventArgs
{
    /// <summary>
    /// MouseEventArgs wrapper to add additional information necessary for processing.
    /// </summary>
    /// <param name="mapViewportPosition">Relative cursor position to the map Viewport3D with Viewport3D width as a unit.</param>
    /// <param name="aspectRatio">Current Viewport3D aspect ratio.</param>
    /// <param name="mapWidth">Current Viewport3D width.</param>
    /// <param name="mouseEventArgs">Original MouseEventArgs.</param>
    public class MapMouseEventArgs(Point mapViewportPosition, double aspectRatio, double mapWidth, MouseEventArgs mouseEventArgs) : System.EventArgs()
    {
        /// <summary>
        /// Relative cursor position to the map Viewport3D with Viewport3D width as a unit.
        /// </summary>
        public Point MapViewportPosition { get; } = mapViewportPosition;

        /// <summary>
        /// Current Viewport3D width.
        /// </summary>
        public double MapWidth { get; } = mapWidth;

        /// <summary>
        /// Current Viewport3D aspect ratio.
        /// </summary>
        public double AspectRatio { get; } = aspectRatio;

        /// <summary>
        /// Original MouseEventArgs.
        /// </summary>
        public MouseEventArgs MouseEventArgs { get; } = mouseEventArgs;
    }
}
