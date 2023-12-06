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
    GameObject maigOnIngridanse;
    GameObject ingridanse = null;
    [SerializeField] Material magiMaterial;



    void Start()
    {
        //ws = GetComponent<Workstation>();
        //ws = GetComponent<Workstation>();
        Material[] bookMatreials = book.GetComponent<MeshRenderer>().materials;
        bookMaterial = bookMatreials[3];
        toggelTextVisabilty(false);
      
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
      
    }

    public void CreatePartical()
    {
        if (onlyOnePartical)
        {
            magicDurations = particalPrefab.GetComponentsInChildren<MagicDuration>();
            toggelTextVisabilty(true);
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
        toggelTextVisabilty(false);
        if (partical != null)
        {
            Destroy(partical);
            onlyOnePartical = true;
        }
        
        if(ingridanse != null)
        {
            Material[] material = ingridanse.GetComponentInChildren<MeshRenderer>().materials;
            Material[] newArry = new Material[ingridanse.GetComponentInChildren<MeshRenderer>().materials.Length - 1];
           
            for(int i = 0; i <newArry.Length; i++)
            {
                newArry[i] = material[i];
               
            }
          
            ingridanse.GetComponentInChildren<MeshRenderer>().materials = newArry;
            ingridanse = null;
        }

    }

    void toggelTextVisabilty(bool b)
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

    public void MagicOnIngridanse()
    {
        if (createOnce)
        {
            ingridanse = ws.GetIngridiense();
            maigOnIngridanse = Instantiate(MagicObejctEffekt, ingridanse.transform);
            Debug.Log("create effekts");

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


        }
    }


    

}
