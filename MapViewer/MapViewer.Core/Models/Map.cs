using MapViewer.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    public class Map
    {
        private readonly MapMesh _simpleMesh;

        /// <summary>
        /// Map data parsed from a file.
        /// </summary>
        public MapData Data { get; }

        /// <summary>
        /// Simple map mesh for MeshGeometry3D with all normals in the global map direction, 
        /// not in the direction of each triangle.
        /// </summary>
        public MapMesh SimpleMesh {
            get => _simpleMesh;
        }

        /// <summary>
        /// Instantiate model for a map which is based on map data from a file and displayed as MapMesh.
        /// </summary>
        /// <param name="data">Map data from a file.</param>
        public Map(MapData data)
        {
            Data = data;
            _simpleMesh = GenerateSimpleMesh();
        }

        /// <summary>
        /// Construct a Map from MapData.
        /// </summary>
        /// <returns>MapMesh based on MapData</returns>
        private MapMesh GenerateSimpleMesh()
        {
            return new MapMesh(
                GenerateSimpleMeshPositions(),
                GenerateSimpleMeshNormals(),
                GenerateSimpleMeshTextureCoordinates(),
                GenerateSimpleMeshTriangleIndices()
                );
            
        }

        /// <summary>
        /// Convert MapData to a list of vertices, where each vertex will be included only once.
        /// </summary>
        /// <returns>List of vertices as a string.</returns>
        private string GenerateSimpleMeshPositions()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Altitude.GetLength(0); i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1); j++)
                {
                    sb.AppendFormat("{0},{1},{2} ", 
                        ColumnToXCoordinate(j),
                        RowToYCoordinate(i),
                        Data.Altitude[i,j]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Create list of normals pointing in Z direction for each vertex.
        /// </summary>
        /// <returns>List of normals as a string.</returns>
        private string GenerateSimpleMeshNormals()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin(" ", Enumerable.Repeat("0,0,1", Convert.ToInt32(Data.RowCount * Data.ColumnCount)));
            return sb.ToString();
        }

        /// <summary>
        /// Convert vertex Z directions (altitude) to a list of texture coordinates.
        /// </summary>
        /// <returns>List of texture coordinates as a string for each vertex based on its altitude.</returns>
        private string GenerateSimpleMeshTextureCoordinates()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Altitude.GetLength(0); i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1); j++)
                {
                    sb.AppendFormat("{0},{0} ",
                        AltitudeToTextureCoordinate(Data.Altitude[i, j]));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convert map vertices to triangles represented by indexes of the vertices.
        /// </summary>
        /// <returns>List of triangle indices as a string.</returns>
        private string GenerateSimpleMeshTriangleIndices()
        {
            StringBuilder sb = new StringBuilder();
            long[] indices;
            for (int i = 0; i < Data.Altitude.GetLength(0)-1; i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1)-1; j++)
                {
                    indices = CoordinateTo2TriangleIndices(i, j);
                    sb.AppendFormat("{0},{1},{2} {3},{4},{5} ",
                        indices[0], indices[1], indices[2], indices[3], indices[4], indices[5]);
                }
            }
            return sb.ToString();

        }

        /// <summary>
        /// Convert Row (the first index) to a Y coordinate.
        /// </summary>
        /// <param name="row">Zero indexed row.</param>
        /// <returns>Y coordinate</returns>
        private float RowToYCoordinate(int row)
        {
            var yOffset = Data.YLLCorner + (Data.RowCount - 1) * Data.CellSize;
            return yOffset - (row * Data.CellSize);
        }

        /// <summary>
        /// Convert Column (the second index) to a X coordinate.
        /// </summary>
        /// <param name="column">Zero indexed column.</param>
        /// <returns>X coordinate</returns>
        private float ColumnToXCoordinate(int column)
        {
            var xOffset = Data.XLLCorner + (Data.ColumnCount - 1) * Data.CellSize;
            return xOffset - (column * Data.CellSize);
        }

        /// <summary>
        /// Convert Altitude to a texture coordinate for a LinearGradientBrush
        /// with a color for the lowest point at [0,0] and for the highest point at [1,1].
        /// For flat maps every point is the lowest i.e. [0,0] texture coordinates.
        /// </summary>
        /// <param name="altitude">Map altitude in a vertex.</param>
        /// <returns>Texture coordinate for both X and Y axis.</returns>
        private float AltitudeToTextureCoordinate(uint altitude)
        {
            try { 
                return (Convert.ToSingle(altitude) - Data.MinAltitude) / (Data.MaxAltitude - Data.MinAltitude);
            } 
            catch(DivideByZeroException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Convert top left row and column indices to triangle indices for
        /// a square in the map.
        /// </summary>
        /// <param name="row">
        /// Zero indexed row.
        /// Should not be grater than Data.RowCount-2.
        /// </param>
        /// <param name="column">
        /// Zero indexed column.
        /// Should not be grater than Data.ColumnCount-2.
        /// </param>
        /// <returns>Triangle indices as a string.</returns>
        private long[] CoordinateTo2TriangleIndices(int row, int column)
        {
            var triangle1index1 = row * Data.ColumnCount + column;
            var triangle1index2 = (row + 1) * Data.ColumnCount + column;
            var triangle1index3 = (row + 1) * Data.ColumnCount + column + 1;
            var triangle2index1 = row * Data.ColumnCount + column;
            var triangle2index2 = (row + 1) * Data.ColumnCount + column + 1;
            var triangle2index3 = row * Data.ColumnCount + column + 1;
            return [
                triangle1index1,
                triangle1index2,
                triangle1index3,
                triangle2index1,
                triangle2index2,
                triangle2index3
                ];
            
        }

    }
}
