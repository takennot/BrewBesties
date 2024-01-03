using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHandeler : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        // set color: base, ripple
        SetColorRed("#930000", "#FF0000");
        SetColorBlue("#327CA1", "#00FFFF");
        SetColorGray("#8C8C8C", "#FFFFFF");
        SetColorGreen("#00872C", "#97FF00");
        SetColorBlack("#413C41", "#5E5D5E");
        SetColorPink("#FF69CB", "#FF9ECB");
        SetColorOrgange("#CA6601", "#FFBD00");
        SetColorPurple("#710D80", "#D105CC");
        SetColorYellow("#CFB009", "#FFF700");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color[] GetColor(string color)
    {
        Debug.Log("Color: " + color);

        Color baseC = greenbaseColor;
        Color rippelC = greenRippleColor;

        if (color != null)
        {
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
        }
        Color[] colorToReturn = new Color[] {baseC,  rippelC};
        return colorToReturn;

    }

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
}
