using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAbility : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    [SerializeField] GameObject firePoint;
       
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        lr.SetPosition(0, firePoint.transform.position);
        RaycastHit hit;
        // find where it hits ass postion
        // if hitted somthing
        //lr.SetPosition(1,(hit.point) posetion den träffade i)
    }
}
