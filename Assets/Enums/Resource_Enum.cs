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
        return GetRandomIngredient(true, true);
    }

    public static Ingredient GetRandomIngredient(bool mushR, bool eye)
    {
        System.Random rand = new System.Random();

        /*
        if(mushR && eye) // && future ingredients
        {
            int random = rand.Next(1, 3);

            switch (random)
            {
                case 1:
                    if (mushR)
                    {
                        return Ingredient.Mushroom;
                    }
                    break;

                case 2:
                    if (eye)
                    {
                        return Ingredient.MonsterEye;
                    }
                    break;

                default:
                    return Ingredient.Mushroom;
            }
        }
        */

        List<Ingredient> allowedIngrediens = new();

        if (mushR) { allowedIngrediens.Add(Ingredient.Mushroom); }
        if (eye) { allowedIngrediens.Add(Ingredient.MonsterEye); }

        if(allowedIngrediens.Count > 0)
        {
            int randomIndex = rand.Next(0, allowedIngrediens.Count);

            return allowedIngrediens[randomIndex];
        }
        else
        {
            Debug.LogWarning("Trying to get random Ingredient with no ingredients as true. Set any order ingredient bool to true in Goal");
        }
        
        return Ingredient.Mushroom;
    }
}
