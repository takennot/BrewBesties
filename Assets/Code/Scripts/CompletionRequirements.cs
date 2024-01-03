using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CompletionRequirements
{
    //seperating by 3 to view easier
    public static List<int> tutorialRequirements = new List<int>(new int[] { 1, 1, 1 });
    public static List<int> getAGripRequirements = new List<int>(new int[] { 235, 470, 600 });
    public static List<int> completionistRequirements = new List<int>(new int[] { 230, 550, 730 });

    public static List<int> tutorial2Requirements = new List<int>(new int[] { 1, 1, 1 });
    public static List<int> threeIslandsRequirements = new List<int>(new int[] { 300, 550, 700 });
    public static List<int> ghostHouseRequirements = new List<int>(new int[] { 355, 500, 640 });


    public static List<int> movingFieldRequirements = new List<int>(new int[] { 435, 770, 925 });
    public static List<int> icyPlatformsRequirements = new List<int>(new int[] { 270, 490, 665 }); 
    public static List<int> rotatinatorRequirements = new List<int>(new int[] { 300, 450, 600 });


    public enum RequirementsForLevels
    {
        TutorialOne,
        GetAGrip,
        Completionist,
        TutorialTwo,
        ThreeIslands,
        GhostHouse,
        MovingField,
        IcyPlatforms,
        Rotatinator
    }

    //Defualt
    public static List<int> defaultRequirements = new List<int>(new int[] { 300, 600, 900 });

    //Unused
    public static List<int> plateMagicRequirements = new List<int>(new int[] { 300, 600, 900 });

    public static List<int> GetLevelRequirements(string levelName)
    {
        switch (levelName.ToLower())
        {
            //bunch 1
            case "tutorial basics":
                return tutorialRequirements;

            case "get a grip":
                return getAGripRequirements;

            case "completionist":
                return completionistRequirements;

            //bunch 2
            case "tutorial throw drag wood":
                return tutorial2Requirements;

            case "three islands":
                return threeIslandsRequirements;

            case "ghost house":
                return ghostHouseRequirements;

            //bunch 3
            case "moving field":
                return movingFieldRequirements;

            case "icy platforms":
                return icyPlatformsRequirements;

            case "rotatinator":
                return rotatinatorRequirements;


            //Unused
            case "plate magic":
                return plateMagicRequirements;

            default:
                return defaultRequirements;
        }
    }
    public static List<int> GetLevelRequirements(RequirementsForLevels levelEnum)
    {
        switch (levelEnum)
        {
            //bunch 1
            case RequirementsForLevels.TutorialOne:
                return tutorialRequirements;

            case RequirementsForLevels.GetAGrip:
                return getAGripRequirements;

            case RequirementsForLevels.Completionist:
                return completionistRequirements;

            //bunch 2
            case RequirementsForLevels.TutorialTwo:
                return tutorial2Requirements;

            case RequirementsForLevels.ThreeIslands:
                return threeIslandsRequirements;

            case RequirementsForLevels.GhostHouse:
                return ghostHouseRequirements;

            //bunch 3
            case RequirementsForLevels.MovingField:
                return movingFieldRequirements;

            case RequirementsForLevels.IcyPlatforms:
                return icyPlatformsRequirements;

            case RequirementsForLevels.Rotatinator:
                return rotatinatorRequirements;

            default:
                return defaultRequirements;
        }
    }
}
