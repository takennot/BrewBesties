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

    [SerializeField] private GameObject book;
    [SerializeField] private Material bookMaterial;
    // Start is called before the first frame update

    [SerializeField] private GameObject particalPrefab;
    [SerializeField] private Transform particalPrefabTransform;
    GameObject partical;

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
}
