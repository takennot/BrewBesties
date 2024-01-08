using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Resource_Enum
{
    public enum Resource
    {
        Mushroom,
        MonsterEye,
        PixieDust,

        Bottle,
        FireWood
    }

    public enum Ingredient
    {
        Mushroom,
        MonsterEye,
        PixieDust,
        Water
    }

    public static bool IsIngredient(Resource resource)
    {
        switch (resource)
        {
            case Resource.Mushroom:
                return true;
            case Resource.MonsterEye:
                return true;
            case Resource.PixieDust:
                return true;

            case Resource.Bottle:
                return false;
            case Resource.FireWood:
                return false;
            default: 
                return false;
        }
    }

    public static Ingredient GetRandomIngredient()
    {
        return GetRandomIngredient(1.0f, 1.0f);
    }

    public static Ingredient GetRandomIngredient(float mushR, float eye)
    {
        //System.Random rand = new System.Random();

        //List<Ingredient> allowedIngrediens = new();
        //
        //if (mushR > 0) { allowedIngrediens.Add(Ingredient.Mushroom); }
        //if (eye) { allowedIngrediens.Add(Ingredient.MonsterEye); }
        //
        //if(allowedIngrediens.Count > 0)
        //{
        //    int randomIndex = rand.Next(0, allowedIngrediens.Count);

        //    return allowedIngrediens[randomIndex];
        //}
        //else
        //{
        //    Debug.LogWarning("Trying to get random Ingredient with no ingredients as true. Set any order ingredient bool to true in Goal");
        //}

        float totalProcent = mushR + eye;

        if(totalProcent > 0)
        {
            float randomFloat = Random.Range(0, totalProcent);
            Debug.Log("Generated random: " + randomFloat);

            if (randomFloat < mushR)
                return Ingredient.Mushroom;
            else if (randomFloat < mushR + eye)
                return Ingredient.MonsterEye;
        }
        else
        {
            Debug.LogWarning("Trying to get random Ingredient with 0% in both. Returning Mushroom as default. Please set any order ingredient float to min 1% in Goal");
        }

        return Ingredient.Mushroom;
    }
}
