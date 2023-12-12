using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WS_EffektController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Workstation workstation;
    [SerializeField] private Transform particalPrefabTransform;

    [Header("Vars")]
    private MagicDuration[] magicDurations;
    private GameObject partical;
    public bool onlyOnePartical = true;

    [Header("VFX")]
    [SerializeField] private GameObject book;
    private Material bookMaterial;
    [SerializeField] private GameObject particalPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(!workstation)
            workstation = GetComponent<Workstation>();

        Material[] bookMatreials = book.GetComponent<MeshRenderer>().materials;
        bookMaterial = bookMatreials[3];
        ToggelTextVisabilty(false);
    }

    private void ToggelTextVisabilty(bool b)
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

    public void CreateParticle(float durationVariable)
    {
        if (onlyOnePartical)
        {
            magicDurations = particalPrefab.GetComponentsInChildren<MagicDuration>();
            ToggelTextVisabilty(true);
            partical = Instantiate(particalPrefab, particalPrefabTransform);
            onlyOnePartical = false;
            foreach (var duration in magicDurations)
            {
                duration.setMagicVariabels(durationVariable); // går vidare???+
            }
        }
    }

    public void DestoryParticle()
    {
        //ToggelTextVisabilty(false);
        if (partical != null)
        {
            Destroy(partical);
            onlyOnePartical = true;
        }
    }
}
