using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private Resource_Enum.Ingredient ingredientType = Resource_Enum.Ingredient.Water;

    [Header("Visual")]
    [SerializeField] private Material material;
    [SerializeField] private Material magicMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spritemagic;

    [Header("Colors")]
    [SerializeField] private string neutralPotionColor;
    [SerializeField] private string magicalPotionColor;

    [SerializeField] private MagicController magicController;

    public int mainMaterialIndex;

    [SerializeField] private bool isMagic = false;

    public Resource_Enum.Ingredient GetIngredientType() { return ingredientType; }

    public Material GetMaterial()
    {
        return meshRenderer.materials[mainMaterialIndex];
    }
    public string GetColorStr()
    {
        if (isMagic)
        {
            return magicalPotionColor;
        }

        return neutralPotionColor;
    }

    public void Magicify()
    {
        isMagic = true;

        Debug.Log("Magic material:" + magicMaterial);
        meshRenderer.material = magicMaterial;

        magicController.CreateMagicSparkleEffect();
    }

    public void Magicify(Material magicalMaterial)
    {
        isMagic = true; 
        
        meshRenderer.material = magicalMaterial;
    }

    public bool GetIsMagic()
    {
        return isMagic;
    }
    public void SetMagic(bool state)
    {
        isMagic = state;
    }

    public Sprite GetImage()
    {
        if (isMagic)
        {
            return spritemagic;
        }
        else
        {
            return spriteNormal;
        }
    }

    public void SetIngredient(Resource_Enum.Ingredient newIngredient)
    {
        ingredientType = newIngredient;
    }

    public MagicController GetMagicController()
    {
        return magicController;
    }

    public Material GetNormalMaterial()
    {
        return material;
    }

}

public class IngredientAbstract
{
    [SerializeField] private Resource_Enum.Ingredient ingredientType;

    public bool validIngredient = false;

    [Header("Visual")]
    [SerializeField] private Material material;
    [SerializeField] private Material magicMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spritemagic;

    public int mainMaterialIndex;

    [SerializeField] private bool isMagic = false;

    public IngredientAbstract(Resource_Enum.Ingredient newIngredientType, bool newMagic)
    {
        ingredientType = newIngredientType;
        isMagic = newMagic;
        validIngredient = true;
    }
    public IngredientAbstract()
    {
        validIngredient = false;
    }

    public Resource_Enum.Ingredient GetIngredientType() { return ingredientType; }

    public Material GetMaterial()
    {
        return meshRenderer.materials[mainMaterialIndex];
    }

    public bool GetIsMagic()
    {
        return isMagic;
    }
    public void SetMagic(bool state)
    {
        isMagic = state;
    }

    public Sprite GetImage()
    {
        if (isMagic)
        {
            return spritemagic;
        }
        else
        {
            return spriteNormal;
        }
    }

    public void SetIngredient(Resource_Enum.Ingredient newIngredient)
    {
        ingredientType = newIngredient;
    }


    public string GetString()
    {
        return ingredientType + "(" + isMagic + ")";
    }

    public void SetImages(SpriteManager spriteManager)
    {
        switch (ingredientType)
        {
            case Resource_Enum.Ingredient.Mushroom:
                spriteNormal = spriteManager.spriteMushroom;
                spritemagic = spriteManager.spriteMagicMushroom;

                break;
            case Resource_Enum.Ingredient.MonsterEye:
                spriteNormal = spriteManager.spriteEye;
                spritemagic = spriteManager.spriteMagicEye;

                break;
            default: break;
        }
    }
    // OBS: DETTA ÄR INTE SAMMA KLASS SOM LÄNGST UPP, DET ÄR EN ABSTRAKT VERSION.
    // OM DU SKA LÄGGA TILL EN METOD, SKROLLA UPP TILL RÄTT KLASS!!!!
}
