using MapViewer.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Model for parsed data from a map file.
    /// </summary>
    /// <param name="filepath">Path of the original map data file.</param>
    /// <param name="columnCount">Number of columns in the map vertex grid.</param>
    /// <param name="rowCount">Number of rows in the map vertex grid.</param>
    /// <param name="xLLCorner">X coordinate of the lower left corner in the world coordinate system.</param>
    /// <param name="yLLCorner">Y coordinate of the lower left corner in the world coordinate system.</param>
    /// <param name="cellSize">Distance between map vertex grid rows and column in the world coordinate system.</param>
    /// <param name="altitude">Z coordinate for each vertex in the map vertex grid.</param>
    public class MapData(string filepath, int columnCount, int rowCount, float xLLCorner, float yLLCorner, float cellSize, int[,] altitude)
    {
        /// <summary>
        /// Map file filepath.
        /// </summary>
        public string Filepath { get; } = filepath;

        /// <summary>
        /// Number of grid pouints on the Y axis.
        /// </summary>
        public int ColumnCount { get; } = columnCount;

        /// <summary>
        /// Number of grid pouints on the X axis.
        /// </summary>
        public int RowCount { get; } = rowCount;

        /// <summary>
        /// X offset of the lower left corner of the map.
        /// </summary>
        public float XLLCorner { get; } = xLLCorner;

        /// <summary>
        /// Y offset of the lower left corner of the map.
        /// </summary>
        public float YLLCorner { get; } = yLLCorner;

        /// <summary>
        /// Map Sample distance for X and Y axis.
        /// </summary>
        public float CellSize { get; } = cellSize;

        /// <summary>
        /// Altitude in each sample point starting from index [0,0] which corresponds to [XLLCorner, YLLCorner + (RowCount - 1) * CellSize]
        /// to index [RowCount-1,ColumnCount-1] which corresponds to [XLLCorner + (ColumnCount - 1) * CellSize, YLLCorner].
        /// </summary>
        public int[,] Altitude { get; } = altitude;

        /// <summary>
        /// Maximal altitude in the map (white color).
        /// </summary>
        public int MaxAltitude { get; } = altitude.Cast<int>().Max();

        /// <summary>
        /// Minimal altitutde in the map (black color).
        /// </summary>
        public int MinAltitude { get; } = altitude.Cast<int>().Min();

        /// <summary>
        /// Width of the map.
        /// </summary>
        public float Width
        { 
            get
            {
                return CellSize * (ColumnCount - 1);
            } 
        }

        /// <summary>
        /// Map center on the X axis.
        /// </summary>
        public float XCenter
        {
            get
            {
                return XLLCorner + CellSize * (Convert.ToSingle(ColumnCount - 1) / 2);
            }
        }

        /// <summary>
        /// Map center on the Y axis.
        /// </summary>
        public float YCenter
        {
            get
            {
                return YLLCorner + CellSize * (Convert.ToSingle(RowCount - 1) / 2);
            }
        }

        /// <summary>
        /// Get interpolated altitude between 4 neighboring points on the map.
        /// </summary>
        /// <param name="coords">Map coordinates as a 2D vector.</param>
        /// <returns>Interpolated altitude.</returns>
        /// <exception cref="MapCoordinatesException"><paramref name="coords">Point</paramref> is outside of the map.</exception>
        public float GetAltitude(Vector2 coords)
        {
            if (coords.X < XLLCorner)
            {
                throw new MapCoordinatesException("X coordinate must be greater than XLLCorner.", coords);
            }
            if (coords.X > (XLLCorner + CellSize*ColumnCount))
            {
                throw new MapCoordinatesException("X coordinate must be less then XLLCorner + CellSize * ColumnCount.", coords);
            }
            if (coords.Y < YLLCorner)
            {
                throw new MapCoordinatesException("Y coordinate must be greater than YLLCorner.", coords);
            }
            if (coords.Y > (YLLCorner + CellSize * RowCount))
            {
                throw new MapCoordinatesException("Y coordinate must be less then YLLCorner + CellSize * RowCount.", coords);
            }
            return BilinearAltitudeInterpolation(coords);
        }

        /// <summary>
        /// Bilinear Interpolation for the altitude.
        /// </summary>
        /// <param name="coords">Map coordinates as a 2D vector.</param>
        /// <returns>Interpolated altitude.</returns>
        private float BilinearAltitudeInterpolation(Vector2 coords)
        {
            // Get corresponding indices
            uint indX1 = (uint)Math.Floor(coords.X / CellSize);
            uint indX2 = (uint)Math.Ceiling(coords.X / CellSize);
            uint indY1 = (uint)Math.Floor(coords.Y / CellSize);
            uint indY2 = (uint)Math.Ceiling(coords.Y / CellSize);
            // Init coordinates of interpolated points
            float x1 = (float)(indX1 * CellSize);
            float x2 = (float)(indX2 * CellSize);
            float y1 = (float)(indY1 * CellSize);
            float y2 = (float)(indY2 * CellSize);
            // Prepare transformation matrix
            Matrix4x4 transform = Matrix4x4.Transpose(new Matrix4x4(
                x2*y2, -x2*y1, -x1*y2, x1*y1,
                  -y2,     y1,     y2,   -y1,
                  -x2,     x2,     x1,   -x1,
                    1,     -1,     -1,     1
                ));
            // Retrieve altitudes to be interpolated
            Vector4 interpolatedValues = new(
                Altitude[indX1, indY1],
                Altitude[indX1, indY2],
                Altitude[indX2, indY1],
                Altitude[indX2, indY2]
                );
            // Compute coefficients
            Vector4 polyCoefs = (1 / ((x2 - x1) * (y2 - y1))) * Vector4.Transform(interpolatedValues, transform);
            // Interpolate
            return polyCoefs.X + polyCoefs.Y * coords.X + polyCoefs.Z * coords.Y + polyCoefs.W * coords.X * coords.Y;
        }

    }


}
