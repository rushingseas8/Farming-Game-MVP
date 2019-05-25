using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehavior : MonoBehaviour {
    float growth  = 0.0f;
    public float growthRate = 0.0f;
    public float growthLimit = 1.0f;
    float maxGrowthRate = 0.001f;
    float minGrowthRate = 0.00001f;

    float maxHeight = 1;

    float startingX;
    float startingZ;
    float startingY;

    float idealNutritionValue;  // 0.0 - 1.0, anywhere between.
    float nutritionRange;       // how far in one direction does the growable range extend to.

    int framesAlive;

	// Use this for initialization
	void Start () {
        startingX = transform.localPosition.x;
        startingY = transform.localPosition.y;
        startingZ = transform.localPosition.z;

        framesAlive = 0;
	}

    float getGrowthRate(float nutritionValue)
    {
        float lowestBound = (idealNutritionValue - nutritionRange < 0 ? 0 : idealNutritionValue - nutritionRange);
        float highestBound = (idealNutritionValue + nutritionRange > 1 ? 1 : idealNutritionValue + nutritionRange);

        if (nutritionValue < lowestBound || nutritionValue > highestBound) return 0.0f;
        else
        {
            float percentageDistace = (nutritionValue - nutritionRange) / (idealNutritionValue - nutritionRange);

            return (maxGrowthRate - minGrowthRate) * percentageDistace + minGrowthRate;
        }
    }

    public void setNutritionValues(float inv, float nr)
    {
        if (inv < 0) idealNutritionValue = 0;
        else if (inv > 1) idealNutritionValue = 1;
        else idealNutritionValue = inv;

        if (nr < 0) nutritionRange = 0;
        else if (nr > 1) nutritionRange = 1;
        else nutritionRange = nr;
    }

    public void setGrowthLimit(float gl)
    {
        growthLimit = gl;
    }
	
	// Update is called once per frame
	void Update () {
		//growthRate = getGrowthRate(0.0f);
		growthRate = getGrowthRate(Block.nutritionLookup((int)(startingX - 0.5f), (int)(startingZ - 0.5f)));

        transform.localScale = new Vector3(0.3f, maxHeight * growth, 0.3f) ;
        transform.localPosition = new Vector3(startingX, startingY + growth, startingZ);

        Component collider = GetComponent<CapsuleCollider>();
        collider.transform.localScale = new Vector3(0.3f, maxHeight * growth, 0.3f);
        collider.transform.localPosition = new Vector3(startingX, startingY + growth, startingZ);

        growth += growthRate;
        growth %= growthLimit;

        framesAlive++;
	}
}
