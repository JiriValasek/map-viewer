using MapViewer.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Model for a map to be rendered in Viewport3D.
    /// </summary>
    public class Map
    {
        private readonly Mesh _mapMesh;

        /// <summary>
        /// Map data parsed from a file.
        /// </summary>
        public MapData Data { get; }

        /// <summary>
        /// Mesh for rendering a map based on <see cref="Data"/>.
        /// </summary>
        public Mesh MapMesh {
            get => _mapMesh;
        }

        /// <summary>
        /// Model for a map to be rendered in Viewport3D.
        /// </summary>
        /// <param name="data">Map data from a file.</param>
        public Map(MapData data)
        {
            Data = data;
            _mapMesh = GenerateMap();
        }

        /// <summary>
        /// Generate a mesh for rendering a map based on <see cref="Data"/>.
        /// </summary>
        /// <returns>MapMesh based on MapData</returns>
        private Mesh GenerateMap()
        {
            var positions = GenerateMapPositions();
            var normals = GenerateMapNormals(positions);
            var textureCoords = GenerateMapTextureCoordinates();
            var triangleIndices = GenerateMapTriangleIndices();
            return new Mesh(
                positions,
                normals,
                textureCoords,
                triangleIndices
                );
            
        }

        /// <summary>
        /// Convert MapData to a list of vertices, where each vertex will be included only once.
        /// </summary>
        /// <returns>List of vertices as 3-dimensional vectors.</returns>
        private List<Vector3> GenerateMapPositions()
        {
            List<Vector3> positions = [];
            for (int i = 0; i < Data.Altitude.GetLength(0); i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1); j++)
                {
                    positions.Add( new Vector3(
                        Data.ColumnToXCoordinate(j),
                        Data.RowToYCoordinate(i),
                        Data.Altitude[i, j]));
                }
            }
            return positions;
        }

        /// <summary>
        /// Create list of normals pointing in direction perpendicular to a vector
        /// from a given position to the next position in a row, and to a vector
        /// from a given position to the next poisition in a column.
        /// </summary>
        /// <returns>List of normals.</returns>
        private List<Vector3> GenerateMapNormals(List<Vector3> positions)
        {
            List<Vector3> normals = [];

            for (int i = 0; i < Data.Altitude.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1) - 1; j++)
                {
                    normals.Add(Vector3.Cross(
                    positions[(j + 1) * Data.ColumnCount + i] - positions[j * Data.ColumnCount + i],
                    positions[j * Data.ColumnCount + i + 1] - positions[j * Data.ColumnCount + i]
                    ));
                }
            }
            return normals;
        }

        /// <summary>
        /// Convert vertex Z directions (altitude) to a list of texture coordinates.
        /// Texture coordinates are from a diagonal of a 2D LinearGradientBrush,
        /// [0,0] for the lowest point and [1,1] for the highest point.
        /// </summary>
        /// <returns>List of texture coordinates for each vertex based on its altitude.</returns>
        private List<Vector2> GenerateMapTextureCoordinates()
        {
            List<Vector2> textureCoords = [];
            float textureXY;
            for (int i = 0; i < Data.Altitude.GetLength(0); i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1); j++)
                {
                    textureXY = AltitudeToTextureCoordinate(Data.Altitude[i, j]);
                    textureCoords.Add(new Vector2(textureXY, textureXY));
                }
            }
            return textureCoords;
        }

        /// <summary>
        /// Convert map vertices to triangles represented by indexes of the vertices.
        /// Composes a map out of squares and squares out of two triangles each.
        /// </summary>
        /// <returns>List of triangle indices.</returns>
        private List<int> GenerateMapTriangleIndices()
        {
            List<int> indices = [];
            IEnumerable<int> square;
            for (int i = 0; i < Data.Altitude.GetLength(0)-1; i++)
            {
                for (int j = 0; j < Data.Altitude.GetLength(1)-1; j++)
                {
                    square = CoordinateTo2TriangleIndices(i, j);
                    indices.AddRange(square);
                }
            }
            return indices;
        }

        /// <summary>
        /// Convert Altitude to a texture coordinate for a LinearGradientBrush
        /// with a color for the lowest point at [0,0] and for the highest point at [1,1].
        /// For flat maps every point is the lowest i.e. [0,0] texture coordinates.
        /// </summary>
        /// <param name="altitude">Map altitude in a vertex.</param>
        /// <returns>Texture coordinate for both X and Y axis.</returns>
        private float AltitudeToTextureCoordinate(int altitude)
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
        private IEnumerable<int> CoordinateTo2TriangleIndices(int row, int column)
        {
            int triangle1index1 = row * Data.ColumnCount + column;
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
