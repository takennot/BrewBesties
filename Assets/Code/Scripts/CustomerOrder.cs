using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder
{
    private System.Random rand = new System.Random();
    public enum Ingredients
    {
        Eye,
        Mushroom
    }
    private IngredientAbstract[] orderThree = new IngredientAbstract[3];


    private int amountOfIngredients = 0;

    public CustomerOrder(IngredientAbstract ingredientOne, IngredientAbstract IngredientTwo, IngredientAbstract IngredientThree)
    {
        orderThree[0] = ingredientOne;
        orderThree[1] = IngredientTwo;
        orderThree[2] = IngredientThree;

        amountOfIngredients = 3;
    }


    public IngredientAbstract[] GetIngredients()
    {
        return orderThree;
    }

    public int GetAmountOfIngredients()
    {
        return amountOfIngredients;
    }
}
