using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    /// <summary>
    /// Instantiate model for MeshGeometry3D representation of a map.
    /// </summary>
    /// <param name="positions">Vertex positions for MeshGeometry3D.</param>
    /// <param name="normals">Vertex normals for MeshGeometry3D.</param>
    /// <param name="textureCoordinates">Vertex texture coordinates for MeshGeometry3D.</param>
    /// <param name="triangleIndices">Triangle indices for MeshGeometry3D.</param>
    public class MapMesh (string positions, string normals, string textureCoordinates, string triangleIndices)
    {
        /// <summary>
        /// Vertex positions for MeshGeometry3D.
        /// </summary>
        public string Positions { get; } = positions;

        /// <summary>
        /// Vertex normals for MeshGeometry3D.
        /// </summary>
        public string Normals { get; } = normals;

        /// <summary>
        /// Vertex texture coordinates for MeshGeometry3D.
        /// </summary>
        public string TextureCoordinates { get; } = textureCoordinates;

        /// <summary>
        /// Triangle indices for MeshGeometry3D.
        /// </summary>
        public string TriangleIndices { get; } = triangleIndices;
    }
}
