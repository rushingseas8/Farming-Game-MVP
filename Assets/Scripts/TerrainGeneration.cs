using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainGeneration {
	public static int meshDimension = 4;

	public static float sizeScale = 128f;
	public static float heightScale = 100f;

	private static float uvScale = 2f;

	private static Material defaultMaterial;
	private static PhysicMaterial defaultPhysics;

	static TerrainGeneration() {
		defaultMaterial = Resources.Load ("Materials/Grass", typeof(Material)) as Material;

		defaultPhysics = new PhysicMaterial ();
		defaultPhysics.bounciness = 0f;
		defaultPhysics.dynamicFriction = 0.4f;
		defaultPhysics.staticFriction = 0.7f;
	}

	public static Mesh generateMesh(float worldX, float worldY) {
		int width = meshDimension;
		int length = meshDimension;

		// We over-generate height values, but only add the actual blocks. This is so
		// we can neatly stitch with neighboring chunks.
		float[] heightValues = new float[(width + 1) * (length + 1)];
		for (int i = 0; i < meshDimension + 1; i++) {
			for (int j = 0; j < meshDimension + 1; j++) {
				heightValues [(i * (meshDimension)) + j] = getHeightForCoordinate (worldX, worldY, i, j);
				Debug.Log (heightValues [(i * meshDimension) + j]);
				if (i != meshDimension && j != meshDimension) {
					Block.blocks.Add (new Block (j, heightValues [(i * meshDimension) + j], i, heightValues [(i * meshDimension) + j] / heightScale));
				}
			}
		}

		// Vertex generation
		// Vertices are generated as follows:
		// 1 - 2
		//   / 
		// 3 - 4
		// The first set [0, (4 * w * l)) is the top flat portions of the terrain. The next two
		// sets, each of length (4 * (w-1) * (l-1)), are the north-south and east-west side walls,
		// respectively. The UVs and triangles mirror this arrangement.
		Profiler.BeginSample("Vertex generation");

		//Vector3[] vertices = new Vector3[(4 * width * length) + (8 * (width - 1) * (length - 1))];
		Vector3[] vertices = new Vector3[4 * width * length];
//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < length; j++) {
//				vertices [4 * ((i * width) + j) + 0] = new Vector3 (j, heightValues [(i * width) + j], i);
//				vertices [4 * ((i * width) + j) + 1] = new Vector3 (j + 1, heightValues [(i * width) + j], i);
//				vertices [4 * ((i * width) + j) + 2] = new Vector3 (j, heightValues [(i * width) + j], i + 1);
//				vertices [4 * ((i * width) + j) + 3] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);
//			}
//		}

		// Generate north-south vertical walls
		int base1 = 0;//(4 * width * length);
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				vertices [base1 + (4 * ((i * (width)) + j) + 0)] = new Vector3 (j, heightValues [(i * width) + j], i + 1);
				vertices [base1 + (4 * ((i * (width)) + j) + 1)] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);

				vertices [base1 + (4 * ((i * (width)) + j) + 2)] = new Vector3 (j, heightValues [((i + 1) * width) + j], i + 1);
				vertices [base1 + (4 * ((i * (width)) + j) + 3)] = new Vector3 (j + 1, heightValues [((i + 1) * width) + j], i + 1);
			}
		}

		// Generate east-west vertical walls
//		int base2 = 0; //2 * base1;
//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < length; j++) {
//				//Debug.Log ("Generating EW wall: " + (base2 + (4 * ((i * (width)) + j))) + (heightValues [(i * width) + j]) + " and " + heightValues [(i * width) + j + 1]);
//				vertices [base2 + (4 * ((i * (width)) + j) + 0)] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);
//				vertices [base2 + (4 * ((i * (width)) + j) + 1)] = new Vector3 (j + 1, heightValues [(i * width) + j], i);
//
//				vertices [base2 + (4 * ((i * (width)) + j) + 2)] = new Vector3 (j + 1, heightValues [(i * width) + j + 1], i + 1);
//				vertices [base2 + (4 * ((i * (width)) + j) + 3)] = new Vector3 (j + 1, heightValues [(i * width) + j + 1], i);
//			}
//		}

		Profiler.EndSample();

		//UV mapping generation
		Profiler.BeginSample("UV generation");
		Vector2[] uvs = new Vector2[4 * width * length];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				uvs [4 * ((i * width) + j) + 0] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
				uvs [4 * ((i * width) + j) + 1] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);
				uvs [4 * ((i * width) + j) + 2] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
				uvs [4 * ((i * width) + j) + 3] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
			}
		}

		//TODO: To make the sides look cleaner with texture, scale the UVs based on the difference in heights.

		// North-south walls
//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < length; j++) {
//				uvs [base1 + (4 * ((i * (width)) + j) + 0)] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
//				uvs [base1 + (4 * ((i * (width)) + j) + 1)] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);
//
//				uvs [base1 + (4 * ((i * (width)) + j) + 2)] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
//				uvs [base1 + (4 * ((i * (width)) + j) + 3)] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
//			}
//		}

		// East-west walls
//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < length; j++) {
//				uvs [base2 + (4 * ((i * (width)) + j) + 0)] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
//				uvs [base2 + (4 * ((i * (width)) + j) + 1)] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);
//
//				uvs [base2 + (4 * ((i * (width)) + j) + 2)] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
//				uvs [base2 + (4 * ((i * (width)) + j) + 3)] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
//			}
//		}
		Profiler.EndSample();

		//Triangle generation
		Profiler.BeginSample("Triangle generation");
		int[] triangles = new int[6 * width * length];

		/*
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				int baseVertexIndex = 4 * ((i * width) + j);
				int baseIndex = 6 * ((i * width) + j);

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 2;
				triangles [baseIndex + 2] = baseVertexIndex + 1;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 2;
				triangles [baseIndex + 5] = baseVertexIndex + 3;
			}
		}

		int base3 = 6 * width * length;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				int baseVertexIndex = base1 + (4 * ((i * (width)) + j));
				int baseIndex = base3 + (6 * ((i * (width)) + j));

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 2;
				triangles [baseIndex + 2] = baseVertexIndex + 1;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 2;
				triangles [baseIndex + 5] = baseVertexIndex + 3;
			}
		}

		int base4 = base3 * 2;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				int baseVertexIndex = base2 + (4 * ((i * (width)) + j));
				int baseIndex = base4 + (6 * ((i * (width)) + j));

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 2;
				triangles [baseIndex + 2] = baseVertexIndex + 1;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 2;
				triangles [baseIndex + 5] = baseVertexIndex + 3;
			}
		}
		*/

		for (int k = 0; k < 1; k++) {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < length; j++) {
					int baseVertexIndex = (k * 4 * width * length) + (4 * ((i * width) + j));
					int baseIndex = (k * 6 * width * length) + (6 * ((i * width) + j));

					triangles [baseIndex + 0] = baseVertexIndex;
					triangles [baseIndex + 1] = baseVertexIndex + 2;
					triangles [baseIndex + 2] = baseVertexIndex + 1;
					triangles [baseIndex + 3] = baseVertexIndex + 1;
					triangles [baseIndex + 4] = baseVertexIndex + 2;
					triangles [baseIndex + 5] = baseVertexIndex + 3;
				}
			}
		}

		Profiler.EndSample ();

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();

		return mesh;
	}

	public static float getHeightForCoordinate(float worldX, float worldY, int i, int j) {
		float adjustedScale = sizeScale * (meshDimension / 128.0f) / 128.0f;

		float perlin = perlinOctaves (8, 0.5f, adjustedScale, worldX + ((float)j / meshDimension), worldY + ((float)i / meshDimension));
		float heightOffset = -0.5f * heightScale;

		return (heightOffset + (heightScale * perlin));
	}

	/**
	 * Generates "levels" amount of perlin noise. This is normalized to be between 0-1.
	 * @param levels: How many iterations to run. Higher = more bumpy terrain, lower = smoother.
	 * @param persistence: How much impact each level has. 0.5 is default; higher = more random, lower = more uniform.
	 * @param scale: How "zoomed in" is this noise? Higher = more dense spikes, lower = more spread out hills.
	 */
	private static float perlinOctaves(int levels, float persistence, float scale, float x, float y) {
		float amp = 1;
		float value = 0;
		float sumAmps = 0;
		for (int k = 0; k < levels; k++) {
			sumAmps += amp;
			value += amp * Mathf.PerlinNoise (x * scale, y * scale);
			amp *= persistence;
			scale *= 2;
		}
		return value / sumAmps;
	}

	/*
	public static GameObject createTerrainObject(Mesh mesh, Vector3 position, Material material=defaultMaterial, PhysicMaterial pmat=defaultPhysics) {
		GameObject obj = new GameObject ();

		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		obj.AddComponent<MeshCollider> ();

		obj.GetComponent<MeshFilter> ().mesh = mesh;
		obj.GetComponent<MeshRenderer> ().material = material;
		obj.GetComponent<MeshCollider> ().material = pmat;

		obj.GetComponent<MeshCollider>().sharedMesh = mesh; 

		obj.transform.position = position;
		return obj;
	}
	*/
		
	/// <summary>
	/// Creates the terrain object.
	/// </summary>
	/// <returns>The terrain object.</returns>
	/// <param name="genPos">The world generation coordinates Each terrain is 1x1 in these coords.</param>
	/// <param name="worldPos">Affects actual Unity GameObject position.</param>
	public static GameObject createTerrainObject(Vector2 genPos, Vector3 worldPos) {
		GameObject obj = new GameObject ();
		Mesh mesh = generateMesh (genPos.x, genPos.y);
		worldPos = new Vector3 (worldPos.x * meshDimension, worldPos.y * meshDimension, worldPos.z * meshDimension);

		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		obj.AddComponent<MeshCollider> ();

		obj.GetComponent<MeshFilter> ().mesh = mesh;
		obj.GetComponent<MeshRenderer> ().material = defaultMaterial;
		obj.GetComponent<MeshCollider> ().material = defaultPhysics;

		/* TODO: create simplified compound collider with boxcolliders */
		obj.GetComponent<MeshCollider>().sharedMesh = mesh; 

		obj.transform.position = worldPos;
		return obj;
	}
}
