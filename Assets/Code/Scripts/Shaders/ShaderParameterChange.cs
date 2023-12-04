using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderParameterChange : MonoBehaviour
{
    // note behöver funger för shader generel elelr bra den
    // för tror jag bara har den har shader som behöver fungera så få ha att den kallar på olika metoder som ger olika färger
    // typ att olika potions har olika färg typer. som att det blir som mer grönt som mörkt eller ljus grönt för det receptet.

    /* olika material har olika bestäma färger som
     * svamp göra att dryken blir rödare
     * ögon att den blir vitar. 
     * 
     * frågan är kan jag ändra i färg värde för annars kan det bli många olika färger bästa vore att sätta färg koden men är svårt att veta vad den är
     * då behöver jag koll på vilka ingridienser som redan finns inne är att ändra färgern men kan typ ha att minus blå eller något liknad för det kanske
     * 
     * ett val är att ha olika recept och om man följer recept x så är det alltid rött
     * medan om man lägger i y som också har x i ett recept så bytter det färger för man följer recept y nu istället. 
    */

    //[SerializeField] bool changeShader = false;

    [SerializeField] bool blue = false;
    [SerializeField] bool red = false;
    [SerializeField] bool gray = false;
    [SerializeField] bool baseColor = false;
    [SerializeField] bool testColors = false;


    [SerializeField] Color testbase;
    [SerializeField] Color testvol;


    Color redbaseColor;
    Color redvolColor;

    Color bluebaseColor;
    Color bluevolColor;

    Color graybaseColor;
    Color grayvolColor;

    Color normalbaseColor;
    Color normalmvolColor;


    private Material materialToChange;

    string baseColorString = "_BaseColor4";
    string volColorString = "_VoronoiColor4";


    // Start is called before the first frame update
    void Start()
    {

        setColorRed("#930000","#FF0000");
        setColorBlue("#0044B4", "#58F3F0");
        setColorGray("#8C8C8C", "#FFFFFF");


        // basic get this mater from object
        var renderer = GetComponent<MeshRenderer>();
        materialToChange = Instantiate(renderer.sharedMaterial);
        renderer.material = materialToChange;
        //gameObjectToChange = this.gameObject;
        //materialToChange = gameObjectToChange.GetComponent<MeshRenderer>().material;

        normalbaseColor = materialToChange.GetColor(baseColorString);
        normalmvolColor = materialToChange.GetColor(volColorString);
    }
    private void OnDestroy()
    {
        if (materialToChange != null)
        {
            Destroy(materialToChange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //materialToChange.color = colorToChange; 
        if (red == true)
        {
            //materialToChange.SetVector("__Color5", colorToChange);
            //materialToChange.SetFloat()

            //materialToChange.SetColor("_Color", colorToChange);
            //materialToChange.SetFloat("_Offset5", 1f);
            // behöver deras refencs namn och ligga på den spesfika 
            // toxic informatin som ändra är 

            //_BaseColor4
            //_VoronoiColor4

            materialToChange.SetColor(baseColorString, redbaseColor);
            materialToChange.SetColor(volColorString, redvolColor);
            // var mitt i så tror inte jag är färdig. men testa hur det blir sedan 

            resetColors();
        }
        else if (blue == true)
        {
            materialToChange.SetColor(baseColorString, bluebaseColor);
            materialToChange.SetColor(volColorString, bluevolColor);
            resetColors();
        }
        else if (gray == true)
        {
            materialToChange.SetColor(baseColorString, graybaseColor);
            materialToChange.SetColor(volColorString, grayvolColor);
            resetColors();
        }
        else if (baseColor == true)
        {
            materialToChange.SetColor(baseColorString, normalbaseColor);
            materialToChange.SetColor(volColorString, normalmvolColor);
            resetColors();
        }

        if (testColors)
        {
            resetColors();
            materialToChange.SetColor(baseColorString, testbase);
            materialToChange.SetColor(volColorString, testvol);
        }

    }

    void resetColors()
    {
        gray = false;
        baseColor = false;
        blue = false;
        red = false;
    }

    void setColors()
    {
        
        // EX: ColorUtility.TryParseHtmlString("#930000", out redvolColor);
    }

    void setColorRed(string stringBaseColor, string stringVolvcolor)
    {
        ColorUtility.TryParseHtmlString(stringBaseColor, out redbaseColor);
        ColorUtility.TryParseHtmlString(stringVolvcolor, out redvolColor);
    }

    void setColorGray(string stringBaseColor, string stringVolvcolor)
    {
        ColorUtility.TryParseHtmlString(stringBaseColor, out graybaseColor);
        ColorUtility.TryParseHtmlString(stringVolvcolor, out grayvolColor);
    }

    void setColorBlue(string stringBaseColor, string stringVolvcolor)
    {
        ColorUtility.TryParseHtmlString(stringBaseColor, out bluebaseColor);
        ColorUtility.TryParseHtmlString(stringVolvcolor, out bluevolColor);
    }

    public void changeColor(string color)
    {
        switch (color)
        {
            case "red":
                red = true;
                break;
            case "blue":
                blue = true;
                break;
            case "gray":
                gray= true;
                break;
            case "green":
                baseColor = true;
                break;

        }
    }
}
    
