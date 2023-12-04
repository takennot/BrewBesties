using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleTextColor : MonoBehaviour
{
    public Toggle toggle;
    public bool startOn;
    public TMP_Text textToChange;
    public Color onColor;
    public Color offColor;

    void Start()
    {
        // Add listener to the toggle
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        textToChange.color = startOn ? onColor : offColor;
    }

    void OnToggleValueChanged(bool isOn)
    {
        // Change the text color based on the toggle state
        textToChange.color = isOn ? onColor : offColor;
    }
}