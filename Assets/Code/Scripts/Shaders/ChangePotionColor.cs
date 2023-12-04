using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePotionColor : MonoBehaviour
{
    // Start is called before the first frame update

    /*
    [SerializeField] bool blue = false;
    [SerializeField] bool red = false;
    [SerializeField] bool gray = false;
    [SerializeField] bool baseColor = false;
    */
    [SerializeField] string colorName; 
    [SerializeField] bool testColors = false;
    [SerializeField] bool manual = false;
    [SerializeField] string materialName;
  

    [SerializeField] Color testbase;
    [SerializeField] Color testripple;

    /*
    Color redbaseColor;
    Color redRippleColor;

    Color bluebaseColor;
    Color blueRippleColor;

    Color graybaseColor;
    Color grayRippleColor;

    Color greenbaseColor;
    Color greenRippleColor;

    Color blackbaseColor;
    Color blackRippleColor;

    Color pinkbaseColor;
    Color pinkRippleColor;

    Color orangebaseColor;
    Color orangeRippleColor;

    Color purplebaseColor;
    Color purpleRippleColor;

    Color yellowbaseColor;
    Color yellowRippleColor;
    */

    public Material materialToChange; 

    string rippelColorString = "_RippelColor";
    string baseColorString = "_BaseColor";
    ColorHandeler ch;

    // liquidPlane.GetComponent<ChangePotionColor>().changeColor("red");
    void Start()
    {

        var renderer = GetComponent<MeshRenderer>();
        materialToChange = Instantiate(renderer.sharedMaterial);
        renderer.material = materialToChange;

      ch = FindObjectOfType<ColorHandeler>();

        /*
        // set color: base, ripple
        SetColorRed("#930000", "#FF0000");
        SetColorBlue("#327CA1", "#00FFFF");
        SetColorGray("#8C8C8C", "#FFFFFF");
        SetColorGreen("#00872C","#97FF00");
        SetColorBlack("#413C41", "#5E5D5E");
        SetColorPink("#FF69CB", "#FF9ECB");
        SetColorOrgange("#CA6601", "#FFBD00");
        SetColorPurple("#710D80", "#D105CC");
        SetColorYellow("#CFB009", "#FFF700");
        */


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
        
        if (testColors)
        {
            materialToChange.SetColor(baseColorString, testbase);
            materialToChange.SetColor(rippelColorString, testripple);
        }
        if (manual)
        {
            ChangeColor(colorName);
        }
        
        testbase = materialToChange.GetColor(baseColorString);
        testripple = materialToChange.GetColor(rippelColorString);

        materialName = materialToChange.name;
    }

    public void ChangeColor(string color)
    {  
        UnityEngine.Color baseColor = ch.GetColor(color)[0];
        UnityEngine.Color rippel = ch.GetColor(color)[1];
        materialToChange.SetColor(baseColorString, baseColor);
        materialToChange.SetColor(rippelColorString, rippel);
    }
    /*
    public void ChangeColor(string color)
    {
       
        if(color != null)
        {
            Color baseC = greenbaseColor;
            Color rippelC = greenRippleColor;
          
            switch (color)
            {
                case "red":
                   
                    baseC = redbaseColor;
                    rippelC = redRippleColor;
                    break;
                case "blue":
                   
                    baseC = bluebaseColor;
                    rippelC = blueRippleColor;
                    break;
                case "gray":
                   
                    baseC = graybaseColor;
                    rippelC = grayRippleColor;
                    break;
                case "green":
                   
                    baseC = greenbaseColor;
                    rippelC = greenRippleColor;
                    break;

                case "black":
                    
                    baseC = blackbaseColor;
                    rippelC = blackRippleColor;
                    break;
                case "pink":
                   
                    baseC = pinkbaseColor;
                    rippelC = pinkRippleColor;
                    break;
                case "orange":
                   
                    baseC = orangebaseColor;
                    rippelC = orangeRippleColor;
                    break;
                case "yellow":
                   
                    baseC = yellowbaseColor;
                    rippelC = yellowRippleColor;
                    break;
                case "purple":
                   
                    baseC = purplebaseColor;
                    rippelC = purpleRippleColor;
                    break;
                case "lila":
                   
                    baseC = purplebaseColor;
                    rippelC = purpleRippleColor;
                    break;
                
                default:
                    Debug.Log("hittade inte färgen");
                    break;

            }
            
            materialToChange.SetColor(baseColorString, baseC);
            materialToChange.SetColor(rippelColorString, rippelC);
        }
       
    }
    */
    /*
     // colors
     void SetColorRed(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out redbaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out redRippleColor);
     }

     void SetColorGray(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out graybaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out grayRippleColor);
     }

     void SetColorBlue(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out bluebaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out blueRippleColor);
     }

     void SetColorGreen(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out greenbaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out greenRippleColor);
     }

     void SetColorBlack(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out blackbaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out blackRippleColor);
     }

     void SetColorPink(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out pinkbaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out pinkRippleColor);
     }

     void SetColorOrgange(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out orangebaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out orangeRippleColor);
     }

     void SetColorPurple(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out purplebaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out purpleRippleColor);
     }

     void SetColorYellow(string stringBaseColor, string stringVolvcolor)
     {
         ColorUtility.TryParseHtmlString(stringBaseColor, out yellowbaseColor);
         ColorUtility.TryParseHtmlString(stringVolvcolor, out yellowRippleColor);
     }
    */
    public Material GetMaterial()
    {
        return materialToChange;
    }
}
