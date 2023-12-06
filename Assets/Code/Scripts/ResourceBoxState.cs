using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBoxState : MonoBehaviour
{
    [SerializeField] private GameObject resourceImage;
    [SerializeField] private Resource_Enum.Resource resource;

    public Material mushroom;
    public Material eye; //flytta till ingredienser sen!!!!

    public bool shouldWorkWithPickUp = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!resourceImage)
            return;

        switch (resource)
        {
            case Resource_Enum.Resource.Mushroom:
                if (mushroom)
                {
                    resourceImage.GetComponent<MeshRenderer>().material = mushroom;
                }
                else
                {
                    resourceImage.GetComponent<MeshRenderer>().material.color = Color.red;
                }

                break;
            case Resource_Enum.Resource.MonsterEye:
                if (eye)
                {
                    resourceImage.GetComponent<MeshRenderer>().material = eye;
                }
                else
                {
                    resourceImage.GetComponent<MeshRenderer>().material.color = Color.white;
                }

                break;
            case Resource_Enum.Resource.PixieDust:
                resourceImage.GetComponent<MeshRenderer>().material.color = Color.magenta;

                break;
            case Resource_Enum.Resource.Bottle:
                resourceImage.GetComponent<MeshRenderer>().material.color = Color.blue;

                break;

            case Resource_Enum.Resource.FireWood:
                resourceImage.GetComponent<MeshRenderer>().material.color = Color.black;

                break;
            default:
                resourceImage.GetComponent<MeshRenderer>().material.color = Color.gray;

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
