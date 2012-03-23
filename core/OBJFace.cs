using System;
using System.Collections.Generic;

namespace libcgt.core
{
	/// <summary>
	/// Summary description for OBJFace.
	/// </summary>
	public class OBJFace
	{
		private bool hasTextureCoordinates;
		private bool hasNormals;

		private List< int > positionIndices;
		private List< int > textureCoordinateIndices;
		private List< int > normalIndices;		

		public OBJFace( bool hasTextureCoordinates, bool hasNormals )
		{
			this.hasTextureCoordinates = hasTextureCoordinates;
			this.hasNormals = hasNormals;

			positionIndices = new List< int >( 3 );
			textureCoordinateIndices = new List< int >( 3 );
			normalIndices = new List< int >( 3 );
		}

		public int NumVertices
		{
			get
			{
				// TODO: check consistency?
				return positionIndices.Count;
			}
		}

		public bool HasTextureCoordinates
		{
			get
			{
                return ( textureCoordinateIndices.Count != 0 );
			}
		}

		public bool HasNormals
		{
			get
			{
                return ( normalIndices.Count != 0 );
			}
		}

		public List< int > PositionIndices
		{
			get
			{
				return positionIndices;
			}
		}

		public List< int > TextureCoordinateIndices
		{
			get
			{
				return textureCoordinateIndices;
			}
		}

		public List< int > NormalIndices
		{
			get
			{
				return normalIndices;
			}
		}
	}
}
