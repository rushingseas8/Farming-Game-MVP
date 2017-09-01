using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Generator : MonoBehaviour {
	public static int meshDimension = 32;

	public static float sizeScale = 128f;
	public static float heightScale = 100f;

	private static float uvScale = 2f;

	private static Material defaultMaterial;
	private static PhysicMaterial defaultPhysics;

	public static Mesh generateMesh(float worldX, float worldY) {
		int width = meshDimension;
		int length = meshDimension;

		float[] heightValues = new float[width * length];
		for (int i = 0; i < meshDimension; i++) {
			for (int j = 0; j < meshDimension; j++) {
				heightValues [(i * meshDimension) + j] = getHeightForCoordinate (worldX, worldY, i, j);
				Block.blocks.Add (new Block(j, heightValues [(i * meshDimension) + j], i, Random.Range (0, 1)));
			}
		}

		//Vertex generation
		Profiler.BeginSample("Vertex generation");
		//Vector3[] vertices = new Vector3[(4 * width * length) + (4 * (width - 1) * (length - 1))];
		Vector3[] vertices = new Vector3[(4 * width * length) + (8 * (width - 1) * (length - 1))];
		Debug.Log ("Vertices size: " + vertices.Length);
		//Vector3[] vertices = new Vector3[(4 * width * length)];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				vertices [4 * ((i * width) + j) + 0] = new Vector3 (j, heightValues [(i * width) + j], i);
				vertices [4 * ((i * width) + j) + 1] = new Vector3 (j + 1, heightValues [(i * width) + j], i);
				vertices [4 * ((i * width) + j) + 2] = new Vector3 (j, heightValues [(i * width) + j], i + 1);
				vertices [4 * ((i * width) + j) + 3] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);
			}
		}

		// Generate the "stitching" between the squares one way
		int base1 = (4 * width * length);
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				vertices [base1 + (4 * ((i * (width - 1)) + j) + 0)] = new Vector3 (j, heightValues [(i * width) + j], i + 1);
				vertices [base1 + (4 * ((i * (width - 1)) + j) + 1)] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);

				vertices [base1 + (4 * ((i * (width - 1)) + j) + 2)] = new Vector3 (j, heightValues [((i + 1) * width) + j], i + 1);
				vertices [base1 + (4 * ((i * (width - 1)) + j) + 3)] = new Vector3 (j + 1, heightValues [((i + 1) * width) + j], i + 1);
			}
		}

		// Then the other way
		//int base1 = (4 * width * length);
		int base2 = base1 + (4 * (width - 1) * (length - 1));
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				vertices [base2 + (4 * ((i * (width - 1)) + j) + 0)] = new Vector3 (j + 1, heightValues [(i * width) + j], i);
				vertices [base2 + (4 * ((i * (width - 1)) + j) + 1)] = new Vector3 (j + 1, heightValues [(i * width) + j], i + 1);

				vertices [base2 + (4 * ((i * (width - 1)) + j) + 2)] = new Vector3 (j + 1, heightValues [(i * width) + j + 1], i);
				vertices [base2 + (4 * ((i * (width - 1)) + j) + 3)] = new Vector3 (j + 1, heightValues [(i * width) + j + 1], i + 1);
			}
		}

		Profiler.EndSample();

		//UV mapping generation
		Profiler.BeginSample("UV generation");
		//Vector2[] uvs = new Vector2[(4 * width * length)];
		//Vector2[] uvs = new Vector2[(4 * width * length) + (4 * (width - 1) * (length - 1))];
		Vector2[] uvs = new Vector2[(4 * width * length) + (8 * (width - 1) * (length - 1))];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				uvs [4 * ((i * width) + j) + 0] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
				uvs [4 * ((i * width) + j) + 1] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);
				uvs [4 * ((i * width) + j) + 2] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
				uvs [4 * ((i * width) + j) + 3] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
			}
		}

		//TODO: To make the sides look cleaner with texture, scale the UVs based on the difference in heights.

		// Generate the "stitching" between the squares one way
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				uvs [base1 + (4 * ((i * (width - 1)) + j) + 0)] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
				uvs [base1 + (4 * ((i * (width - 1)) + j) + 1)] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);

				uvs [base1 + (4 * ((i * (width - 1)) + j) + 2)] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
				uvs [base1 + (4 * ((i * (width - 1)) + j) + 3)] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
			}
		}

		// Then the other way
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				uvs [base2 + (4 * ((i * (width - 1)) + j) + 0)] = new Vector2 ((float)j / uvScale, (float)i / uvScale);
				uvs [base2 + (4 * ((i * (width - 1)) + j) + 1)] = new Vector2 ((float)(j + 1) / uvScale, (float)i / uvScale);

				uvs [base2 + (4 * ((i * (width - 1)) + j) + 2)] = new Vector2 ((float)j / uvScale, (float)(i + 1) / uvScale);
				uvs [base2 + (4 * ((i * (width - 1)) + j) + 3)] = new Vector2 ((float)(j + 1) / uvScale, (float)(i + 1) / uvScale);
			}
		}

		Profiler.EndSample();

		//Triangle generation
		Profiler.BeginSample("Triangle generation");
		//int[] triangles = new int[(6 * width * length)];
		//int[] triangles = new int[(6 * width * length) + (6 * (width - 1) * (length - 1))];
		int[] triangles = new int[(6 * width * length) + (12 * (width - 1) * (length - 1))];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < length; j++) {
				int baseVertexIndex = 4 * ((i * width) + j);
				int baseIndex = 6 * ((i * width) + j);

				/*
				triangles [baseIndex + 0] = (i * (width)) + j;
				triangles [baseIndex + 1] = ((i + 1) * (width)) + j + 1;
				triangles [baseIndex + 2] = (i * (width)) + j + 1;
				triangles [baseIndex + 3] = (i * (width)) + j;
				triangles [baseIndex + 4] = ((i + 1) * (width)) + j;
				triangles [baseIndex + 5] = ((i + 1) * (width)) + j + 1;
				*/

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 2;
				triangles [baseIndex + 2] = baseVertexIndex + 1;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 2;
				triangles [baseIndex + 5] = baseVertexIndex + 3;
			}
		}

		int base3 = 6 * width * length;
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				int baseVertexIndex = base1 + (4 * ((i * (width - 1)) + j));
				int baseIndex = base3 + (6 * ((i * (width - 1)) + j));

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 2;
				triangles [baseIndex + 2] = baseVertexIndex + 1;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 2;
				triangles [baseIndex + 5] = baseVertexIndex + 3;
			}
		}

		int base4 = base3 + (6 * (width - 1) * (length - 1));
		for (int i = 0; i < width - 1; i++) {
			for (int j = 0; j < length - 1; j++) {
				int baseVertexIndex = base2 + (4 * ((i * (width - 1)) + j));
				int baseIndex = base4 + (6 * ((i * (width - 1)) + j));

				triangles [baseIndex + 0] = baseVertexIndex;
				triangles [baseIndex + 1] = baseVertexIndex + 1;
				triangles [baseIndex + 2] = baseVertexIndex + 2;
				triangles [baseIndex + 3] = baseVertexIndex + 1;
				triangles [baseIndex + 4] = baseVertexIndex + 3;
				triangles [baseIndex + 5] = baseVertexIndex + 2;
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

	public static GameObject createTerrainObject(Mesh mesh, Vector3 position, Material material, PhysicMaterial pmat) {
		GameObject obj = new GameObject ();

		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		obj.AddComponent<MeshCollider> ();

		obj.GetComponent<MeshFilter> ().mesh = mesh;
		obj.GetComponent<MeshRenderer> ().material = material;
		obj.GetComponent<MeshCollider> ().material = pmat;

		/* TODO: create simplified compound collider with boxcolliders */
		obj.GetComponent<MeshCollider>().sharedMesh = mesh; 

		obj.transform.position = position;
		return obj;
	}

	void Start () {
		defaultMaterial = Resources.Load ("Materials/Grass", typeof(Material)) as Material;

		defaultPhysics = new PhysicMaterial ();
		defaultPhysics.bounciness = 0f;
		defaultPhysics.dynamicFriction = 0.4f;
		defaultPhysics.staticFriction = 0.7f;

		int startX = 1938;
		int startY = 9281;

		GameObject obj = createTerrainObject (
			generateMesh(startX, startY),
			new Vector3(0, 0, 0),
			defaultMaterial,
			defaultPhysics);
	}

	void Update () {
		
	}
}
