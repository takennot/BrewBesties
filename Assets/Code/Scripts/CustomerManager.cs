using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CustomerManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool leave = false;

    [Header("Patience")]
    [SerializeField] private float patienceTimer;
    private float patienceTimerMax;
    public bool isServed = false;

    private int orderSize;
    [SerializeField] private CustomerOrder customerOrder;

    [Header("Order")]
    [SerializeField] private ParticleSystem newCustomerParticleSystem;
    private GameObject currentGoal;
    public string ingredient1 = " ";
    public string ingredient2 = " ";
    public string ingredient3 = " ";

    public float magicChance = 0.5f; // 0.5 = 50%

    // If this is removed, hell breaks loose. So dont toucH!!! V
    public string ignoreThisString = "FUCK YOU UNITY";

    [Header("UI")]
    public bool showSlots = false;
    [SerializeField] private Transform UISlot;
    [SerializeField] private GameObject UISpeechBubble;

    [SerializeField] private UnityEngine.UI.Image ingredientSlot1;
    [SerializeField] private UnityEngine.UI.Image ingredientSlot2;
    [SerializeField] private UnityEngine.UI.Image ingredientSlot3;

    [Header("Useende")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material materialHappy;
    [SerializeField] private Material materialIrritated;
    [SerializeField] private Material materialAngry;

    public SpriteManager spriteManager;


    [Header("VFX")]
    [SerializeField] private GameObject angryParticalSystem;
    [SerializeField] private GameObject longEffekt;
    [SerializeField] private GameObject happyParticalSystem;
    [SerializeField] private Transform particalPostion;
    private GameObject longParticalEffekt = null;
    private ParticleSystem ps;



    void Start()
    {
        UISpeechBubble.SetActive(false);
    }

    public float timerLeaving = 0;
    Vector3 pos;

    // Update is called once per frame
    void FixedUpdate()
    {

        if (leave)
        {
            //transform.position = Vector3.MoveTowards(transform.position, pos - new Vector3(0, 0, -100), 0.5f);
            //transform.position = Vector3.Lerp(transform.position, pos - new Vector3(0, 0, -5), 0.07f);
            transform.position = Vector3.Lerp(transform.position, transform.position + (currentGoal.transform.forward), 0.07f);

            if (timerLeaving > 1.5f)
            {
                Debug.Log("Destroyed!!!");
                Destroy(gameObject);
            }


            timerLeaving += Time.deltaTime;
            PlayAngryLeaving();

        }
        else
        {
            patienceTimer += Time.deltaTime;
        }
    }

    private void Update()
    {
        /*
        if (patienceTimer > patienceTimerMax * 0.5 && meshRenderer.material != materialAngry)
        {
            //meshRenderer.material = materialAngry;
            if (playParticalOnce)
            {
                PlayParticleSystem("angry");
                StartCoroutine(CountTillParticalDestruction());
                playParticalOnce = false;
            }
        }
        else if (patienceTimer > patienceTimerMax * 0.75 && meshRenderer.material != materialIrritated && meshRenderer.material != materialAngry)
        {
            //meshRenderer.material = materialIrritated;
            if (playParticalTwise)
            {
                PlayParticleSystem("angry");
                StartCoroutine(CountTillParticalDestruction());
                playParticalTwise = false;
            }
        }
        */

    }

    public void SetGhostColor(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    public void SetNewOrder(Goal goal)
    {
        currentGoal = goal.gameObject;

        Debug.Log("NewOrderRuns");
        IngredientAbstract newIngredient1 = GetRandomIngredient(goal.mushroomAllowed, goal.magicMushroomPercent, goal.eyeAllowed, goal.magicEyePercent);
        IngredientAbstract newIngredient2 = GetRandomIngredient(goal.mushroomAllowed, goal.magicMushroomPercent, goal.eyeAllowed, goal.magicEyePercent);
        IngredientAbstract newIngredient3 = GetRandomIngredient(goal.mushroomAllowed, goal.magicMushroomPercent, goal.eyeAllowed, goal.magicEyePercent);

        newIngredient1.SetImages(goal.spriteManager);
        newIngredient2.SetImages(goal.spriteManager);
        newIngredient3.SetImages(goal.spriteManager);

        Debug.Log("3 Ingredient");
        customerOrder = new CustomerOrder(newIngredient1, newIngredient2, newIngredient3);
        ingredient1 = newIngredient1.GetString();
        ingredient2 = newIngredient2.GetString();
        ingredient3 = newIngredient3.GetString();

        ingredientSlot1.sprite = newIngredient1.GetImage();
        ingredientSlot2.sprite = newIngredient2.GetImage();
        ingredientSlot3.sprite = newIngredient3.GetImage();

        pos = transform.position;
    }

    public void SetNewOrderTutorial(GoalTutorial goal, IngredientAbstract newIngredient1, IngredientAbstract newIngredient2, IngredientAbstract newIngredient3)
    {
        newIngredient1.SetImages(goal.spriteManager);
        newIngredient2.SetImages(goal.spriteManager);
        newIngredient3.SetImages(goal.spriteManager);

        Debug.Log("3 Ingredient");
        customerOrder = new CustomerOrder(newIngredient1, newIngredient2, newIngredient3);
        ingredient1 = newIngredient1.GetString();
        ingredient2 = newIngredient2.GetString();
        ingredient3 = newIngredient3.GetString();

        ingredientSlot1.sprite = newIngredient1.GetImage();
        ingredientSlot2.sprite = newIngredient2.GetImage();
        ingredientSlot3.sprite = newIngredient3.GetImage();

    }


    public IngredientAbstract GetRandomIngredient(bool mushR, float magicM, bool eye, float magicEye)
    {
        // var v = Resource_Enum.Ingredient.GetValues(typeof(Ingredient));

        IngredientAbstract ingredient = new();

        ingredient.SetIngredient(Resource_Enum.GetRandomIngredient(mushR, eye));

        if (ingredient.GetIngredientType() == Resource_Enum.Ingredient.Mushroom)
        {
            magicChance = magicM;
        }
        else if (ingredient.GetIngredientType() == Resource_Enum.Ingredient.MonsterEye)
        {
            magicChance = magicEye;
        }

        float random = Random.Range(0f, 1f); //0-1

        if (random < magicChance)
        {
            ingredient.SetMagic(true);
        }

        return ingredient;
    }

    public void ShowSpeechBubble()
    {
        if (showSlots)
            UISpeechBubble.SetActive(true);

        if (newCustomerParticleSystem.isPlaying)
            newCustomerParticleSystem.Stop();
    }
    public void HideSpeechBubble()
    {
        UISpeechBubble.SetActive(false);
    }
    public Transform GetSpeechBubbleUISlotTransform()
    {
        return UISlot;
    }
    public void SetUISpeechBubblePositionOnScreen(Vector3 newPosition)
    {
        UISpeechBubble.transform.position = newPosition;
    }
    /// <summary>
    /// Returns a customer order
    /// </summary>
    /// <returns>CustomerOrder object</returns>
    public CustomerOrder GetOrder()
    {
        return customerOrder;
    }


    /// <summary>
    /// Sets <c>leave</c> to true thus initializing the whole "leaving" segment.
    /// De-attach customer before calling the method!!!
    /// </summary>

    
    public void LeaveGoal(bool happy)
    {
        Debug.Log("LEaveeee goooal");

        if (happy)
        {
            PlayHappy();
        }
        else
        {
            PlayAngryLeaving();
        }

        leave = true;
    }
    
    public bool IsCustomerSatisfied(Potion incomingPotion)
    {
        IngredientAbstract[] order = customerOrder.GetIngredients();

        bool ingredient1Match = false;
        bool ingredient2Match = false;
        bool ingredient3Match = false;

        Debug.Log("_______________________________");

        foreach (IngredientAbstract ingredient in order)
        {
            bool matchFOund = false;

            if (!matchFOund && ingredient1Match == false && incomingPotion.ingredient1 != null && (ingredient.GetIngredientType() == incomingPotion.ingredient1.GetIngredientType()) && (ingredient.GetIsMagic() == incomingPotion.ingredient1.GetIsMagic()))
            {
                Debug.Log("MatchFound1");
                ingredient1Match = true;

                matchFOund = true;
            }

            if (!matchFOund && ingredient2Match == false && incomingPotion.ingredient2 != null && (ingredient.GetIngredientType() == incomingPotion.ingredient2.GetIngredientType()) && (ingredient.GetIsMagic() == incomingPotion.ingredient2.GetIsMagic()))
            {
                Debug.Log("MatchFound2");
                ingredient2Match = true;

                matchFOund = true;
            }

            if (!matchFOund && ingredient3Match == false && incomingPotion.ingredient3 != null && (ingredient.GetIngredientType() == incomingPotion.ingredient3.GetIngredientType()) && (ingredient.GetIsMagic() == incomingPotion.ingredient3.GetIsMagic()))
            {
                Debug.Log("MatchFound3");
                ingredient3Match = true;

                matchFOund = true;
            }
        }

        Debug.Log("Match: " + ingredient1Match + " - " + ingredient2Match + " - " + ingredient3Match);

        Debug.Log("_______________________________");

        if (customerOrder.GetAmountOfIngredients() == 1)
        {
            return ingredient1Match;
        }
        else if (customerOrder.GetAmountOfIngredients() == 2)
        {
            return ingredient1Match && ingredient2Match;
        }
        else if (customerOrder.GetAmountOfIngredients() == 3)
        {
            return ingredient1Match && ingredient2Match && ingredient3Match;
        }
        else
        {
            return false;
        }

    }

    public float GetPatienceTimer()
    {
        return patienceTimer;
    }
    /*
    private void PlayParticleSystem(string mode)
    {
        if(mode.Equals("angry") || mode == "angry")
        {
            //Debug.Log("kommer in i angry");
            playingParticalSystem = Instantiate(angryParticalSystem, particalPostion);

            return;
        }
        if (mode.Equals("happy") || mode == "happy")
        {
            playingParticalSystem = Instantiate(happyParticalSystem, particalPostion);
            return;
        }
    }
    

    private IEnumerator CountTillParticalDestruction()
    {
        yield return new WaitForSeconds(5);
        DestroyParticalPlaying();
    }

    
    private void DestroyParticalPlaying()
    {
        if(playingParticalSystem != null)
        {
            Destroy(playingParticalSystem);
        }
    }
    */
    public void SetPatienceTimerMax(int seconds)
    {
        patienceTimerMax = seconds;
    }

    public void PlayHappy()
    {
        GameObject gb = Instantiate(happyParticalSystem, particalPostion);
        Destroy(gb, 5);
    }

    public void PlayAngryLeaving()
    {
        /*
        GameObject gb = Instantiate(angryParticalSystem, particalPostion);
        Destroy(gb, 5);
        */
        StartCoroutine(createAngryEffekt());
    }

    private IEnumerator createAngryEffekt()
    {
        yield return new WaitForSeconds(1f);
        GameObject gb = Instantiate(angryParticalSystem, particalPostion);
        Destroy(gb, 2.5f);
    }


    public void LongTermnAngry(float timeGhostHasBeenAngry)
    {
        if (longParticalEffekt != null)
        {
            InstancetLongTermEffekt();
        }

        var em = ps.emission;
        em.enabled = true;
        em.rateOverTime = (timeGhostHasBeenAngry / 2f);

    }

    private void InstancetLongTermEffekt()
    {
        longParticalEffekt = Instantiate(longEffekt, particalPostion);
        ps = longParticalEffekt.GetComponentInChildren<ParticleSystem>();
    }


    //public void SetIrritatedAtSeconds(int seconds)
    //{
    //    irritatedAtSeconds = seconds;
    //}
    //public void SetAngryAtSeconds(int seconds)
    //{
    //    angryAtSeconds = seconds;
    //}
    //public int GetIrritatedAtSeconds()
    //{
    //    return irritatedAtSeconds;
    //}
    //public int GetAngryAtSeconds()
    //{
    //    return angryAtSeconds;
    //}
}