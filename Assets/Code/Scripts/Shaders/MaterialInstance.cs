using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstance : MonoBehaviour
{
    public GameObject gameObjectToChange;
    public Color colorToChange;
    public Material materialToChange;


    // Start is called before the first frame update
    void Start()
    {
        gameObjectToChange = this.gameObject;
        materialToChange = gameObjectToChange.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        materialToChange.color = colorToChange;
        
    }
}
