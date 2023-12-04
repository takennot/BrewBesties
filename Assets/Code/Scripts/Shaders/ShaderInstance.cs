using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderInstance : MonoBehaviour
{
    private Material flaskMaterial;
    private Material outline; 

    void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        flaskMaterial = Instantiate(renderer.sharedMaterial);
        if (renderer.materials.Length >= 3)
        {
            renderer.materials[0]= flaskMaterial;
            renderer.materials[2] = outline;
        }
        else
        {
            renderer.materials[0] = flaskMaterial;
        }
        

    }
    private void OnDestroy()
    {
        if (flaskMaterial != null)
        {
            Destroy(flaskMaterial);
        }
    }
}
