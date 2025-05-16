using System.Numerics;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Model for an OrthographicCamera with default LookDirection (-z) and UpDirection (y).
    /// </summary>
    /// <param name="width">Camera's width in the world's coordinate system.</param>
    /// <param name="positionX">X coordinate for camera position.</param>
    /// <param name="positionY">Y coordinate for camera position.</param>
    /// <param name="positionZ">Z coordinate for camera position.</param>
    public class Camera(double width, float positionX, float positionY, float positionZ)
    {
        private readonly Vector3 _position = new(positionX, positionY, positionZ);
        private readonly Vector3 _lookDirection = new(0, 0, -1);
        private readonly Vector3 _upDirection = new(0, 1, 0);

        /// <summary>
        /// Model for an OrthographicCamera with a custom LookDirection and UpDirection.
        /// </summary>
        /// <param name="position">Camera's position.</param>
        /// <param name="lookDirection">Camera's lens direction</param>
        /// <param name="upDirection">Camera's up dircetion, should be perpendicular to the <paramref name="lookDirection"/>.</param>
        /// <param name="width">Width of the camera.</param>
        public Camera(Vector3 position, Vector3 lookDirection, Vector3 upDirection, double width) :
            this(width, position.X, position.Y, position.Z)
        {
            _lookDirection = lookDirection;
            _upDirection = upDirection;
        }

        /// <summary>
        /// Camera's position.
        /// </summary>
        public Vector3 Position
        {
            get => _position;
        }

        /// <summary>
        /// Camera's lens direction.
        /// </summary>
        public Vector3 LookDirection
        {
            get => _lookDirection;
        }

        /// <summary>
        /// Camera's up direction.
        /// </summary>
        public Vector3 UpDirection
        {
            get => _upDirection;
        }

        /// <summary>
        /// Camera's width in the world's coordinate system.
        /// </summary>
        public double Width { get; } = width;
    }
}
