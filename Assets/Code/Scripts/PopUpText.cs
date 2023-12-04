using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private TMP_Text popUpText;
    [SerializeField] private GameObject popUpTextParent;
    [SerializeField] private Animator animatorText; //Animate bool

    public bool animate = false;

    public void PopUpTextSetup(Transform posInWorld, Camera cam)
    {
        Vector3 newTransform = cam.WorldToScreenPoint(posInWorld.position);
        popUpTextParent.transform.position = newTransform;

       // animatorText.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (animate)
        {
            animate = false;
            PopUp();
        }

        
    }

    public void PopUp()
    {
        animatorText.SetBool("Animate", true);

        StartCoroutine(PopUpDestroy());
    }

    private IEnumerator PopUpDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void SetTextColor(Color color)
    {
        popUpText.color = color;
    }

    public void SetText(string text)
    {
        popUpText.text = text;
    }
}
