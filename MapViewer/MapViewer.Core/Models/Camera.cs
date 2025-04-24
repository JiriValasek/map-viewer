using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Instantiate model for an OrthographicCamera.
    /// </summary>
    /// <param name="width">Camera's width in the world's coordinate system.</param>
    /// <param name="positionX">X coordinate for initial position.</param>
    /// <param name="positionY">Y coordinate for initial position.</param>
    /// <param name="positionZ">Z coordinate for initial position.</param>
    public class Camera(double width, float positionX, float positionY, float positionZ)
    {
        private Vector3 _position = new Vector3(positionX, positionY, positionZ);
        private Vector3 _lookDirection = new Vector3(0, 0, -1);
        private Vector3 _upDirection = new Vector3(0, 1, 0);

        /// <summary>
        /// Camera's position.
        /// </summary>
        public Vector3 Position 
        {
            get => Vector3.Transform(_position, Transform);
        }

        /// <summary>
        /// Camera's lens direction.
        /// </summary>
        public Vector3 LookDirection
        {
            get => Vector3.Transform(_lookDirection, Transform);
        }

        /// <summary>
        /// Camera's up direction.
        /// </summary>
        public Vector3 UpDirection
        {
            get => Vector3.Transform(_upDirection, Transform);
        }

        /// <summary>
        /// Current cameras's transformation.
        /// </summary>
        public Matrix4x4 Transform { get; }

        /// <summary>
        /// Camera's width in the world's coordinate system.
        /// </summary>
        public double Width { get; } = width;
    }
}
