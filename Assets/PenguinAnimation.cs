using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinAnimation : MonoBehaviour
{
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Animate");
            animator.SetBool("Play", true);
        }
    }
}
