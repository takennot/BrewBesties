using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel : MonoBehaviour
{
    public bool hasCompletedTutorial;
    public bool isActive;

    [SerializeField] private float delayTimeHide = 1.0f;
    [SerializeField] private float delayTimeShow = 2.0f;

    [SerializeField] private float highlightWidth = 0.25f;

    [SerializeField] private PlayerScript player;

    [SerializeField] private bool hasDonePickup, hasDonePickup2, hasDonePickup3, hasDonePickup4, hasDonePickupBottle;
    [SerializeField] private bool hasDoneCounterPlace;
    [SerializeField] private bool hasDoneCounterPickUp;
    [SerializeField] private bool hasDoneWorkstationPlace;
    [SerializeField] private bool hasDonePutInCauldron, hasDonePutInCauldron2, hasDonePutInCauldron3;
    [SerializeField] private bool hasDoneProcess;
    [SerializeField] private bool hasDonePutProcessedInCauldron;
    [SerializeField] private bool hasDonePickUpWood;
    [SerializeField] private bool hasDoneLitCauldron;
    [SerializeField] private bool hasDoneSawWood;
    [SerializeField] private bool hasDoneFillBottle;
    [SerializeField] private bool hasDoneGoal;
    [SerializeField] private bool hasDoneMagicPotion;
    [SerializeField] private bool hasDoneServeMagicPotion;
    [SerializeField] private bool hasDoneServeFinalPotion;

    [Header("UI")]
    [SerializeField] private Color colorCompleted = Color.green;
    private Color startColor;

    [Header("Mission Text")]
    [SerializeField] private TMP_Text textMainMission;

    [SerializeField] private TMP_Text textPickup;
    [SerializeField] private TMP_Text textCounterPlace;
    [SerializeField] private TMP_Text textCounterPickUp;
    [SerializeField] private TMP_Text textPutInCauldron;

    [SerializeField] private TMP_Text textPickup2;
    [SerializeField] private TMP_Text textPutInCauldron2;
    [SerializeField] private TMP_Text textPickup3;
    [SerializeField] private TMP_Text textPutInCauldron3;

    [SerializeField] private TMP_Text textSawWood;
    [SerializeField] private TMP_Text textPickUpWood;
    [SerializeField] private TMP_Text textLitCauldron;

    [SerializeField] private TMP_Text textPickupBottle;
    [SerializeField] private TMP_Text textFillBottle;
    [SerializeField] private TMP_Text textGoal;

    [SerializeField] private TMP_Text textPickup4;
    [SerializeField] private TMP_Text textWorkstationPlace;
    [SerializeField] private TMP_Text textProcess;

    [SerializeField] private TMP_Text textPutProcessedInCauldron;
    [SerializeField] private TMP_Text textMagicPotion;
    [SerializeField] private TMP_Text textServeMagicPotion;

    [SerializeField] private TMP_Text textServeFinalPotion;

    [Header("Scripts")]
    [SerializeField] private AnimationScale animScale;
    [SerializeField] private CounterState[] counterStates;


    private CounterState counterState;
    //private CounterState counterWithFirstItem = null;
    private CauldronState cauldronState = null;
    private Workstation workstationState = null;
    private GoalTutorial goalState = null;

    [Header("Game Objects")]
    [SerializeField] private GameObject saw;
    [SerializeField] private GameObject resourceBox;
    [SerializeField] private GameObject cauldron;
    [SerializeField] private GameObject counter;
    [SerializeField] private GameObject workstation;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject bottle;
    [SerializeField] private GameObject[] wallsByGoal;

    [Header("Camera")]
    public Camera camera;

    private void Awake()
    {
        counterStates = FindObjectsOfType<CounterState>();
        camera = Camera.current;
    }

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        cauldronState = cauldron.GetComponent<CauldronState>();
        workstationState = workstation.GetComponent<Workstation>();
        counterState = counter.GetComponent<CounterState>();

        //Objects that should start as scale 0,0,0
        cauldron.transform.localScale = new Vector3(0, 0, 0);

        counter.transform.localScale = new Vector3(0, 0, 0);

        saw.transform.localScale = new Vector3(0, 0, 0);
        saw.GetComponent<Collider>().enabled = false;

        workstation.transform.localScale = new Vector3(0, 0, 0);
        workstation.GetComponent<Collider>().enabled = false;

        bottle.transform.localScale = new Vector3(0, 0, 0);
        bottle.GetComponent<Collider>().enabled = false;

        goal.transform.localScale = new Vector3(0, 0, 0);
        goalState = goal.GetComponentInChildren<GoalTutorial>();

        textPickup.outlineWidth = 0.25f;

        //Hide mission texts
        SetShowText(new TMP_Text[] { textPickup2, textPutInCauldron2, textPickup3, textPutInCauldron3, 
            textSawWood, textPickUpWood, textLitCauldron, textPickupBottle, textFillBottle, textGoal, textPickup4, 
            textWorkstationPlace, textProcess, textPutProcessedInCauldron, textMagicPotion, textServeMagicPotion, 
            textServeFinalPotion }, false);

        startColor = textMainMission.color;
        SetHighlightedText(textPickup, null);

        resourceBox.GetComponentInChildren<Outline>().ShowManualOutline();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDonePickup) MissionPickup();
        else if (!hasDoneCounterPlace) MissionCounterPlace();
        else if (!hasDoneCounterPickUp) MissionCounterPickup();
        else if (!hasDonePutInCauldron) MissionPutInCauldron();
        else if (!hasDonePickup2) MissionPickup2();
        else if (!hasDonePutInCauldron2) MissionPutInCauldron2();
        else if (!hasDonePickup3) MissionPickup3();
        else if (!hasDonePutInCauldron3) MissionPutInCauldron3();
        else if (!hasDoneSawWood) MissionSawWood();
        else if (!hasDonePickUpWood) MissionPickUpWood();
        else if (!hasDoneLitCauldron) MissionLitCauldron();
        else if (!hasDonePickupBottle) MissionPickUpBottle();
        else if (!hasDoneFillBottle) MissionFillBottle();
        else if (!hasDoneGoal) MissionGoal();
        else if (!hasDonePickup4) MissionPickup4();
        else if (!hasDoneWorkstationPlace) MissionPlaceOnWorkstation();
        else if (!hasDoneProcess) MissionProcess();
        else if (!hasDonePutProcessedInCauldron) MissionPutProcessedInCauldron();
        else if (!hasDoneMagicPotion) MissionMagicPotion();
        else if (!hasDoneServeMagicPotion) MissionServeMagicPotion();
        else if (!hasDoneServeFinalPotion) MissionServeFinalPotion();
        else hasCompletedTutorial = true;
    }

    public bool GetHasCompletedTutorial()
    {
        return hasCompletedTutorial;
    }

    private void MissionPickup()
    {
        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Ingredient"))
            return;

        if (player.GetObjectInHands().GetComponent<Ingredient>() == null)
            return;

        animScale.ScaleUp(counter);
        camera.GetComponent<CameraUIManager>().Initilize();

        textPickup.color = colorCompleted;
        SetHighlightedText(textCounterPlace, textPickup);
        resourceBox.GetComponentInChildren<Outline>().HideManualOutline();
        counter.GetComponentInChildren<Outline>().ShowManualOutline();

        Debug.Log("Completed Pickup Mission");
        hasDonePickup = true;
    }

    private void MissionCounterPlace()
    {
        if (counterState.storedItem != null)
        {
            textCounterPlace.color = colorCompleted;
            SetHighlightedText(textCounterPickUp, textCounterPlace);

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Counter Place Mission");
            hasDoneCounterPlace = true;
        }

    }

    private void MissionCounterPickup()
    {
        if (counterState.storedItem == null)
        {
            textCounterPickUp.color = colorCompleted;
            animScale.ScaleUp(cauldron, new Vector3(2, 2, 2));

            resourceBox.GetComponent<Collider>().enabled = true;

            SetHighlightedText(textPutInCauldron, textCounterPickUp);
            cauldron.GetComponentInChildren<Outline>().ShowManualOutline();
            counter.GetComponentInChildren<Outline>().HideManualOutline();

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Counter Pick up Mission");
            hasDoneCounterPickUp = true;
        }
        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void MissionPutInCauldron()
    {
        if (cauldronState.GetIngredientCount() > 0)
        {
            textPutInCauldron.color = colorCompleted;
            SetHighlightedText(textPickup2, textPutInCauldron);
            cauldron.GetComponentInChildren<Outline>().HideManualOutline();
            resourceBox.GetComponentInChildren<Outline>().ShowManualOutline();

            MissionComplete(new TMP_Text[] { textPickup, textCounterPlace, textCounterPickUp, textPutInCauldron },
                new TMP_Text[] { textPickup2, textPutInCauldron2, textPickup3, textPutInCauldron3 }, "Fill the Cauldron with Ingredients!");

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Put in Cauldron Mission");
            hasDonePutInCauldron = true;
        }
        camera.GetComponent<CameraUIManager>().Initilize();
    }                                    
                                         
    private void MissionPickup2()
    {

        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Ingredient"))
            return;

        if (player.GetObjectInHands().GetComponent<Ingredient>() == null)
            return;

        textPickup2.color = colorCompleted;
        SetHighlightedText(textPutInCauldron2, textPickup2);
        resourceBox.GetComponentInChildren<Outline>().HideManualOutline();
        cauldron.GetComponentInChildren<Outline>().ShowManualOutline();
        
        Debug.Log("Completed Pickup Mission 2");
        camera.GetComponent<CameraUIManager>().Initilize();
        hasDonePickup2 = true;

    }

    private void MissionPutInCauldron2()
    {
        if (cauldronState.GetIngredientCount() > 1)
        {
            textPutInCauldron2.color = colorCompleted;
            SetHighlightedText(textPickup3, textPutInCauldron2);
            cauldron.GetComponentInChildren<Outline>().HideManualOutline();
            resourceBox.GetComponentInChildren<Outline>().ShowManualOutline();

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Put in Cauldron Mission 2");
            hasDonePutInCauldron2 = true;
        }
    }

    private void MissionPickup3()
    {

        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Ingredient"))
            return;

        if (player.GetObjectInHands().GetComponent<Ingredient>() == null)
            return;

        textPickup3.color = colorCompleted;
        SetHighlightedText(textPutInCauldron3, textPickup3);
        resourceBox.GetComponentInChildren<Outline>().HideManualOutline();
        cauldron.GetComponentInChildren<Outline>().ShowManualOutline();

        camera.GetComponent<CameraUIManager>().Initilize();
        Debug.Log("Completed Pickup Mission 3");
        hasDonePickup3 = true;


    }

    private void MissionPutInCauldron3()
    {
        if (cauldronState.GetIngredientCount() > 2)
        {
            textPutInCauldron3.color = colorCompleted;
            saw.GetComponent<Collider>().enabled = true;
            animScale.ScaleUp(saw);
            camera.GetComponent<CameraUIManager>().Initilize();

            SetHighlightedText(textSawWood, textPutInCauldron3);
            cauldron.GetComponentInChildren<Outline>().HideManualOutline();
            saw.GetComponentInChildren<Outline>().ShowManualOutline();
            

            MissionComplete(new TMP_Text[] { textPickup2, textPutInCauldron2, textPickup3, textPutInCauldron3 },
                new TMP_Text[] { textSawWood, textPickUpWood, textLitCauldron }, "Give the Cauldron some heat!");

            Debug.Log("Completed Put in Cauldron Mission 3");
            hasDonePutInCauldron3 = true;
        }

        camera.GetComponent<CameraUIManager>().Initilize();

    }


    private void MissionSawWood() {
        if (saw.GetComponent<Saw>().hasSawed)
        {
            textSawWood.color = colorCompleted;
            SetHighlightedText(textPickUpWood, textSawWood);

            camera.GetComponent<CameraUIManager>().Initilize();

            Debug.Log("Completed Saw Wood Mission");
            hasDoneSawWood = true;
        }

        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void MissionPickUpWood() {
        if (player.GetObjectInHands() != null) {
            if (player.GetObjectInHands().CompareTag("Wood")) {
                textPickUpWood.color = colorCompleted;
                SetHighlightedText(textLitCauldron, textPickUpWood);
                saw.GetComponentInChildren<Outline>().HideManualOutline();
                cauldron.GetComponentInChildren<Outline>().ShowManualOutline();

                Debug.Log("Completed Pick Up Wood Mission");
                hasDonePickUpWood = true;
            }
        }

        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void MissionLitCauldron() {
        //NOTICE: Mission asks for wood to be put in cauldronState, but internally the mission checks if the cauldronState is warm enough to process potions
        if (cauldronState.GetComponent<FireState>().IsWarm()) {
            textLitCauldron.color = colorCompleted;
            bottle.GetComponent<Collider>().enabled = true;
            animScale.ScaleUp(bottle);


            SetHighlightedText(textPickupBottle, textLitCauldron);
            cauldron.GetComponentInChildren<Outline>().HideManualOutline();
            bottle.GetComponentInChildren<Outline>().ShowManualOutline();

            MissionComplete(new TMP_Text[] { textSawWood, textPickUpWood, textLitCauldron },
                new TMP_Text[] { textPickupBottle, textFillBottle, textGoal }, "Serve your first customer!");

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Lit Cauldron Mission");
            hasDoneLitCauldron = true;
        }
    }

    private void MissionPickUpBottle()
    {
        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Bottle"))
            return;

        textPickupBottle.color = colorCompleted;
        SetHighlightedText(textFillBottle, textPickupBottle);
        bottle.GetComponentInChildren<Outline>().HideManualOutline();
        cauldron.GetComponentInChildren<Outline>().ShowManualOutline();

        foreach (GameObject wall in wallsByGoal)
        {
            animScale.ScaleDown(wall);
        }
        animScale.ScaleUp(goal, new Vector3(1, 1, 1f));
        camera.GetComponent<CameraUIManager>().Initilize();


        Debug.Log("Completed Pickup Bottle");
        hasDonePickupBottle = true;

    }

    private void MissionFillBottle()
    {
        camera.GetComponent<CameraUIManager>().Initilize();
        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Bottle"))
            return;

        if (player.GetObjectInHands().GetComponent<Bottle>().IsEmpty() == true)
            return;

        textFillBottle.color = colorCompleted;
        SetHighlightedText(textGoal, textFillBottle);
        cauldron.GetComponentInChildren<Outline>().HideManualOutline();
        goal.GetComponentInChildren<Outline>().ShowManualOutline();

        goalState.enabled = true;
        goalState.NewCustomer(new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, false),
                                                        new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, false),
                                                        new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, false));


        camera.GetComponent<CameraUIManager>().Initilize();
        Debug.Log("Completed Pickup Bottle");
        hasDoneFillBottle = true;

    }

    private void MissionGoal()
    {
        camera.GetComponent<CameraUIManager>().Initilize();
        if (goalState.GetCompletedRecipesCount() < 1)
        {
            return;
        }
        textGoal.color = colorCompleted;
        SetHighlightedText(textPickup4, textGoal);
        goal.GetComponentInChildren<Outline>().HideManualOutline();
        resourceBox.GetComponentInChildren<Outline>().ShowManualOutline();

        MissionComplete(new TMP_Text[] { textPickupBottle, textFillBottle, textGoal },
            new TMP_Text[] { textPickup4, textWorkstationPlace, textProcess, textPutProcessedInCauldron }, "Casting spells!");

        camera.GetComponent<CameraUIManager>().Initilize();
        Debug.Log("Completed Goal Mission");
        Debug.Log(goalState.GetCompletedRecipesCount());
        hasDoneGoal = true;

    }

    private void MissionPickup4()
    {
        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Ingredient"))
            return;

        if (player.GetObjectInHands().GetComponent<Ingredient>() == null)
            return;

        textPickup4.color = colorCompleted;
        workstation.GetComponent<Collider>().enabled = true;
        animScale.ScaleUp(workstation);
        camera.GetComponent<CameraUIManager>().Initilize();

        SetHighlightedText(textWorkstationPlace, textPickup4);
        resourceBox.GetComponentInChildren<Outline>().HideManualOutline();
        workstation.GetComponentInChildren<Outline>().ShowManualOutline();

        Debug.Log("Completed Pickup Mission 4");
        hasDonePickup4 = true;
    }
    private void MissionPlaceOnWorkstation()
    {
        if (workstation.GetComponent<CounterState>().storedItem != null)
        {
            textWorkstationPlace.color = colorCompleted;
            SetHighlightedText(textProcess, textWorkstationPlace);

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed Place on workstation Mission");
            hasDoneWorkstationPlace = true;
        }
    }

    private void MissionProcess()
    {
        if (workstationState.GetComponent<CounterState>().storedItem == null)
            return;

        if (workstationState.GetComponent<CounterState>().storedItem.GetComponent<Ingredient>().GetIsMagic() == false)
            return;
            
        textProcess.color = colorCompleted;
        SetHighlightedText(textPutProcessedInCauldron, textProcess);
        workstation.GetComponentInChildren<Outline>().HideManualOutline();
        cauldron.GetComponentInChildren<Outline>().ShowManualOutline();

        camera.GetComponent<CameraUIManager>().Initilize();
        Debug.Log("Completed Process Mission");
        hasDoneProcess = true;
    }

    private void MissionPutProcessedInCauldron()
    {
        if (cauldronState.GetIngredientCount() >= 1)
        {
            IngredientAbstract firstIngredient = cauldronState.GetIngredientAtIndex(0);
            IngredientAbstract secondIngredient = cauldronState.GetIngredientAtIndex(1);
            IngredientAbstract thirdIngredient = cauldronState.GetIngredientAtIndex(2);

            bool isMagic1 = firstIngredient != null ? firstIngredient.GetIsMagic() : false;
            bool isMagic2 = secondIngredient != null ? secondIngredient.GetIsMagic() : false;
            bool isMagic3 = thirdIngredient != null ? thirdIngredient.GetIsMagic() : false;

            if (isMagic1 || isMagic2 || isMagic3)
            {
                textPutProcessedInCauldron.color = colorCompleted;
                SetHighlightedText(textMagicPotion, textPutProcessedInCauldron);
                cauldron.GetComponentInChildren<Outline>().HideManualOutline();

                MissionComplete(new TMP_Text[] { textPickup4, textWorkstationPlace, textProcess, textPutProcessedInCauldron },
                    new TMP_Text[] { textMagicPotion, textServeMagicPotion }, "Serve a Potion with a Magic Ingredient!");
                goalState.NewCustomer(new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, true),
                                                                new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, true),
                                                                new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, true));

                camera.GetComponent<CameraUIManager>().Initilize();
                Debug.Log("Completed Magic in Cauldron Mission");
                hasDonePutProcessedInCauldron = true;
            }
        }
    }

    private void MissionMagicPotion()
    {
        if (player.GetObjectInHands() == null)
            return;

        if (!player.GetObjectInHands().CompareTag("Bottle"))
            return;

        if (player.GetObjectInHands().GetComponent<Bottle>().IsEmpty() == true)
            return;

        Potion potion = player.GetObjectInHands().GetComponent<Bottle>().GetPotion();

        if (potion.ingredient1.GetIsMagic() && potion.ingredient2.GetIsMagic() && potion.ingredient3.GetIsMagic())
        {
            textMagicPotion.color = colorCompleted;
            SetHighlightedText(textServeMagicPotion, textMagicPotion);

            camera.GetComponent<CameraUIManager>().Initilize();
            Debug.Log("Completed aquire magic potion Mission");
            hasDoneMagicPotion = true;
        }
    }

    private void MissionServeMagicPotion()
    {
        if (goalState.GetCompletedRecipesCount() < 2)
        {
            return;
        }
        textServeMagicPotion.color = colorCompleted;
        SetHighlightedText(textServeFinalPotion, textServeMagicPotion);
        MissionComplete(new TMP_Text[] { textPutProcessedInCauldron, textMagicPotion, textServeMagicPotion },
            new TMP_Text[] { textServeFinalPotion }, "Great job! One last order!");

        Debug.Log("Completed Serve Magic Mission");
        Debug.Log(goalState.GetCompletedRecipesCount());

        Invoke("FinalCustomer", 2f);

        hasDoneServeMagicPotion = true;

    }

    private void FinalCustomer()
    {
        goalState.NewCustomer(new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, false),
                                                        new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, true),
                                                        new IngredientAbstract(Resource_Enum.Ingredient.Mushroom, true));

        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void MissionServeFinalPotion()
    {
        if (goalState.GetCompletedRecipesCount() < 3)
        {
            return;
        }
        textServeFinalPotion.color = colorCompleted;
        SetHighlightedText(null, textServeFinalPotion);
        textMainMission.color = colorCompleted;
        Debug.Log("Completed Serve Final Mission");
        hasDoneServeFinalPotion = true;

        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void MissionComplete(TMP_Text[] textsToHide, TMP_Text[] textsToShow, string missionText)
    {
        textMainMission.color = colorCompleted;
        // Add main mission text to the arrays
        TMP_Text[] newHideArray = new TMP_Text[textsToHide.Length + 1];
        TMP_Text[] newShowArray = new TMP_Text[textsToShow.Length + 1];

        for (int i = 0; i < textsToHide.Length; i++)
        {
            newHideArray[i] = textsToHide[i];
        }
        for (int i = 0; i < textsToShow.Length; i++)
        {
            newShowArray[i] = textsToShow[i];
        }

        newHideArray[textsToHide.Length] = textMainMission;
        newShowArray[textsToShow.Length] = textMainMission;

        textsToHide = newHideArray;
        textsToShow = newShowArray;

        StartCoroutine(SetShowTextDelay(textsToHide, false, delayTimeHide));
        StartCoroutine(SetMainText(missionText, (delayTimeHide + delayTimeShow) / 2));
        StartCoroutine(SetShowTextDelay(textsToShow, true, delayTimeShow));

        //Play sound effect here

        camera.GetComponent<CameraUIManager>().Initilize();
    }

    private void SetHighlightedText(TMP_Text textHighlight, TMP_Text textNoHighlight)
    {
        if (textHighlight != null)
        textHighlight.outlineWidth = highlightWidth;

        if (textNoHighlight != null)
        textNoHighlight.outlineWidth = 0f;
    }

    private IEnumerator SetShowTextDelay(TMP_Text[] texts, bool shouldShow, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        foreach (TMP_Text text in texts)
        {
            text.enabled = shouldShow;
        }
    }

    private void SetShowText(TMP_Text[] texts, bool shouldShow)
    {
        foreach (TMP_Text text in texts)
        {
            text.enabled = shouldShow;
        }
    }

    private IEnumerator SetMainText(string missionText, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        textMainMission.text = missionText;
        textMainMission.color = startColor;
    }
}
