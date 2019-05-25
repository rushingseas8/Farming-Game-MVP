using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Generator : MonoBehaviour {

	// Adam's stuff

	int plantLimit = 10000;

	int genDelay = 1; //in frames
	int currentStep = 0;

	int currentPlants = 0;

	// George's stuff

	void Start () {

		int startX = 1938;
		int startY = 9281;

		/*
		GameObject obj = createTerrainObject (
			generateMesh(startX, startY),
			new Vector3(0, 0, 0),
			defaultMaterial,
			defaultPhysics);
			*/

		//GameObject obj = TerrainGeneration.createTerrainObject(new Vector2(startX, startY), new Vector3(0, 0, 0));

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				GameObject obj = TerrainGeneration.createTerrainObject(new Vector2(startX + i, startY + j), new Vector3(i, 0, j));
				//Debug.Log (obj.transform.position);
			}
		}
	}

	void Update () {
		/*
		if (currentStep == 0 && currentPlants < plantLimit)
		{
			int randomX = Random.Range(0, 32);
			int randomZ = Random.Range(0, 32);
			float y = Block.heightLookup (randomX, randomZ);

			float randomGL = Random.Range(0.5f, 5.0f);
			float randomINV = Random.Range(0.05f, 0.95f);
			float randomNR = Random.Range(0.05f, 0.5f);

			GameObject plant = (GameObject) Instantiate(Resources.Load("Prefabs/Plant"), new Vector3(randomX + 0.5f, y + 0.001f, randomZ + 0.5f), Quaternion.identity);

			plant.GetComponent<PlantBehavior>().setGrowthLimit(randomGL);
			plant.GetComponent<PlantBehavior>().setNutritionValues(randomINV, randomNR);

			currentPlants++;
		}
		currentStep++;

		currentStep %= genDelay;
		*/
	}
}
