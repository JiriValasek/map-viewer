using System.Numerics;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Model for a MeshGeometry3D view.
    /// </summary>
    /// <param name="positions">Vertex positions for MeshGeometry3D.</param>
    /// <param name="normals">Vertex normals for MeshGeometry3D.</param>
    /// <param name="textureCoordinates">Vertex texture coordinates for MeshGeometry3D.</param>
    /// <param name="triangleIndices">Triangle indices for MeshGeometry3D.</param>
    public class Mesh(IEnumerable<Vector3> positions, IEnumerable<Vector3> normals, IEnumerable<Vector2> textureCoordinates, IEnumerable<int> triangleIndices)
    {
        /// <summary>
        /// Vertex positions for MeshGeometry3D.
        /// </summary>
        public IEnumerable<Vector3> Positions { get; } = positions;

        /// <summary>
        /// Vertex normals for MeshGeometry3D.
        /// </summary>
        public IEnumerable<Vector3> Normals { get; } = normals;

        /// <summary>
        /// Vertex texture coordinates for MeshGeometry3D.
        /// </summary>
        public IEnumerable<Vector2> TextureCoordinates { get; } = textureCoordinates;

        /// <summary>
        /// Triangle indices for MeshGeometry3D.
        /// </summary>
        public IEnumerable<int> TriangleIndices { get; } = triangleIndices;
    }
}
