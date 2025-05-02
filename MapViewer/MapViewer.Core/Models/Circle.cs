using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Model for a circle drawn on the map.
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// Circle's center point.
        /// </summary>
        public Vector2 Center { get; }
        
        /// <summary>
        /// Circle's radius.
        /// </summary>
        public float Radius { get; }

        private readonly Mesh _circleMesh;

        /// <summary>
        /// Mesh for rendering circle in the map.
        /// </summary>
        public Mesh CircleMesh { get => _circleMesh; }

        /// <summary>
        /// Mesh for rendering circle's center in the map.
        /// </summary>
        private readonly Mesh _centerMesh;
        public Mesh CenterMesh { get => _centerMesh; }

        /// <summary>
        /// Circle's altitude in the 3D map.
        /// </summary>
        public float Altitude { get; }

        /// <summary>
        /// Model for a circle drawn on the map.
        /// </summary>
        /// <param name="center">Circle's center point.</param>
        /// <param name="radius">Circle's radius.</param>
        /// <param name="altitude">Circle's altitude in the 3D map.</param>
        /// <param name="centerSize">Width and heigh of the mesh for rendering cicle's center in map units.</param>
        /// <param name="lineWidth">Width of circle's and center's lines in map units.</param>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        public Circle(Vector2 center, float radius, float altitude, float centerSize, float lineWidth, int segmentCount )
        {
            Center = center;
            Radius = radius;
            Altitude = altitude;
            _circleMesh = GenerateCircleMesh(lineWidth, segmentCount);
            _centerMesh = GenerateCenterMesh(lineWidth, centerSize);
        }

        /// <summary>
        /// Generate a mesh for rendering circle in the map.
        /// </summary>
        /// <param name="lineWidth">Width of circle's line in map units.</param>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        /// <returns>Mesh for rendering a circle in the map.</returns>
        private Mesh GenerateCircleMesh(float lineWidth, int segmentCount)
        {
            return new Mesh(
                GenerateCircleMeshPositions(lineWidth, segmentCount),
                GenerateCircleMeshNormals(segmentCount),
                GenerateCircleMeshTextureCoordinates(segmentCount),
                GenerateCircleMeshTriangleIndices(segmentCount)
                );
        }

        /// <summary>
        /// Generate positions of each vertertex in the circle.
        /// Generates circle out of trapezoid line segments.
        /// </summary>
        /// <param name="lineWidth">Width of circle's line in map units.</param>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        /// <returns>List of vertices as 3-dimensional vectors.</returns>
        private List<Vector3> GenerateCircleMeshPositions(float lineWidth, int segmentCount)
        {
            List<Vector3> positions = [];
            for (float i = 0; i < segmentCount; i++)
            {
                positions.Add(
                    new Vector3(
                        (float)(Center.X + (Radius - lineWidth / 2) * Math.Cos(2 * Math.PI * (i / segmentCount))),
                        (float)(Center.Y + (Radius - lineWidth / 2) * Math.Sin(2 * Math.PI * (i / segmentCount))),
                        Altitude
                        )
                    );
                positions.Add(
                    new Vector3(
                        (float)(Center.X + (Radius + lineWidth / 2) * Math.Cos(2 * Math.PI * (i / segmentCount))),
                        (float)(Center.Y + (Radius + lineWidth / 2) * Math.Sin(2 * Math.PI * (i / segmentCount))),
                        Altitude
                        )
                    );
            }
            return positions;
        }

        /// <summary>
        /// Generate normals for each vertex in the circle.
        /// Normals are oriented in opposite direction to the camera's look direction.
        /// </summary>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        /// <returns>List of normal vectors.</returns>
        private static IEnumerable<Vector3> GenerateCircleMeshNormals(int segmentCount)
        {
            return Enumerable.Repeat(new Vector3(0, 0, 1), 2 * segmentCount);
        }

        /// <summary>
        /// Generate texture coordinates for each vertex in the circle.
        /// Texture coordinates are all 0,0 for SolidColorBrush.
        /// </summary>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        /// <returns>List of 2-dimensional vectors pointers to the texture map one for each vertex.</returns>
        private static IEnumerable<Vector2> GenerateCircleMeshTextureCoordinates(int segmentCount)
        {
            return Enumerable.Repeat(new Vector2(0, 0), 2 * segmentCount);
        }

        /// <summary>
        /// Generate list of triangle indices to convert vertices into drawable triangles.
        /// Composes a circle out of trapezoids and trapezoids out of two triangles each.
        /// </summary>
        /// <param name="segmentCount">Number of line segments to use for rendering the circle.</param>
        /// <returns>List of triangle indices, where each successive three indices are one triangle.</returns>
        private static List<int> GenerateCircleMeshTriangleIndices(int segmentCount)
        {
            List<int> indices = [];
            for (int i = 0; i < (segmentCount - 1); i++)
            {
                indices.Add(2 * i);
                indices.Add(2 * i + 1);
                indices.Add(2 * (i + 1) + 1);
                indices.Add(2 * i);
                indices.Add(2 * (i + 1) + 1);
                indices.Add(2 * (i + 1));
            }
            indices.Add(2 * segmentCount - 2);
            indices.Add(2 * segmentCount - 1);
            indices.Add(1);
            indices.Add(2 * segmentCount - 2);
            indices.Add(1);
            indices.Add(0);
            return indices;
        }

        /// <summary>
        /// Generate mesh for rendering circle's center in the map.
        /// </summary>
        /// <param name="lineWidth">Width of center's lines in map units.</param>
        /// <param name="centerSize">Width and heigh of the mesh for rendering cicle's center in map units.</param>
        /// <returns>Mesh for rendering a circle's center in the map.</returns>
        private Mesh GenerateCenterMesh(float lineWidth, float centerSize)
        {
            return new Mesh(
                GenerateCenterMeshPositions(lineWidth, centerSize),
                GenerateCenterMeshNormals(),
                GenerateCenterMeshTextureCoordinates(),
                GenerateCenterMeshTriangleIndices()
                );
        }

        /// <summary>
        /// Generate positions of each vertertex in the circle's center.
        /// Generates a cross out of 4 triangles.
        /// </summary>
        /// <param name="lineWidth">Width of center's lines in map units.</param>
        /// <param name="centerSize">Width and heigh of the mesh for rendering cicle's center in map units.</param>
        /// <returns>List of vertices as 3-dimensional vectors.</returns>
        private List<Vector3> GenerateCenterMeshPositions(float lineWidth, float centerSize)
        {
            List<Vector3> positions = [];
            positions.Add(new Vector3(Center.X, Center.Y, Altitude ));
            positions.Add(new Vector3(Center.X + centerSize, Center.Y - (lineWidth / 2), Altitude));
            positions.Add(new Vector3(Center.X + centerSize, Center.Y + (lineWidth / 2), Altitude));
            positions.Add(new Vector3(Center.X - centerSize, Center.Y + (lineWidth / 2), Altitude));
            positions.Add(new Vector3(Center.X - centerSize, Center.Y - (lineWidth / 2), Altitude));
            positions.Add(new Vector3(Center.X + (lineWidth / 2), Center.Y + centerSize, Altitude));
            positions.Add(new Vector3(Center.X - (lineWidth / 2), Center.Y + centerSize, Altitude));
            positions.Add(new Vector3(Center.X - (lineWidth / 2), Center.Y - centerSize, Altitude));
            positions.Add(new Vector3(Center.X + (lineWidth / 2), Center.Y - centerSize, Altitude));
            return positions;
        }

        /// <summary>
        /// Generate normals for each vertex in the circle's center.
        /// Normals are oriented in opposite direction to the camera's look direction.
        /// </summary>
        /// <returns>List of normal vectors.</returns>
        private static IEnumerable<Vector3> GenerateCenterMeshNormals()
        {
            return Enumerable.Repeat(new Vector3(0, 0, 1), 9);
        }

        /// <summary>
        /// Generate texture coordinates for each vertex in the circle's center.
        /// Texture coordinates are all 0,0 for SolidColorBrush.
        /// </summary>
        /// <returns>List of 2-dimensional vectors pointers to the texture map one for each vertex.</returns>
        private static IEnumerable<Vector2> GenerateCenterMeshTextureCoordinates()
        {
            return Enumerable.Repeat(new Vector2(0, 0), 9);
        }

        /// <summary>
        /// Generate list of triangle indices to convert vertices into drawable triangles.
        /// Composes a circle's center cross out of four triangles.
        /// </summary>
        /// <returns>List of triangle indices, where each successive three indices are one triangle.</returns>
        private static List<int> GenerateCenterMeshTriangleIndices()
        {
            List<int> indices = [0, 1, 2, 0, 3, 4, 0, 5, 6, 0, 7, 8];
            return indices;
        }
    }
}
