using System.Numerics;

namespace MapViewer.Core.Exceptions
{

    /// <summary>
    /// Represents errors that occurs when trying to get Altitude outside the map.
    /// </summary>
    public class MapCoordinatesException : Exception
    {
        public Vector2 Coordinates { get; }

        public MapCoordinatesException(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }

        public MapCoordinatesException(string message, Vector2 coordinates) : base(message)
        {
            Coordinates = coordinates;
        }

        public MapCoordinatesException(string message, Exception innerException, Vector2 coordinates) : base(message, innerException)
        {
            Coordinates = coordinates;
        }


    }
}
