using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderParameterChange : MonoBehaviour
{
    // note beh�ver funger f�r shader generel elelr bra den
    // f�r tror jag bara har den har shader som beh�ver fungera s� f� ha att den kallar p� olika metoder som ger olika f�rger
    // typ att olika potions har olika f�rg typer. som att det blir som mer gr�nt som m�rkt eller ljus gr�nt f�r det receptet.

    /* olika material har olika best�ma f�rger som
     * svamp g�ra att dryken blir r�dare
     * �gon att den blir vitar. 
     * 
     * fr�gan �r kan jag �ndra i f�rg v�rde f�r annars kan det bli m�nga olika f�rger b�sta vore att s�tta f�rg koden men �r sv�rt att veta vad den �r
     * d� beh�ver jag koll p� vilka ingridienser som redan finns inne �r att �ndra f�rgern men kan typ ha att minus bl� eller n�got liknad f�r det kanske
     * 
     * ett val �r att ha olika recept och om man f�ljer recept x s� �r det alltid r�tt
     * medan om man l�gger i y som ocks� har x i ett recept s� bytter det f�rger f�r man f�ljer recept y nu ist�llet. 
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
            // beh�ver deras refencs namn och ligga p� den spesfika 
            // toxic informatin som �ndra �r 

            //_BaseColor4
            //_VoronoiColor4

            materialToChange.SetColor(baseColorString, redbaseColor);
            materialToChange.SetColor(volColorString, redvolColor);
            // var mitt i s� tror inte jag �r f�rdig. men testa hur det blir sedan 

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
    
