using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject popUpPrefab;
    private GameObject previousPopUp;
    public void SpawnPopUp(Camera cam, Transform theTransform, string text, Color color)
    {
        if (previousPopUp != null)
        {
            Destroy(previousPopUp);
        }

        GameObject popUp = Instantiate<GameObject>(popUpPrefab);
        previousPopUp = popUp;

        popUp.GetComponent<PopUpText>().PopUpTextSetup(theTransform, cam);
        popUp.GetComponent<PopUpText>().SetText(text);
        popUp.GetComponent<PopUpText>().SetTextColor(color);
        popUp.GetComponent<PopUpText>().PopUp();
    }
}
