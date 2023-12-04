using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Potion
{
    [SerializeField] public IngredientAbstract ingredient1 = null;
    [SerializeField] public IngredientAbstract ingredient2 = null;
    [SerializeField] public IngredientAbstract ingredient3 = null;

    [SerializeField] public bool isPotionDone;

    [SerializeField] public MeshRenderer meshRenderer;

    public Potion(IngredientAbstract ingredient1, IngredientAbstract ingredient2, IngredientAbstract ingredient3, bool done)
    {
        if(ingredient1.validIngredient)
        {
            this.ingredient1 = ingredient1;
            Debug.Log("Added1: " + ingredient1.GetIngredientType() + " to potion");
        }
        if (ingredient2.validIngredient)
        {
            this.ingredient2 = ingredient2;
            Debug.Log("Added2: " + ingredient2.GetIngredientType() + " to potion");
        }
        if (ingredient3.validIngredient)
        {
            this.ingredient3 = ingredient3;
            Debug.Log("Added3: " + ingredient3.GetIngredientType() + " to potion");
        }

       // material = materialInside;

        

        this.isPotionDone = done;
    }

    public string GetString()
    {
        string newString = "Potion: ";

        if (ingredient1 != null && ingredient1.validIngredient)
        {
            newString += ingredient1.GetIngredientType() + "(" + ingredient1.GetIsMagic() + ")";
        }

        if (ingredient2 != null && ingredient2.validIngredient)
        {
            newString += ingredient2.GetIngredientType() + "(" + ingredient2.GetIsMagic() + ")";
        }
        else
        {
            newString += " none";
        }

        if (ingredient3 != null && ingredient3.validIngredient)
        {
            newString += ingredient3.GetIngredientType() + "(" + ingredient3.GetIsMagic() + ")";
        }
        else
        {
            newString += " none";
        }

        return newString;
    }

    public bool IsDone()
    {
        return isPotionDone;
    }

}
