using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CompletionRequirements
{
    //seperating by 3 to view easier
    public static List<int> tutorialRequirements = new List<int>(new int[] { 1, 1, 1 });
    public static List<int> getAGripRequirements = new List<int>(new int[] { 250, 700, 900 });
    public static List<int> completionistRequirements = new List<int>(new int[] { 170, 600, 780 });

    public static List<int> tutorial2Requirements = new List<int>(new int[] { 1, 1, 1 });
    public static List<int> movingFieldRequirements = new List<int>(new int[] { 300, 600, 900 });
    public static List<int> threeIslandsRequirements = new List<int>(new int[] { 300, 550, 700 });

    public static List<int> ghostHouseRequirements = new List<int>(new int[] { 300, 550, 700 });
    public static List<int> icyPlatformsRequirements = new List<int>(new int[] { 270, 490, 665 }); 
    public static List<int> rotatinatorRequirements = new List<int>(new int[] { 300, 450, 600 });

    //Defualt
    public static List<int> defaultRequirements = new List<int>(new int[] { 300, 600, 900 });

    //Unused
    public static List<int> plateMagicRequirements = new List<int>(new int[] { 300, 600, 900 });

    public static List<int> GetLevelRequirements(string levelName)
    {
        switch (levelName.ToLower())
        {
            case "get a grip":
                return getAGripRequirements;
            case "ghost house":
                return ghostHouseRequirements;
            case "icy platforms":
                return icyPlatformsRequirements;
            case "plate magic":
                return plateMagicRequirements;
            case "rotatinator":
                return rotatinatorRequirements;
            case "tutorial basics":
                return tutorialRequirements;
            case "tutorial throw drag wood":
                return tutorial2Requirements;
            default:
                return defaultRequirements;
        }
    }
}
