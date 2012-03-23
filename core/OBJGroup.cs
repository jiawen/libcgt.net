using System;
using System.Collections.Generic;

namespace libcgt.core
{
	/// <summary>
	/// Summary description for OBJGroup.
	/// </summary>
	public class OBJGroup
	{
		private string name;
		private bool hasTextureCoordinates;
		private bool hasNormals;
		private List< OBJFace > faces;

		public OBJGroup( string name )
		{
			this.name = name;
			hasTextureCoordinates = false;
			hasNormals = false;

			faces = new List< OBJFace >( 3 );
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public bool HasTextureCoordinates
		{
			get
			{
				return hasTextureCoordinates;
			}
			set
			{
				hasTextureCoordinates = value;
			}
		}

		public bool HasNormals
		{
			get
			{
				return hasNormals;
			}
			set
			{
				hasNormals = value;
			}
		}

		public List< OBJFace > Faces
		{
			get
			{
				return faces;
			}
		}

        public void Triangulate()
        {
            List< OBJFace > triangulatedFaces = new List< OBJFace >( faces.Count );

            foreach( OBJFace face in faces )
            {
                int nVertices = face.NumVertices;

                int v0 = face.PositionIndices[ 0 ];
                for( int v = 1; v < nVertices - 1; ++v )
                {
                    int v1 = face.PositionIndices[ v ];
                    int v2 = face.PositionIndices[ v + 1 ];

                    OBJFace triangle = new OBJFace( face.HasTextureCoordinates, face.HasNormals );
                    triangle.PositionIndices.Add( v0 );
                    triangle.PositionIndices.Add( v1 );
                    triangle.PositionIndices.Add( v2 );

                    if( face.HasTextureCoordinates )
                    {
                        triangle.TextureCoordinateIndices.Add( face.TextureCoordinateIndices[ 0 ] );
                        triangle.TextureCoordinateIndices.Add( face.TextureCoordinateIndices[ v ] );
                        triangle.TextureCoordinateIndices.Add( face.TextureCoordinateIndices[ v + 1 ] );
                    }

                    if( face.HasNormals )
                    {
                        triangle.NormalIndices.Add( face.NormalIndices[ 0 ] );
                        triangle.NormalIndices.Add( face.NormalIndices[ v ] );
                        triangle.NormalIndices.Add( face.NormalIndices[ v + 1 ] );
                    }

                    triangulatedFaces.Add( triangle );
                }
            }

            faces = triangulatedFaces;
        }
	}
}
