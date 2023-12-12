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

    [SerializeField] private MagicController magicController;

    public int mainMaterialIndex;

    [SerializeField] private bool isMagic = false;

    public Resource_Enum.Ingredient GetIngredientType() { return ingredientType; }

    public Material GetMaterial()
    {
        return meshRenderer.materials[mainMaterialIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    // OBS: DETTA �R INTE SAMMA KLASS SOM L�NGST UPP, DET �R EN ABSTRAKT VERSION.
    // OM DU SKA L�GGA TILL EN METOD, SKROLLA UPP TILL R�TT KLASS!!!!
}
