using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    MagicDuration[] magicDurations;
    [SerializeField] Workstation ws;
    //private bool setVariabels = true;

    //[SerializeField] private bool testDuration = false;
    [SerializeField] public bool onlyOnePartical = true;
    public bool createOnce = true;

    [SerializeField] private GameObject book;
    [SerializeField] private Material bookMaterial;
    // Start is called before the first frame update

    [SerializeField] private GameObject particalPrefab;
    [SerializeField] private Transform particalPrefabTransform;
    GameObject partical;
    [SerializeField] GameObject MagicObejctEffekt;
    GameObject magicOnIngredient;
    GameObject ingredient = null;
    [SerializeField] Material magiMushroomMaterial;
    [SerializeField] Material magiEyeMaterial; 



    void Start()
    {
        //ws = GetComponent<Workstation>();
        //ws = GetComponent<Workstation>();
        Material[] bookMatreials = book.GetComponent<MeshRenderer>().materials;
        bookMaterial = bookMatreials[3];
        ToggelTextVisabilty(false);
      
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(testDuration == false)
        {
            magicDurations = FindObjectsOfType(typeof(MagicDuration)) as MagicDuration[];
            if (magicDurations.Length > 0 && magicDurations != null) // magic duration exist in scene
            {
                if (setVariabels)
                {
                    toggelTextVisabilty(true);
                    foreach (var duration in magicDurations)
                    {
                        duration.effectTime = ws.GetSlider().maxValue; // 4f; // do something
                    }
                }
                setVariabels = false;
            }
            else
            {
                setVariabels = true;
                toggelTextVisabilty(false);
            }
        }
        */
        
        if(ingredient == null)
        {
            return;
        }

        if (ws.doWork == false && ingredient.GetComponent<Ingredient>().GetIsMagic() == false)
        {
            if (magicOnIngredient != null)
            {
                Destroy(magicOnIngredient);
            }
        }
        
    }

    public void CreatePartical()
    {
        if (onlyOnePartical)
        {
            magicDurations = particalPrefab.GetComponentsInChildren<MagicDuration>();
            ToggelTextVisabilty(true);
            partical = Instantiate(particalPrefab, particalPrefabTransform);
            onlyOnePartical = false;
            foreach (var duration in magicDurations)
            {
                duration.setMagicVariabels(ws.GetSlider().maxValue); // går vidare???+
                //duration.effectTime = ws.GetSlider().maxValue; // 4f; // do something
                /*
                while(duration.GetMoveOnToNext() == false)
                {

                }
                */
            }

            //partical = Instantiate(particalPrefab, particalPrefabTransform);
            //onlyOnePartical = false;
        }
       
    }
    public void DestoryParticla()
    {
        ToggelTextVisabilty(false);
        if (partical != null)
        {
            Destroy(partical);
            onlyOnePartical = true;
        }
        
        if(ingredient != null)
        {
            // Material[] material = ingredient.GetComponentInChildren<MeshRenderer>().materials;
            // Material[] newArry = new Material[ingredient.GetComponentInChildren<MeshRenderer>().materials.Length];

            //newArry[0] = ws.GetIngridiense().GetComponent<Ingredient>().normMaterial();



            /*
            if (inScript.GetIngredientType() == Resource_Enum.Ingredient.Water)
            {
               
            }
            else
            {
                //newArry[0] = inScript.GetNormalMaterial();
            }

            

            Material[] newArry = new Material[ingridanse.GetComponentInChildren<MeshRenderer>().materials.Length - 1];
           
            for(int i = 0; i <newArry.Length; i++)
            {
                newArry[i] = material[i];
               
            }
          */
            //ingredient.GetComponentInChildren<MeshRenderer>().materials = newArry;

            // Detta övre förstör materialet som hamnar på ingredienserna när Magicify() körs.
            // Men det fungerar utan denna kod så därför kommenterade jag ut den! //Saga

            ingredient = null;
            
        }
       

    }

    void ToggelTextVisabilty(bool b)
    {
        if (b)
        {
            bookMaterial.SetFloat("_EmissonStreanght", 100f);
        }
        else
        {
            bookMaterial.SetFloat("_EmissonStreanght", 0f);
        }
        
    }

    public void MagicOnIngredient()
    {
        if (createOnce)
        {

            ingredient = ws.GetIngridiense();

            magicOnIngredient = Instantiate(MagicObejctEffekt, ingredient.transform);
            Debug.Log("create effekts");

            //Material[] material = ingredient.GetComponentInChildren<MeshRenderer>().materials;
            Material[] newArry = new Material[ingredient.GetComponentInChildren<MeshRenderer>().materials.Length];

            if (ws.GetIngridiense().GetComponent<Ingredient>().GetIngredientType() == Resource_Enum.Ingredient.MonsterEye)
            {
                newArry[0] = magiEyeMaterial;
            }
            else
            {
                newArry[0] = magiMushroomMaterial;
            }

            ingredient.GetComponentInChildren<MeshRenderer>().materials = newArry;



            //newArry[0] = ws.GetIngridiense().GetComponent<Ingredient>().GetMaterial();
            /*
            Material[] material = ingridanse.GetComponentInChildren<MeshRenderer>().materials;
            Material[] newArry = new Material[ingridanse.GetComponentInChildren<MeshRenderer>().materials.Length + 1];

            Debug.Log("material is " + material.Length + " new är " + newArry.Length);
            int index = 0;
            foreach(Material m in material)
            {
                newArry[index] = material[index];
                index++;

            }
            newArry[index] = magiMaterial;
            ingridanse.GetComponentInChildren<MeshRenderer>().materials = newArry;
            Debug.Log("nyt materila är " + newArry[index]);
            //ingridanse.GetComponent<MeshRenderer>().materials = newArry;
            */
        }
    }


    

}
