using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour {
    int plantLimit = 10000;

    int genDelay = 1; //in frames
    int currentStep = 0;

    int currentPlants = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentStep == 0 && currentPlants < plantLimit)
        {
            float randomX = Random.Range(-50.0f, 50.0f);
            float randomZ = Random.Range(-50.0f, 50.0f);
            float y = 0;
            
            float randomGL = Random.Range(0.5f, 5.0f);
            float randomINV = Random.Range(0.05f, 0.95f);
            float randomNR = Random.Range(0.05f, 0.5f);

            GameObject plant = (GameObject) Instantiate(Resources.Load("Prefabs/Plant"), new Vector3(randomX, y, randomZ), Quaternion.identity);

            plant.GetComponent<PlantBehavior>().setGrowthLimit(randomGL);
            plant.GetComponent<PlantBehavior>().setNutritionValues(randomINV, randomNR);

            currentPlants++;
        }
        currentStep++;

        currentStep %= genDelay;
	}
}
