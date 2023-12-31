using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    // THIS CLASS HANDLES MAGIC EFFEKTS AND SUCH ON INGREDIENTS!!!
    //MagicDuration[] magicDurations;

    public bool onlyOnePartical = true;

    public bool createOnce = true;

    [Header("workStation stuff")]

    [Header("Ingredient stuff")]
    [SerializeField] private GameObject ingredientObject;

    [Header("Magic VFX stuff")]

    [SerializeField] private Transform particalPrefabTransform;

    [SerializeField] GameObject MagicObjectEffekt;
    private GameObject magicOnIngredient;

    [Header("Materials")]
    [SerializeField] Material magiMushroomMaterial;
    [SerializeField] Material magiEyeMaterial;

    // Update is called once per frame

    public void CreateParticle(float durationVariable)
    {
        if (onlyOnePartical)
        {
            onlyOnePartical = false;
        }
    }

    public void DestoryParticle()
    {
        Destroy(magicOnIngredient);
    }

    public void MagicOnIngredient()
    {
        if (createOnce)
        {

            CreateMagicSparkleEffect();
            //Debug.Log("create effekts");

            if (ingredientObject.GetComponent<Ingredient>().GetIngredientType() == Resource_Enum.Ingredient.MonsterEye)

            {
                ingredientObject.GetComponentInChildren<MeshRenderer>().material = magiEyeMaterial;
            }
            else
            {
                ingredientObject.GetComponentInChildren<MeshRenderer>().material = magiMushroomMaterial;
            }

        }
    }

    public void CreateMagicSparkleEffect()
    {
        magicOnIngredient = Instantiate(MagicObjectEffekt, ingredientObject.transform);
    }

    public void NotMagicOnIngredient()
    {
        if (createOnce)
        {
            magicOnIngredient = Instantiate(MagicObjectEffekt, ingredientObject.transform);
            //Debug.Log("goTo");

            ingredientObject.GetComponentInChildren<MeshRenderer>().material = ingredientObject.GetComponent<Ingredient>().GetNormalMaterial();
        }
    }

}
