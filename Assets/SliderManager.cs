using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider[] sliders; // Reference to all sliders

    public void PlayEntryAnimation(Slider slider)
    {
        // Play the entry animation for the slider
        slider.GetComponent<Animator>().Play("EntryAnimation");
    }

    public void PlayExitAnimation(Slider slider)
    {
        slider.GetComponent<Animator>().Play("ExitAnimation");
    }
}
