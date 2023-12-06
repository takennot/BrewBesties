using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workstation : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private CounterState counterState;
    [SerializeField] private Ingredient ingredientOnStation = null;

    [Header("Speed")]
    [SerializeField] private float sliderValue = 0;

    [SerializeField] private float maxWorkProcess = 5;

    [Header("Refs")]
    [SerializeField] private Slider magicSlider;
    [SerializeField] private Image magicSliderFillArea;
    [SerializeField] private Transform magicSlider_Pos;

    public bool doWork = false;

    [Header("Sound")]
    [SerializeField] private AudioClip ProcessingSound;
    [SerializeField] private AudioClip doneSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceProcess;
    [SerializeField] MagicController magicController;

    

    // Start is called before the first frame update
    void Start()
    {
        //magicController = GetComponent<MagicController>();
        magicSlider.maxValue = maxWorkProcess;
        magicSlider.value = 0;

        magicSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (magicSlider.value == magicSlider.maxValue)
        {
            FinishWork();
        }
        if (counterState.storedItem != null)
        {
            ingredientOnStation = counterState.storedItem.GetComponent<Ingredient>();
        }
        else
        {
            ingredientOnStation = null;
            magicSlider.gameObject.SetActive(false);
            magicSlider.value = 0;

           //magicController.DestoryParticla();
        }

        if(doWork && ingredientOnStation != null && ingredientOnStation.GetIsMagic() == false)
        {
            magicSlider.gameObject.SetActive(true);
            

            if (!sourceProcess.isPlaying)
                sourceProcess.Play();

            
            magicSlider.value += Time.deltaTime;
            sliderValue = magicSlider.value;
            magicController.MagicOnIngridanse();
            magicController.CreatePartical();
            magicController.onlyOnePartical = false;
            magicController.createOnce = false;


        }
        else
        {
            magicController.onlyOnePartical = true;
            magicController.createOnce = true;
            magicController.DestoryParticla();
        }
    }

    public void DoWorkProcess(PlayerScript thisPlayer)
    {
        
        doWork = true;
    }

    public void StopWorkProcess(PlayerScript thisPlayer)
    {
        doWork = false;
        sourceProcess.Pause();
    }

    private void FinishWork()
    {
        source.PlayOneShot(doneSound);
        sourceProcess.Stop();

        magicSlider.value = 0;
        magicSlider.gameObject.SetActive(false);
        counterState.storedItem.GetComponent<Ingredient>().Magicify();
        magicController.DestoryParticla();
        
    }

    public Transform GetMagicSliderTransform()
    {
        return magicSlider_Pos;
    }
    public Slider GetSlider()
    {
        return magicSlider;
    }
    public GameObject GetIngridiense()
    {
        return ingredientOnStation.gameObject;
    }
}
