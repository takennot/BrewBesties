using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] private bool XAxisOnly = false;
    [SerializeField] private bool ZAxisOnly = false;

    void Start()
    {
        mainCamera = Camera.main;
        if(XAxisOnly == true && ZAxisOnly == true)
        {
            Debug.Log(gameObject.name + ": Only one of XAxisOnly and ZAxisOnly should be true, \n Setting zAxisOnly to false");
            ZAxisOnly = false;
        }
    }
    void LateUpdate()
    {
        if (XAxisOnly)
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        else if (ZAxisOnly)
        {
            transform.LookAt(mainCamera.transform.rotation * Vector3.up + mainCamera.transform.rotation * Vector3.forward, transform.position);
        }

        else
            transform.LookAt(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.up);
    }
}
