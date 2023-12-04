using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] private bool XAxisOnly = false;

    void Start()
    {
        mainCamera = Camera.main;
    }
    void LateUpdate()
    {
        if (XAxisOnly)
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        else
            transform.LookAt(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.up);
    }
}
