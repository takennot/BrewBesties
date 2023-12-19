using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using static Resource_Enum;

public class CauldronState : MonoBehaviour
{
    [SerializeField] private GameObject liquidPlane;
    [SerializeField] private ChangePotionColor liquidPlaneChangePotionColor;

    [SerializeField] private GameObject[] slots;
    [SerializeField] private Image[] canvasImageSlots;
    [SerializeField] private GameObject[] canvasImageSlotsBackgrounds;

    [Header("Ingredients")]
    private IngredientAbstract ingredient1;
    private IngredientAbstract ingredient2;
    private IngredientAbstract ingredient3;

    [SerializeField] private Resource_Enum.Ingredient ingredient1type;
    [SerializeField] private Resource_Enum.Ingredient ingredient2type;
    [SerializeField] private Resource_Enum.Ingredient ingredient3type;

    [SerializeField] private bool magic1;
    [SerializeField] private bool magic2;
    [SerializeField] private bool magic3;

    [SerializeField] private int ingredientCount = 0;

    [Header("UI")]
    [SerializeField] private Slider processSlider;
    [SerializeField] private Image processSliderFillArea;
    [SerializeField] private Transform procSlider_Pos;

    [Header("Process")]
    [SerializeField] private float process = 0;
    [SerializeField] private float processToFinishCauldron = 0;
    [SerializeField] private float secondsPerProcess = 7;

    [SerializeField] private bool hasToBeDone = false; //For tutorial

    [Header("Audio")]
    [SerializeField] private AudioClip processDoneClip;
    [SerializeField] private AudioClip PloppAudio;
    [SerializeField] private AudioClip waterDropClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource boilingSource;
    private bool hasPlayedClip = false;

    [SerializeField] bool hasInit = false;

    private bool checkpoint1Reached = false;
    private bool checkpoint2Reached = false;

    [SerializeField] private DropEffectHandeler drop;
    private bool onlyDoOnce = true;

    [Header("Outlines")]
    public OutlineHandler cauldronOutline;
    public OutlineHandler fireOutline;

    // Start is called before the first frame update
    void Start()
    {
        // process
        //EmptyCauldron(); // reset

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasInit)
        {
            Potion potion = EmptyCauldron(); // init reset

            source = GetComponent<AudioSource>();
            drop = GetComponent<DropEffectHandeler>();
            onlyDoOnce = true;

            boilingSource.Play();
        }

        if (FindAnyObjectByType<GameManagerScript>() && !FindAnyObjectByType<GameManagerScript>().runElementsInLevel)
            return;

        processSlider.maxValue = secondsPerProcess * 3;
        process = processSlider.value;

        // if warm
        if (gameObject.GetComponent<FireState>().IsWarm())
        {
            if (process == processToFinishCauldron && processToFinishCauldron > 0)
            {
                // done
                processSliderFillArea.color = Color.green;
               
                if (onlyDoOnce)
                {
                    
                    drop.PlayFinishEffect();
                    onlyDoOnce = false;
                }
               

                if (!hasPlayedClip)
                {
                    source.PlayOneShot(processDoneClip);
                    hasPlayedClip = true;
                }

            } else if (ingredientCount > 0)
            {
                if (processSlider.IsActive() == false)
                {
                    processSlider.gameObject.SetActive(true);
                }

                if (processSlider.value < ingredientCount * secondsPerProcess)
                {
                    processSlider.value += (float)Time.deltaTime;
                }
            }
        }

        /*
        if(gameObject.GetComponent<FireState>().IsWarm() == false || (process == processToFinishCauldron) == false || ingredientCount < 3)
        {
            drop.PlayUnFinishEffect();
        }*/
       

        if (!checkpoint1Reached && processSlider.value >= secondsPerProcess)
        {
            checkpoint1Reached = true;
            source.pitch = 1f;
            source.PlayOneShot(PloppAudio);
        }
        if (!checkpoint2Reached && processSlider.value >= secondsPerProcess * 2)
        {
            checkpoint2Reached = true;
            source.pitch = 1.3f;
            source.PlayOneShot(PloppAudio);
        }
    }

    private void Update()
    {

        if (FindAnyObjectByType<GameManagerScript>() && !FindAnyObjectByType<GameManagerScript>().runElementsInLevel)
            return;

        

        // DEBUGG
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Debug.Log("T");
            EmptyCauldron();
        }

        if (ingredient1 != null)
        {
            ingredient1type = ingredient1.GetIngredientType();
            magic1 = ingredient1.GetIsMagic();
        } else
        {
            ingredient1type = Resource_Enum.Ingredient.Water;
        }

        if (ingredient2 != null)
        {
            ingredient2type = ingredient2.GetIngredientType();
            magic2 = ingredient2.GetIsMagic();
        } else
        {
            ingredient2type = Resource_Enum.Ingredient.Water;
        }

        if (ingredient3 != null)
        {
            ingredient3type = ingredient3.GetIngredientType();
            magic3 = ingredient3.GetIsMagic();
        } else
        {
            ingredient3type = Resource_Enum.Ingredient.Water;
        }

    }

    public bool AddIngredient(Ingredient ingredient)
    {
        Resource_Enum.Ingredient ingredientType = ingredient.GetIngredientType();

        bool success = Add(ingredient);

        if (success)
        {
            if(liquidPlaneChangePotionColor)
                liquidPlaneChangePotionColor.ChangeColor(ingredient.GetColorStr());

            switch (ingredientType)
            {
                case Resource_Enum.Ingredient.Mushroom:
                    drop.PlayEffect("mushroom");
                break;
                case Resource_Enum.Ingredient.MonsterEye:
                    drop.PlayEffect("monstereye");

                    break;
                case Resource_Enum.Ingredient.PixieDust:
                    drop.PlayEffect("pixiedust");
                    break;
                default:
                    if (liquidPlaneChangePotionColor)
                        liquidPlaneChangePotionColor.ChangeColor("blue");
                    drop.PlayEffect(" ");

                    break;
            }

            AddToProcess();
        }

        return success;
    }

    private bool Add(Ingredient ingredient)
    {
        if(GetIngredientCount() < 3)
        {
            foreach (Image slot in canvasImageSlots)
            {
                if (slot != null && slot.gameObject.activeSelf == false)
                {
                    slot.GetComponent<Image>().sprite = ingredient.GetImage();
                    slot.gameObject.SetActive(true);

                    for (int i = 0; i < canvasImageSlots.Length; i++)
                    {
                        if (canvasImageSlots[i] == slot)
                        {
                            canvasImageSlotsBackgrounds[i].SetActive(true);
                        }
                    }

                    ingredientCount++;
                    processSliderFillArea.color = Color.red;

                    if (ingredient1 == null)
                    {
                        ingredient1 = new(ingredient.GetIngredientType(), ingredient.GetIsMagic());
                    }
                    else if (ingredient2 == null)
                    {
                        ingredient2 = new(ingredient.GetIngredientType(), ingredient.GetIsMagic());
                    }
                    else if (ingredient3 == null)
                    {
                        ingredient3 = new(ingredient.GetIngredientType(), ingredient.GetIsMagic());
                    }
                    else
                    {
                        source.PlayOneShot(processDoneClip);
                    }

                    source.PlayOneShot(waterDropClip);

                    return true;
                }

            }
        }
        else
        {
            return false;
        }

        return false;
    }

    private void AddToProcess()
    {
        processToFinishCauldron += 1 * secondsPerProcess;

    }

    public Potion GetPotion()
    {
        bool isDone = processSlider.value >= processSlider.maxValue;
        if (hasToBeDone == true && isDone == false) //For tutorial
        {
            return null;
        }

        source.PlayOneShot(waterDropClip);
        return EmptyCauldron();
    }

    private Potion EmptyCauldron()
    {
        ingredientCount = 0;
        bool isDone = processSlider.value >= processSlider.maxValue;

        if (hasInit && !isDone)
        {
            drop.PlayUnFinishEffect();
        }

        onlyDoOnce = true;
        // process
        ResetCauldronProcess();

        // liquid

        if (liquidPlaneChangePotionColor)
            liquidPlaneChangePotionColor.ChangeColor("blue");

        // slots ingredients
        foreach (Image slot in canvasImageSlots)
        {
            if (slot != null)
            {
                slot.sprite = null;
                slot.gameObject.SetActive(false);
            }
        }

        foreach (GameObject slot in canvasImageSlotsBackgrounds)
        {
            if (slot != null)
            {
                slot.gameObject.SetActive(false);
            }
        }

        IngredientAbstract ingredient1Temp = new();
        IngredientAbstract ingredient2Temp = new();
        IngredientAbstract ingredient3Temp = new();

        if (ingredient1 != null)
        {
            ingredient1Temp = new(ingredient1.GetIngredientType(), ingredient1.GetIsMagic());
            //ingredient1Temp.validIngredient = true;
        }
        if (ingredient2 != null)
        {
            ingredient2Temp = new(ingredient2.GetIngredientType(), ingredient2.GetIsMagic());
            //ingredient2Temp.validIngredient = true;
        }
        if (ingredient3 != null)
        {
            ingredient3Temp = new(ingredient3.GetIngredientType(), ingredient3.GetIsMagic());
            //ingredient3Temp.validIngredient = true;
        }

        ingredient1 = null;
        ingredient2 = null;
        ingredient3 = null;

        if (!hasInit)
        {
            hasInit = true;
        }

        return new(ingredient1Temp, ingredient2Temp, ingredient3Temp, isDone);

    }

    private void ResetCauldronProcess()
    {
        processSlider.value = 0;
        process = 0;
        processToFinishCauldron = 0;
        processSlider.gameObject.SetActive(false);
        processSliderFillArea.color = Color.red;
        hasPlayedClip = false;
    }

    public bool paused = false;

    public void Pause()
    {
        Debug.Log("Pasue Cauldron sounds");
        if (FindAnyObjectByType<GameManagerScript>().runElementsInLevel)
        {
            UnPauseCauldron();
        }
        else if (!FindAnyObjectByType<GameManagerScript>().runElementsInLevel)
        {
            PauseCauldron();
        }
    }

    private void PauseCauldron()
    {
        if (paused) return;

        AudioSource[] audiosources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource audiosource in audiosources)
        {
            audiosource.Pause();
        }

        paused = true;
    }

    private void UnPauseCauldron()
    {
        if (!paused) return;

        AudioSource[] audiosources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource audiosource in audiosources)
        {
            audiosource.UnPause();
        }

        paused = false;
    }

    // UI
    public void SetIngredientSlots(Camera cam)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            canvasImageSlotsBackgrounds[i].transform.position = cam.WorldToScreenPoint(slots[i].transform.position);
        }
    }

    public int GetIngredientCount()
    {
        return ingredientCount;
    }

    public Transform GetProcessSliderTransform()
    {
        return procSlider_Pos;
    }

    public Slider GetSlider()
    {
        return processSlider;
    }

    public IngredientAbstract GetIngredientAtIndex(int index)
    {
        switch (index)
        {
            case 0:
            return ingredient1;
            case 1:
            return ingredient2;
            case 2:
            return ingredient3;
            default:
            return null;
        }
    }

    public OutlineHandler SetUpAndGetCauldronOutline()
    {
        cauldronOutline.enabled = true;
        fireOutline.enabled = false;

        return cauldronOutline;
    }
    public OutlineHandler SetUpAndGetFireOutline()
    {
        fireOutline.enabled = true;
        cauldronOutline.enabled = false;

        return fireOutline;
    }

}