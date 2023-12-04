using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject mushroom;
    [SerializeField] private GameObject bottle;
    [SerializeField] private GameObject mosterEye;
    [SerializeField] private GameObject pixieDust;
    [SerializeField] public GameObject fireWood;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.E)) {
            GetResource();
        }*/
    }

    public void GetResource(PlayerScript player)
    {
        GameObject resource;

        Resource_Enum.Resource resourceYä = gameObject.GetComponent<ResourceBoxState>().GetResource();


        if (gameObject.GetComponent<ResourceBoxState>().shouldWorkWithPickUp && resourceYä != Resource_Enum.Resource.FireWood)
        {

            switch (resourceYä)
            {
                case Resource_Enum.Resource.Mushroom:
                    Debug.Log("Mushroom");
                    resource = Instantiate<GameObject>(mushroom);

                    break;
                case Resource_Enum.Resource.MonsterEye:
                    Debug.Log("Eye");
                    resource = Instantiate<GameObject>(mosterEye);

                    break;
                case Resource_Enum.Resource.PixieDust:
                    Debug.Log("Dust");
                    resource = Instantiate<GameObject>(pixieDust);

                    break;
                case Resource_Enum.Resource.Bottle:
                    Debug.Log("Bottle");
                    resource = Instantiate<GameObject>(bottle);

                    break;

                /*case Resource_Enum.Resource.FireWood:
                    
                    Debug.Log("Firewood");
                    resource
                    doIt = false;

                    break;*/
                default:
                    resource = Instantiate<GameObject>(mushroom);

                    break;
            }

            resource.transform.localPosition = this.transform.localPosition + new Vector3(0, 0.5f, 0);
            resource.gameObject.name = resourceYä.ToString();

            player.Grab(resource.GetComponent<Item>());
        }
    }
}
