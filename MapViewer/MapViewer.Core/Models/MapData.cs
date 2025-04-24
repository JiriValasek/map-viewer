using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    public class MapData
    {
        /// <summary>
        /// Map file filepath.
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        /// Number of grid pouints on the Y axis.
        /// </summary>
        public uint ColumnCount { get; }

        /// <summary>
        /// Number of grid pouints on the X axis.
        /// </summary>
        public uint RowCount { get; }

        /// <summary>
        /// X offset of the lower left corner of the map.
        /// </summary>
        public float XLLCorner { get; }

        /// <summary>
        /// Y offset of the lower left corner of the map.
        /// </summary>
        public float YLLCorner { get; }

        /// <summary>
        /// Map Sample distance for X and Y axis.
        /// </summary>
        public float CellSize { get; }

        /// <summary>
        /// Altitude in each sample pouint starting from index [0,0] which corresponds to [XLLCorner, YLLCorner + (RowCount - 1) * CellSize]
        /// to index [RowCount-1,ColumnCount-1] which corresponds to [XLLCorner + (ColumnCount - 1) * CellSize, YLLCorner].
        /// </summary>
        public uint[,] Altitude { get; } //TODO is SOLID? Optimization use ReadOnlySpan2D from Microsoft.Toolkit.HighPerformance

        /// <summary>
        /// Maximal altitude in the map (white color).
        /// </summary>
        public uint MaxAltitude { get; }

        /// <summary>
        /// Minimal altitutde in the map (black color).
        /// </summary>
        public uint MinAltitude { get; }

        /// <summary>
        /// Construct model for parsed data from a map file.
        /// </summary>
        /// <param name="filepath">Path of the original map data file.</param>
        /// <param name="columnCount">Number of columns in the map vertex grid.</param>
        /// <param name="rowCount">Number of rows in the map vertex grid.</param>
        /// <param name="xLLCorner">X coordinate of the lower left corner in the world coordinate system.</param>
        /// <param name="yLLCorner">Y coordinate of the lower left corner in the world coordinate system.</param>
        /// <param name="cellSize">Distance between map vertex grid rows and column in the world coordinate system.</param>
        /// <param name="altitude">Z coordinate for each vertex in the map vertex grid.</param>
        public MapData(string filepath, uint columnCount, uint rowCount, float xLLCorner, float yLLCorner, float cellSize, uint[,] altitude)
        {
            Filepath = filepath;
            ColumnCount = columnCount;
            RowCount = rowCount;
            XLLCorner = xLLCorner;
            YLLCorner = yLLCorner;
            CellSize = cellSize;
            Altitude = altitude;
            MaxAltitude = altitude.Cast<uint>().Max();
            MinAltitude = altitude.Cast<uint>().Min();
        }
    }


}
