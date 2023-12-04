using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject popUpPrefab;
    public void SpawnPopUp(Camera cam, Transform theTransform, string text, Color color)
    {
        GameObject popUp = Instantiate<GameObject>(popUpPrefab);

        popUp.GetComponent<PopUpText>().PopUpTextSetup(theTransform, cam);
        popUp.GetComponent<PopUpText>().SetText(text);
        popUp.GetComponent<PopUpText>().SetTextColor(color);
        popUp.GetComponent<PopUpText>().PopUp();
    }
}
