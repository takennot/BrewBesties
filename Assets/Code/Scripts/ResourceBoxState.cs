using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBoxState : MonoBehaviour
{
    [SerializeField] private GameObject ResourceImage;
    [SerializeField] private Resource_Enum.Resource resource;

    public Material mushroom;
    public Material eye; //flytta till ingredienser sen!!!!

    public bool shouldWorkWithPickUp = true;

    // Start is called before the first frame update
    void Start()
    {
        switch (resource)
        {
            case Resource_Enum.Resource.Mushroom:
                if (mushroom)
                {
                    ResourceImage.GetComponent<MeshRenderer>().material = mushroom;
                }
                else
                {
                    ResourceImage.GetComponent<MeshRenderer>().material.color = Color.red;
                }

                break;
            case Resource_Enum.Resource.MonsterEye:
                if (eye)
                {
                    ResourceImage.GetComponent<MeshRenderer>().material = eye;
                }
                else
                {
                    ResourceImage.GetComponent<MeshRenderer>().material.color = Color.white;
                }

                break;
            case Resource_Enum.Resource.PixieDust:
                ResourceImage.GetComponent<MeshRenderer>().material.color = Color.magenta;

                break;
            case Resource_Enum.Resource.Bottle:
                ResourceImage.GetComponent<MeshRenderer>().material.color = Color.blue;

                break;

            case Resource_Enum.Resource.FireWood:
                ResourceImage.GetComponent<MeshRenderer>().material.color = Color.black;

                break;
            default:
                ResourceImage.GetComponent<MeshRenderer>().material.color = Color.gray;

                break;
        }
    }

    public Resource_Enum.Resource GetResource()
    {
        return resource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
