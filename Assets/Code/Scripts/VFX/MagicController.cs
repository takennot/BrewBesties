using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    // THIS CLASS HANDLES MAGIC EFFEKTS AND SUCH ON INGREDIENTS!!!
    MagicDuration[] magicDurations;

    public bool onlyOnePartical = true;
    public bool createOnce = true;

    [Header("workStation stuff")]
    //[SerializeField] private GameObject book;
    //[SerializeField] private Material bookMaterial;
    //[SerializeField] private GameObject particalPrefab;
    // Start is called before the first frame update
    //GameObject partical;

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

            //foreach (var duration in magicDurations)
            //{
            //    duration.setMagicVariabels(durationVariable);
            //}
        }
    }

    public void DestoryParticle()
    {
        Destroy(magicOnIngredient);

        //if(ingredientObject != null)
        //{
        //    ingredientObject = null;
        //}
    }

    public void MagicOnIngredient()
    {
        if (createOnce)
        {
            //ingredient = ws.GetIngridiense();

            //Debug.Log(MagicObjectEffekt + " : " + ingredientObject);

            CreateMagicSparkleEffect();
            //Debug.Log("create effekts");

            Material[] newArry = new Material[ingredientObject.GetComponentInChildren<MeshRenderer>().materials.Length];

            if (ingredientObject.GetComponent<Ingredient>().GetIngredientType() == Resource_Enum.Ingredient.MonsterEye)
            {
                newArry[0] = magiEyeMaterial;
            }
            else
            {
                newArry[0] = magiMushroomMaterial;
            }

            ingredientObject.GetComponentInChildren<MeshRenderer>().materials = newArry;

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
            //ingredient = ws.GetIngridiense();

            //Debug.Log(MagicObjectEffekt + " : " + ingredientObject);

            magicOnIngredient = Instantiate(MagicObjectEffekt, ingredientObject.transform);
            //Debug.Log("goTo");

            Material[] newArry = new Material[ingredientObject.GetComponentInChildren<MeshRenderer>().materials.Length];

            newArry[0] = ingredientObject.GetComponent<Ingredient>().GetNormalMaterial();

            ingredientObject.GetComponentInChildren<MeshRenderer>().materials = newArry;

        }
    }
}
