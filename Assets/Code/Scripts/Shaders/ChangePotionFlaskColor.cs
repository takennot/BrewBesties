using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePotionFlaskColor : MonoBehaviour
{

    private Material flaskMaterial;

    private string baseColorString = "_BaseColor";
    private string rippelColorString = "_RippelColor";
    private string rimColorString = "_RimColor";
    // Start is called before the first frame update
    void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        flaskMaterial = Instantiate(renderer.sharedMaterial);
        renderer.material = flaskMaterial;
    }
    private void OnDestroy()
    {
        if (flaskMaterial != null)
        {
            Destroy(flaskMaterial);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeFlaskColor(Color baseColor, Color rippleColor)
    {
        flaskMaterial.SetColor(baseColorString, baseColor);
        flaskMaterial.SetColor(rippelColorString, rippleColor);
        flaskMaterial.SetColor(rimColorString, rippleColor);
    }
}
