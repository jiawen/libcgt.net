using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
	/// <summary>
	/// The raw data from an OBJ file.
    /// Note that the OBJ file format does not support multiple texture coordinates.
	/// </summary>
	public class OBJData
	{
		private List< Vector3f > positions;
		private List< Vector2f > textureCoordinates;
		private List< Vector3f > normals;

		private Dictionary< string, OBJGroup > groups;

        // constructs a completely empty OBJData
        public static OBJData Empty()
        {
            OBJData objData = new OBJData();
            objData.groups.Add( "", new OBJGroup( "" ) );

            return objData;
        }

        public static OBJData FromFile( string objFilename )
        {
            using( var sr = new StreamReader( objFilename ) )
            {
                OBJData objData = new OBJData();
                string currentGroupName = "";
                OBJGroup currentGroup = new OBJGroup( currentGroupName );
                objData.groups.Add( currentGroupName, currentGroup );

                int lineNumber = 1;
                string line = sr.ReadLine();
                while( line != null )
                {
                    if( line != "" )
                    {
                        string[] tokens = OBJData.RemoveEmptyTokens( line.Split( ' ', '\t' ) );
                        if( tokens.Length > 0 )
                        {
                            switch( tokens[ 0 ] )
                            {
                                case "g":

                                    string newGroupName = "";
                                    if( tokens.Length > 1 )
                                    {
                                        newGroupName = tokens[ 1 ];
                                    }
                                    if( newGroupName != currentGroupName )
                                    {
                                        if( objData.groups.ContainsKey( newGroupName ) )
                                        {
                                            currentGroup = ( OBJGroup )( objData.groups[ newGroupName ] );
                                        }
                                        else
                                        {
                                            currentGroup = new OBJGroup( newGroupName );
                                            objData.groups.Add( newGroupName, currentGroup );
                                        }

                                        currentGroupName = newGroupName;
                                    }
                                    break;

                                case "v":

                                    OBJData.HandlePosition( lineNumber, line,
                                        tokens, objData );
                                    break;

                                case "vt":
                                    OBJData.HandleTextureCoordinate( lineNumber, line,
                                        tokens, objData );
                                    break;

                                case "vn":
                                    OBJData.HandleNormal( lineNumber, line,
                                        tokens, objData );
                                    break;

                                case "f":
                                    OBJData.HandleFace( lineNumber, line,
                                        tokens, currentGroup, objData );
                                    break;

                                case "fo":
                                    OBJData.HandleFace( lineNumber, line,
                                        tokens, currentGroup, objData );
                                    break;
                            }
                        }
                    }

                    ++lineNumber;
                    line = sr.ReadLine();
                }

                return objData;
            }
        }

        /// <summary>
        /// Triangulates all faces in all groups
        /// </summary>
        public void Triangulate()
        {
            foreach( OBJGroup group in groups.Values )
            {
                group.Triangulate();
            }
        }

        /// <summary>
        /// Merges all the groups into the "" group
        /// The merged group has the least common set of attributes
        /// So if one of the groups don't have texture coordinates,
        /// then they will all be dropped
        /// </summary>
        public void MergeGroups()
        {
            // initially assume they have everything
            // then scan through all non-empty groups,
            // and flip the bits if they don't have the attribute
            bool mergedGroupHasTextureCoordinates = true;
            bool mergedGroupHasNormals = true;

            // first remove all groups that have no faces
            List< string > emptyGroupNames = new List<string>();
            foreach( string groupName in groups.Keys )
            {
                OBJGroup currentGroup = groups[ groupName ];
                if( currentGroup.Faces.Count == 0 )
                {
                    emptyGroupNames.Add( groupName );
                }
            }
            foreach( string groupName in emptyGroupNames )
            {
                groups.Remove( groupName );
            }

            foreach( OBJGroup group in groups.Values )
            {
                if( !( group.HasTextureCoordinates ) )
                {
                    mergedGroupHasTextureCoordinates = false;
                    break;
                }
            }

            foreach( OBJGroup group in groups.Values )
            {
                if( !( group.HasNormals ) )
                {
                    mergedGroupHasNormals = false;
                    break;
                }
            }

            OBJGroup mergedGroup = new OBJGroup( "" );
            mergedGroup.HasTextureCoordinates = mergedGroupHasTextureCoordinates;
            mergedGroup.HasNormals = mergedGroupHasNormals;

            foreach( OBJGroup group in groups.Values )
            {
                foreach( OBJFace f in group.Faces )
                {
                    OBJFace newFace = new OBJFace( mergedGroupHasTextureCoordinates, mergedGroupHasNormals );
                    newFace.PositionIndices.AddRange( f.PositionIndices );
                    if( mergedGroupHasTextureCoordinates )
                    {
                        newFace.TextureCoordinateIndices.AddRange( f.TextureCoordinateIndices );
                    }
                    if( mergedGroupHasNormals )
                    {
                        newFace.NormalIndices.AddRange( f.NormalIndices );
                    }

                    mergedGroup.Faces.Add( newFace );
                }
            }

            groups.Clear();
            groups.Add( "", mergedGroup );
        }

        public void SaveBinaryTriangleList( string filename, string groupName )
        {
            OBJGroup group = groups[ groupName ];

            // output is a triangle list, so after triangulation
            // a polygon of n vertices becomes n - 2 triangles
            int nVertices = 0;
            foreach( OBJFace f in group.Faces )
            {
                int nVerticesInFace = f.PositionIndices.Count;
                nVertices += 3 * ( nVerticesInFace - 2 );
            }

            int nTextureCoordinateSets = 0;
            if( group.HasTextureCoordinates )
            {
                nTextureCoordinateSets = 1;
            }

            int nNormals = 0;
            if( group.HasNormals )
            {
                nNormals = 1;
            }

            FileStream fs = new FileStream( filename, FileMode.Create );
            BinaryWriter bw = new BinaryWriter( fs );

            bw.Write( nVertices );
            bw.Write( nTextureCoordinateSets );
            bw.Write( nNormals );

            foreach( OBJFace f in group.Faces )
            {
                Vector3f v0 = positions[ f.PositionIndices[ 0 ] ];

                for( int v = 1; v < ( f.PositionIndices.Count - 1 ); ++v )
                {
                    Vector3f v1 = positions[ f.PositionIndices[ v ] ];
                    Vector3f v2 = positions[ f.PositionIndices[ v + 1 ] ];

                    bw.Write( v0.x );
                    bw.Write( v0.y );
                    bw.Write( v0.z );
                    bw.Write( v1.x );
                    bw.Write( v1.y );
                    bw.Write( v1.z );
                    bw.Write( v2.x );
                    bw.Write( v2.y );
                    bw.Write( v2.z );
                }
            }

            if( group.HasTextureCoordinates )
            {
                foreach( OBJFace f in group.Faces )
                {
                    Vector2f t0 = textureCoordinates[ f.TextureCoordinateIndices[ 0 ] ];

                    for( int v = 1; v < ( f.TextureCoordinateIndices.Count - 1 ); ++v )
                    {
                        Vector2f t1 = textureCoordinates[ f.TextureCoordinateIndices[ v ] ];
                        Vector2f t2 = textureCoordinates[ f.TextureCoordinateIndices[ v + 1 ] ];

                        bw.Write( t0.x );
                        bw.Write( t0.y );
                        bw.Write( t1.x );
                        bw.Write( t1.y );
                        bw.Write( t2.x );
                        bw.Write( t2.y );
                    }
                }
            }

            if( group.HasNormals )
            {
                foreach( OBJFace f in group.Faces )
                {
                    Vector3f n0 = normals[ f.NormalIndices[ 0 ] ];

                    for( int v = 1; v < ( f.NormalIndices.Count - 1 ); ++v )
                    {
                        Vector3f n1 = normals[ f.NormalIndices[ v ] ];
                        Vector3f n2 = normals[ f.NormalIndices[ v + 1 ] ];

                        bw.Write( n0.x );
                        bw.Write( n0.y );
                        bw.Write( n0.z );
                        bw.Write( n1.x );
                        bw.Write( n1.y );
                        bw.Write( n1.z );
                        bw.Write( n2.x );
                        bw.Write( n2.y );
                        bw.Write( n2.z );
                    }
                }
            }

            bw.Close();
            fs.Close();
        }

        /*
        public static void SaveBinaryTriangleList( string filename, OBJData data, OBJGroup group )
        {
            // output is a triangle list, so after triangulation
            // a polygon of n vertices becomes n - 2 triangles
            int nVertices = 0;
            foreach( OBJFace f in group.Faces )
            {
                int nVerticesInFace = f.PositionIndices.Count;
                nVertices += 3 * ( nVerticesInFace - 2 );
            }

            int nTextureCoordinateSets = 0;
            if( group.HasTextureCoordinates )
            {
                nTextureCoordinateSets = 1;
            }

            int nNormals = 0;
            if( group.HasNormals )
            {
                nNormals = 1;
            }

            FileStream fs = new FileStream( filename, FileMode.Create );
            BinaryWriter bw = new BinaryWriter( fs );

            bw.Write( nVertices );
            bw.Write( nTextureCoordinateSets );
            bw.Write( nNormals );

            foreach( OBJFace f in group.Faces )
            {
                Vector3f v0 = data.Positions[ f.PositionIndices[ 0 ] ];

                for( int v = 1; v < ( f.PositionIndices.Count - 1 ); ++v )
                {
                    Vector3f v1 = data.Positions[ f.PositionIndices[ v ] ];
                    Vector3f v2 = data.Positions[ f.PositionIndices[ v + 1 ] ];

                    bw.Write( v0.x );
                    bw.Write( v0.y );
                    bw.Write( v0.z );
                    bw.Write( v1.x );
                    bw.Write( v1.y );
                    bw.Write( v1.z );
                    bw.Write( v2.x );
                    bw.Write( v2.y );
                    bw.Write( v2.z );
                }                
            }

            if( group.HasTextureCoordinates )
            {
                foreach( OBJFace f in group.Faces )
                {
                    Vector2f t0 = data.TextureCoordinates[ f.TextureCoordinateIndices[ 0 ] ];

                    for( int v = 1; v < ( f.TextureCoordinateIndices.Count - 1 ); ++v )
                    {
                        Vector2f t1 = data.TextureCoordinates[ f.TextureCoordinateIndices[ v ] ];
                        Vector2f t2 = data.TextureCoordinates[ f.TextureCoordinateIndices[ v + 1 ] ];

                        bw.Write( t0.x );
                        bw.Write( t0.y );
                        bw.Write( t1.x );
                        bw.Write( t1.y );
                        bw.Write( t2.x );
                        bw.Write( t2.y );
                    }
                }
            }

            if( group.HasNormals )
            {
                foreach( OBJFace f in group.Faces )
                {
                    Vector3f n0 = data.Normals[ f.NormalIndices[ 0 ] ];

                    for( int v = 1; v < ( f.NormalIndices.Count - 1 ); ++v )
                    {
                        Vector3f n1 = data.Normals[ f.NormalIndices[ v ] ];
                        Vector3f n2 = data.Normals[ f.NormalIndices[ v + 1 ] ];

                        bw.Write( n0.x );
                        bw.Write( n0.y );
                        bw.Write( n0.z );
                        bw.Write( n1.x );
                        bw.Write( n1.y );
                        bw.Write( n1.z );
                        bw.Write( n2.x );
                        bw.Write( n2.y );
                        bw.Write( n2.z );
                    }
                }
            }

            bw.Close();
            fs.Close();
        }
        */

		public void SaveOBJ( string filename )
		{
			StreamWriter sw = new StreamWriter( filename );

			// save positions
			for( int i = 0; i < positions.Count; ++i )
			{
				Vector3f v = positions[i];
				sw.WriteLine( "v {0} {1} {2}", v.x, v.y, v.z );
			}

			// save texture coordinates
			for( int i = 0; i < textureCoordinates.Count; ++i )
			{
				Vector2f v = textureCoordinates[i];
				sw.WriteLine( "vt {0} {1}", v.x, v.y );
			}
		
			// save normals
			for( int i = 0; i < normals.Count; ++i )
			{
				Vector3f v = normals[i];
				sw.WriteLine( "vn {0} {1} {2}", v.x, v.y, v.z );
			}

			foreach( OBJGroup group in groups.Values )
			{
                if( group.Name == null || group.Name == "" )
                {
                    throw new ArgumentException( "OBJ group name cannot be null or empty!" );
                }
                sw.WriteLine( "g " + group.Name );

				foreach( OBJFace face in group.Faces )
				{
					StringBuilder sb = new StringBuilder( "f " );

					for( int i = 0; i < face.NumVertices; ++i )
					{
						sb.Append( ( int )( face.PositionIndices[i] ) + 1 );
						
						if( face.HasTextureCoordinates )
						{
                            // has both
                            if( face.HasNormals )
                            {
                                sb.AppendFormat( "/{0}/{1} ",
                                    ( int )( face.TextureCoordinateIndices[ i ] ) + 1,
                                    ( int )( face.NormalIndices[ i ] ) + 1 );
                            }
                            else // texture coordinates only
                            {
                                sb.AppendFormat( "/{0} ",
                                ( int )( face.TextureCoordinateIndices[ i ] ) + 1 );
                            }
						}
						// normals but no texture coordinates
						else if( face.HasNormals )
						{
							sb.AppendFormat( "//{0} ",
								( int )( face.NormalIndices[i] ) + 1 );
						}
					}

					sw.WriteLine( sb.ToString().Trim() );
				}
			}
			
			sw.Close();
		}

		private static void HandlePosition( int lineNumber, string line,
			string[] tokens, OBJData objData )
		{
			if( tokens.Length < 4 )
			{
				throw new ArgumentException( "Incorrect number of tokens at line number: "
					+ lineNumber + "\n" + line );
			}
			else
			{
				float x = float.Parse( tokens[1] );
				float y = float.Parse( tokens[2] );
				float z = float.Parse( tokens[3] );

				objData.positions.Add( new Vector3f( x, y, z ) );
			}
		}

		private static void HandleTextureCoordinate( int lineNumber, string line,
			string[] tokens, OBJData objData )
		{
			if( tokens.Length < 3 )
			{
				throw new ArgumentException( "Incorrect number of tokens at line number: "
					+ lineNumber + "\n" + line );
			}
			else
			{
				float s = float.Parse( tokens[1] );
				float t = float.Parse( tokens[2] );

				objData.textureCoordinates.Add( new Vector2f( s, t ) );
			}
		}

		private static void HandleNormal( int lineNumber, string line,
			string[] tokens, OBJData objData )
		{
			if( tokens.Length < 4 )
			{
				throw new ArgumentException( "Incorrect number of tokens at line number: "
					+ lineNumber + "\n" + line );
			}
			else
			{
				float nx = float.Parse( tokens[1] );
				float ny = float.Parse( tokens[2] );
				float nz = float.Parse( tokens[3] );

				objData.normals.Add( new Vector3f( nx, ny, nz ) );
			}
		}

		private static void HandleFace( int lineNumber, string line,
			string[] tokens, OBJGroup group, OBJData objData )
		{
			if( tokens.Length < 4 )
			{
				throw new ArgumentException( "Incorrect number of tokens at line number: "
					+ lineNumber + "\n" + line );
			}
			else
			{
				// first check line consistency - each vertex in the face
				// should have the same number of attributes

				bool faceIsValid;
				bool faceHasTextureCoordinates;
				bool faceHasNormals;

				faceIsValid = OBJData.IsFaceLineAttributesConsistent( tokens,
					out faceHasTextureCoordinates, out faceHasNormals );

				if( !faceIsValid )
				{
					throw new ArgumentException( "Face attributes inconsistent at line number: "
						+ lineNumber + "\n" + line );
				}

				// ensure that all faces in a group are consistent
				// they either all have texture coordinates or they don't
				// they either all have normals or they don't
				// 
				// check how many faces the current group has
				// if the group has no faces, then the first vertex sets it

				if( group.Faces.Count == 0 )
				{
					group.HasTextureCoordinates = faceHasTextureCoordinates;
					group.HasNormals = faceHasNormals;
				}

				bool faceIsConsistentWithGroup = ( group.HasTextureCoordinates == faceHasTextureCoordinates ) &&
					( group.HasNormals == faceHasNormals );
				
				if( !faceIsConsistentWithGroup )
				{
					string message = "Face attributes inconsistent with group: " + group.Name;
					message += " at line: " + lineNumber + "\n" + line;
					message += " group.HasTextureCoordinates: " + group.HasTextureCoordinates;
					message += " face.HasTextureCoordinates: " + faceHasTextureCoordinates;
					message += " group.HasNormals: " + group.HasNormals;
					message += " face.HasNormals: " + faceHasNormals;
					
					throw new ArgumentException( message );
				}

				OBJFace face = new OBJFace( faceHasTextureCoordinates, faceHasNormals );

				// for each vertex
				for( int i = 1; i < tokens.Length; ++i )
				{
					int vertexPositionIndex;
					int vertexTextureCoordinateIndex;
					int vertexNormalIndex;

					OBJData.GetVertexAttributes( tokens[i],
						out vertexPositionIndex, out vertexTextureCoordinateIndex, out vertexNormalIndex );

					face.PositionIndices.Add( vertexPositionIndex );

					if( faceHasTextureCoordinates )
					{
						face.TextureCoordinateIndices.Add( vertexTextureCoordinateIndex );
					}
					if( faceHasNormals )
					{
						face.NormalIndices.Add( vertexNormalIndex );
					}
				}

				group.Faces.Add( face );
			}
		}

		private static bool IsFaceLineAttributesConsistent( string[] tokens,
			out bool hasTextureCoordinates, out bool hasNormals )
		{
			int firstVertexPositionIndex;
			int firstVertexTextureCoordinateIndex;
			int firstVertexNormalIndex;

			bool firstVertexIsValid;
			bool firstVertexHasTextureCoordinates;
			bool firstVertexHasNormals;

			firstVertexIsValid = OBJData.GetVertexAttributes( tokens[1],
				out firstVertexPositionIndex, out firstVertexTextureCoordinateIndex, out firstVertexNormalIndex );
			firstVertexHasTextureCoordinates = ( firstVertexTextureCoordinateIndex != -1 );
			firstVertexHasNormals = ( firstVertexNormalIndex != -1 );

			if( !firstVertexIsValid )
			{
				hasTextureCoordinates = false;
				hasNormals = false;
				return false;
			}
			
			for( int i = 2; i < tokens.Length; ++i )
			{
				int vertexPositionIndex;
				int vertexTextureCoordinateIndex;
				int vertexNormalIndex;

				bool vertexIsValid;
				bool vertexHasTextureCoordinates;
				bool vertexHasNormals;

				vertexIsValid = OBJData.GetVertexAttributes( tokens[i],
					out vertexPositionIndex, out vertexTextureCoordinateIndex, out vertexNormalIndex );
				vertexHasTextureCoordinates = ( vertexTextureCoordinateIndex != -1 );
				vertexHasNormals = ( vertexNormalIndex != -1 );

				if( !vertexIsValid )
				{
					hasTextureCoordinates = false;
					hasNormals = false;
					return false;
				}

                if( firstVertexHasTextureCoordinates != vertexHasTextureCoordinates )
				{
					hasTextureCoordinates = false;
					hasNormals = false;
					return false;
				}

				if( firstVertexHasNormals != vertexHasNormals )
				{
					hasTextureCoordinates = false;
					hasNormals = false;
					return false;
				}
			}

			hasTextureCoordinates = firstVertexHasTextureCoordinates;
			hasNormals = firstVertexHasNormals;
			return true;
		}

		// objFaceVertexToken is something of the form:
		// "int"
		// "int/int"
		// "int/int/int", 
		// "int//int"
		// i.e. one of the delimited int strings that specify a vertex and its attributes
		// 
		// returns:
		// whether the vertex is valid
		private static bool GetVertexAttributes( string objFaceVertexToken,
			out int positionIndex, out int textureCoordinateIndex, out int normalIndex )
		{
			string[] vertexAttributes = objFaceVertexToken.Split( '/' );
			int vertexNumAttributes = vertexAttributes.Length;
			
			// check if it has position
			if( vertexNumAttributes < 1 )
			{
				positionIndex = -1;
				textureCoordinateIndex = -1;
				normalIndex = -1;
				return false;
			}
			else
			{
				if( vertexAttributes[0] == "" )
				{
					positionIndex = -1;
					textureCoordinateIndex = -1;
					normalIndex = -1;
					return false;
				}
				else
				{
					positionIndex = int.Parse( vertexAttributes[0] ) - 1;

					if( vertexNumAttributes > 1 )
					{
						if( vertexAttributes[1] == "" )
						{
							textureCoordinateIndex = -1;
						}
						else
						{
							textureCoordinateIndex = int.Parse( vertexAttributes[1] ) - 1;
						}

						if( vertexNumAttributes > 2 )
						{
							if( vertexAttributes[2] == "" )
							{
								normalIndex = -1;
							}
							else
							{
								normalIndex = int.Parse( vertexAttributes[2] ) - 1;
							}
						}
						else
						{
							normalIndex = -1;
						}				
					}
					else
					{
						textureCoordinateIndex = -1;
						normalIndex = -1;
					}
				}
				
				return true;
			}
		}

		private static string[] RemoveEmptyTokens( string[] tokens )
		{
			// count number of non-empty tokens
			int numNonEmptyTokens = 0;
			for( int i = 0; i < tokens.Length; ++i )
			{
				if( tokens[i] != "" )
				{
					++numNonEmptyTokens;
				}
			}

			string[] nonEmptyTokens = new string[ numNonEmptyTokens ];
			int j = 0;
			for( int i = 0; i < tokens.Length; ++i )
			{
				if( tokens[i] != "" )
				{
					nonEmptyTokens[j] = tokens[i];
					++j;
				}
			}

			return nonEmptyTokens;
		}

		public List< Vector3f > Positions
		{
			get
			{
				return positions;
			}
		}

		public List< Vector2f > TextureCoordinates
		{
			get
			{
				return textureCoordinates;
			}
		}

		public List< Vector3f > Normals
		{
			get
			{
				return normals;
			}
        }
        
        public OBJGroup DefaultGroup
        {
            get
            {
                return groups[ "" ];
            }
        }

        public Dictionary< string, OBJGroup > Groups
		{
			get
			{
				return groups;
			}
		}
		
		private OBJData()
		{
			positions = new List< Vector3f >( 100 );
			textureCoordinates = new List< Vector2f >( 100 );
			normals = new List< Vector3f >( 100 );

			groups = new Dictionary< string, OBJGroup >( 10 );
		}
	}
}
