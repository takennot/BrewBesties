using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIManager : MonoBehaviour
{
    // [SerializeField]
    public CauldronState[] cauldrons;
    public FireState[] fireStates;
    public Saw[] saws;
    public Workstation[] workstations;

    public bool initilized = false;
    public bool shouldUpdateUIPos = false;

    private Camera cam;
    private CameraSway cameraSway;

    // Start is called before the first frame update
    void Start()
    {
        cameraSway = GetComponent<CameraSway>();
        Initilize();
    }

    [System.Obsolete]
    private void Awake()
    {
        if(gameObject.active)
            Initilize();
    }

    public void Initilize()
    {
        //Debug.Log("Initilized camera:" + gameObject.name);

        cauldrons = FindObjectsOfType<CauldronState>();
        fireStates = FindObjectsOfType<FireState>();
        saws = FindObjectsOfType<Saw>();
        workstations = FindObjectsOfType<Workstation>();

        // fixa UI positioner
        cam = GetComponent<Camera>();

        // Cauldron
        foreach (CauldronState cauldron in cauldrons)
        {
            Slider sliderCauldron;
            sliderCauldron = cauldron.GetSlider();

            cauldron.SetIngredientSlots(cam); // ingredient slots
            sliderCauldron.transform.position = cam.WorldToScreenPoint(cauldron.GetProcessSliderTransform().position);
        }

        // fire
        foreach (FireState fireState in fireStates)
        {
            Slider sliderFire;
            sliderFire = fireState.GetSlider();

            sliderFire.transform.position = cam.WorldToScreenPoint(fireState.GetFireSliderTransform().position);
        }

        //saw
        foreach (Saw saw in saws)
        {
            Slider sliderSaw;
            sliderSaw = saw.GetSlider();
            sliderSaw.transform.position = cam.WorldToScreenPoint(saw.GetSawSliderTransform().position);
        }

        // isMagic workstation
        foreach (Workstation workstation in workstations)
        {
            Slider sliderWorkstation;
            sliderWorkstation = workstation.GetSlider();
            sliderWorkstation.transform.position = cam.WorldToScreenPoint(workstation.GetMagicSliderTransform().position);
        }

        initilized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!initilized || !shouldUpdateUIPos)
        {
            return;
        }

        foreach (CauldronState cauldron in cauldrons)
        {
            Slider sliderCauldron;
            sliderCauldron = cauldron.GetSlider();

            cauldron.SetIngredientSlots(cam); // ingredient slots
            sliderCauldron.transform.position = cam.WorldToScreenPoint(cauldron.GetProcessSliderTransform().position);
        }

        // fire
        foreach (FireState fireState in fireStates)
        {
            Slider sliderFire;
            sliderFire = fireState.GetSlider();

            sliderFire.transform.position = cam.WorldToScreenPoint(fireState.GetFireSliderTransform().position);
        }

        //saw
        foreach (Saw saw in saws)
        {
            Slider sliderSaw;
            sliderSaw = saw.GetSlider();
            sliderSaw.transform.position = cam.WorldToScreenPoint(saw.GetSawSliderTransform().position);
        }

        // isMagic workstation
        foreach (Workstation workstation in workstations)
        {
            Slider sliderWorkstation;
            sliderWorkstation = workstation.GetSlider();
            sliderWorkstation.transform.position = cam.WorldToScreenPoint(workstation.GetMagicSliderTransform().position);
        }
    }
}
