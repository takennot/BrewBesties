using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting.Antlr3.Runtime;

public class Goal : MonoBehaviour, GoalInterface
{
    [Header("Idk")]
    [SerializeField] private CounterState counter;
    //[SerializeField] private TMP_Text text;
    [SerializeField] private int scoreTotal = 0;

    [Header("Audio")]
    private AudioSource source;
    [SerializeField] private AudioClip completedClip;
    [SerializeField] private AudioClip notCompletedClip;
    [SerializeField] private AudioClip newCustomerClip;

    [Header("Scoring")]
    [SerializeField] private int scoreBase;
    [SerializeField] private int scoreAddIfHappy = 20;
    [SerializeField] private int magicScore = 5;
    [SerializeField] private int scoreAngryLoseMax;
    //[SerializeField] private int scoreLoseWrongPotion = -50;
    [SerializeField] private int firstOrderBonus = 20;

    [SerializeField] private PopUpManager popUpManager;

    [Header("Customer times")]
    [SerializeField] private int irritatedAtSeconds = 20;
    [SerializeField] private int angryAtSeconds = 45;
    [SerializeField] private int leaveAtSeconds;

    [Header("Seats")]
    [SerializeField] private Transform seat1;
    [SerializeField] private Transform seat2;
    [SerializeField] private Transform seat3;

    // wait to update stuff
    private bool waitToUpdateOrders = false;
    private bool readyToUpdate = true;
    private float readyToUpdateTimer = 0;

    private Vector3 seatOffset = new Vector3(0, 1, 0);

    [Header("Customers")]
    public GameObject customerPrefab;
    [SerializeField] private CustomerManager customer1;
    [SerializeField] private CustomerManager customer2;
    [SerializeField] private CustomerManager customer3;
    public int amountOfCustomers;

    [SerializeField] private float timeForNextCustomer = 30;
    [SerializeField] private float waitOffsetFromStart = 5;
    private float timer = 0;

    [Header("Orders")]
    public bool mushroomAllowed;
    [Range(0.0f, 1.0f)] public float magicMushroomPercent;

    public bool eyeAllowed;
    [Range(0.0f, 1.0f)] public float magicEyePercent;

    [Header("Orders UI")]
    [SerializeField] private UIOrder orderUI1;
    [SerializeField] private UIOrder orderUI2;
    [SerializeField] private UIOrder orderUI3;

    [Header("Tutorial")]
    [SerializeField] private bool startActivated = true;
    [SerializeField] private bool showScoreUI = true;
    [SerializeField] private int recipesCompleted = 0;
    
    [Header("Other")]
    [SerializeField] private Camera cam;
    private int playersCollidingWIth = 0;

    //[Header("Sprites")]
    public Sprite spriteMushroom { get; set; }
    public Sprite magicSpriteMushroom { get; set; }
    public Sprite spriteEye { get; set; }
    public Sprite magicSpriteEye { get; set; }

    public SpriteManager spriteManager;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        counter = GetComponent<CounterState>();
        scoreTotal = 0;

        source = GetComponent<AudioSource>();
        source.clip = completedClip;

        spriteManager = FindObjectOfType<SpriteManager>();

        timer = timeForNextCustomer - waitOffsetFromStart;

        leaveAtSeconds = angryAtSeconds + scoreAngryLoseMax;

        if (!showScoreUI)
        {
            //Do stuff
        }
    }

    private void FixedUpdate()
    {
        //Tutorial won't start until activated
        if (!startActivated) return;

        if (amountOfCustomers < 3) 
        {
            if (timer >= timeForNextCustomer)
            {
                NewCustomer();
            }

            timer += Time.deltaTime;
        }

        // make customer leave if angry
        if(customer1 && customer1.GetPatienceTimer() >= leaveAtSeconds)
        {
            CustomerLeave(customer1);
        }
        if (customer2 && customer2.GetPatienceTimer() >= leaveAtSeconds)
        {
            CustomerLeave(customer2);
        }
        if (customer3 && customer3.GetPatienceTimer() >= leaveAtSeconds)
        {
            CustomerLeave(customer3);
        }

        // fuck if i know. wait a bit for the cutomer to leave before update
        if(waitToUpdateOrders)
            readyToUpdateTimer += Time.deltaTime;

        // Check customers and UI
        if (readyToUpdateTimer > 1.5f)
        {
            waitToUpdateOrders = false;
            readyToUpdateTimer = 0;
            readyToUpdate = true;
            
        }

        if (readyToUpdate)
        {
            UpdateCustomerSeatOrder();
        }
        // -------------------------------------------------------------------------
        
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (counter.storedItem != null) {

            if(counter.storedItem.GetComponent<Bottle>())
            {
                Debug.Log("Bottle delivered");

                Bottle incomingBottle = counter.storedItem.GetComponent<Bottle>();
                Potion incomingPotion = incomingBottle.GetPotion();

                if(incomingPotion != null)
                {
                    Debug.Log("Potion delivered: " + incomingPotion.GetString() );

                    bool foundSatisfiedCustomer = false;
                    int satisfiedCustomer = -1;

                    if (customer1 && !foundSatisfiedCustomer)
                    {
                        bool satisfied = customer1.IsCustomerSatisfied(incomingPotion);

                        if (satisfied)
                        {
                            foundSatisfiedCustomer = true;
                            satisfiedCustomer = 1;
                        }

                        Debug.Log("satisfied1:" + satisfied);
                    }
                    if (customer2 && !foundSatisfiedCustomer)
                    {
                        bool satisfied = customer2.IsCustomerSatisfied(incomingPotion);

                        if (satisfied)
                        {
                            foundSatisfiedCustomer = true;
                            satisfiedCustomer = 2;
                        }
                        Debug.Log("satisfied2:" + satisfied);
                    }
                    if(customer3 && !foundSatisfiedCustomer)
                    {
                        bool satisfied = customer3.IsCustomerSatisfied(incomingPotion);

                        if (satisfied)
                        {
                            foundSatisfiedCustomer = true;
                            satisfiedCustomer = 3;
                        }
                        Debug.Log("satisfied3:" + satisfied);
                    }

                    if(foundSatisfiedCustomer) 
                    {
                        if (incomingPotion.isPotionDone)
                        {
                            switch (satisfiedCustomer)
                            {
                                case 1:
                                    ServeCustomer(customer1);

                                    break;
                                case 2:
                                    ServeCustomer(customer2);

                                    break;
                                case 3:
                                    ServeCustomer(customer3);

                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            FailPotion("Undercooked!");
                        }
                        
                    }
                    else
                    {
                        Debug.Log("Fail potion");
                        FailPotion("Wrong potion!");
                        
                    }

                }
                else
                {
                    FailPotion("Wrong potion!");
                }

                Destroy(counter.storedItem);
                counter.storedItem = null;
            }
        }

        // customer trigger stuff

        if (playersCollidingWIth > 0)
        {
            if (customer1)
            {
                customer1.ShowSpeechBubble();
                orderUI1.SetPaperVisibility(true);
            }
            if (customer2)
            {
                customer2.ShowSpeechBubble();
                orderUI2.SetPaperVisibility(true);
            }
            if (customer3)
            {
                customer3.ShowSpeechBubble();
                orderUI3.SetPaperVisibility(true);
            }   
        }
        else
        {
            if (customer1)
            {
                customer1.HideSpeechBubble();
                orderUI1.SetPaperVisibility(false);
            }
            if (customer2)
            {
                customer2.HideSpeechBubble();
                orderUI2.SetPaperVisibility(false);
            }
            if (customer3)
            {
                customer3.HideSpeechBubble();
                orderUI3.SetPaperVisibility(false);
            }
        }

        //show slider or not
        {
            if (orderUI1.gameObject.active && customer1 == null)
            {
                orderUI1.gameObject.SetActive(false);
            }
            else if (!orderUI1.gameObject.active && customer1 != null)
            {
                orderUI1.gameObject.SetActive(true);
            }

            if (orderUI2.gameObject.active && customer2 == null)
            {
                orderUI2.gameObject.SetActive(false);
            }
            else if (!orderUI2.gameObject.active && customer2 != null)
            {
                orderUI2.gameObject.SetActive(true);
            }

            if (orderUI3.gameObject.active && customer3 == null)
            {
                orderUI3.gameObject.SetActive(false);
            }
            else if (!orderUI3.gameObject.active && customer3 != null)
            {
                orderUI3.gameObject.SetActive(true);
            }
        }

    }

    private void UpdateCustomerSeatOrder()
    {
        // if needed to push forward
        if(customer1 == null)
        {
            if(customer2 != null)
            {
                customer1 = customer2;
                customer2 = null;

                orderUI1.SetCustomer(customer1, leaveAtSeconds, false);
                orderUI2.SetCustomer(customer2, leaveAtSeconds, false);
            }
            else if(customer3 != null)
            {
                customer1 = customer3;
                customer3 = null;

                orderUI1.SetCustomer(customer1, leaveAtSeconds, false);
                orderUI3.SetCustomer(customer3, leaveAtSeconds, false);
            }
        }
        else if (customer2 == null)
        {
            if (customer3 != null)
            {
                customer2 = customer3;
                customer3 = null;

                orderUI2.SetCustomer(customer2, leaveAtSeconds, false);
                orderUI3.SetCustomer(customer3, leaveAtSeconds, false);
            }
        }

        // fix positions ANIMATION NEEDED
        if (customer1)
        {
            customer1.transform.position = seat1.transform.position + seatOffset;
            SetCustomerUI(customer1);
        }

        //orderUI1.SetCustomer(customer1, leaveAtSeconds, false);

        if (customer2)
        {
            customer2.transform.position = seat2.transform.position + seatOffset;
            SetCustomerUI(customer2);
        }

        //orderUI2.SetCustomer(customer2, leaveAtSeconds, false);

        if (customer3)
        {
            customer3.transform.position = seat3.transform.position + seatOffset;
            SetCustomerUI(customer3);
        }

        //orderUI3.SetCustomer(customer3, leaveAtSeconds, false);
    }

    private void FailPotion(string failText)
    {
        source.PlayOneShot(notCompletedClip);

        popUpManager.SpawnPopUp(cam, gameObject.transform, failText, Color.red);

        //GivePoints(scoreLoseWrongPotion);
    }

    private void ServeCustomer(CustomerManager customer)
    {
        amountOfCustomers--;
        customer.isServed = true;
        recipesCompleted++;

        source.PlayOneShot(completedClip);

        GoAwayCustomer(customer);
    }

    private void CustomerLeave(CustomerManager customer)
    {
        // walk away

        amountOfCustomers--;

        source.PlayOneShot(notCompletedClip);

        GoAwayCustomer(customer);
    }

    

    private void GoAwayCustomer(CustomerManager customer)
    {
        if (customer.isServed)
        {
            int points = CalculatePoints(customer);

            popUpManager.SpawnPopUp(cam, customer.gameObject.transform, "+" + points + "p", Color.green);

            GivePoints(points);
        }
        else
        {
            popUpManager.SpawnPopUp(cam, customer.gameObject.transform, "!?#@!", Color.red);
        }

        customer1 = null;

        if (customer == customer1)
            customer1 = null;
        if (customer == customer2)
            customer2 = null;
        if (customer == customer3)
            customer3 = null;

        customer.LeaveGoal();

        //UpdateCustomerSeatOrder();
        readyToUpdate = false;
        waitToUpdateOrders = true;
    }

    private int CalculatePoints(CustomerManager customer)
    {
        int score = scoreBase;

        if (customer.isServed)
        {
            if (customer == customer1)
            {
                Debug.Log("Bonus points!!");
                score += firstOrderBonus;
            }

                // patience calc
            switch (customer.GetPatience())
            {
                case 1: // happy
                    score += scoreAddIfHappy;
                    break;
                case 0: // irritated

                    break;
                case -1: // angry
                    int loseAmount = (int)(customer.GetPatienceTimer() - customer.GetAngryAtSeconds()) / 2; // varannan sekund = /2

                    if (loseAmount > scoreAngryLoseMax)
                    {
                        loseAmount = scoreAngryLoseMax;
                    }

                    score -= loseAmount;

                    break;
                default:

                    break;
            }

            // magic
            IngredientAbstract[] ingredients = customer.GetOrder().GetIngredients();

            if (ingredients[0].GetIsMagic())
            {
                score += magicScore;
            }
            if (ingredients[1].GetIsMagic())
            {
                score += magicScore;
            }
            if (ingredients[2].GetIsMagic())
            {
                score += magicScore;
            }
        }

        

        Debug.Log("ScoreAdd: " +  score);
        return score;
    }

    private void GivePoints(int points)
    {
        Debug.Log("Submitted completed recipe");
        scoreTotal += points;

       

        Debug.Log("Total score: " + scoreTotal);

        //text.text = "Recipes Completed: " + scoreTotal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            playersCollidingWIth++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            playersCollidingWIth--;
        }
    }

    public void NewCustomer() // CODE CAN BE IMPROVED AND SHORTEND
    {
        GameObject customer;
        cam = Camera.main;

        if (customer1 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat1.transform.position + seatOffset;

            customer1 = customer.GetComponent<CustomerManager>();
            customer1.SetNewOrder(this);

            customer1.SetIrritatedAtSeconds(irritatedAtSeconds);
            customer1.SetAngryAtSeconds(angryAtSeconds);
            customer1.spriteManager = spriteManager;

            //damn 1
            SetCustomerUI(customer1);

            // UI Order
            orderUI1.SetCustomer(customer1, leaveAtSeconds, true);
        }
        else if(customer2 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat2.transform.position + seatOffset;

            customer2 = customer.GetComponent<CustomerManager>();
            customer2.SetNewOrder(this);

            customer2.SetIrritatedAtSeconds(irritatedAtSeconds);
            customer2.SetAngryAtSeconds(angryAtSeconds);
            customer2.spriteManager = spriteManager;

            //damn 2
            SetCustomerUI(customer2);

            // UI Order
            orderUI2.SetCustomer(customer2, leaveAtSeconds, true);
        }
        else if(customer3 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat3.transform.position + seatOffset;

            customer3 = customer.GetComponent<CustomerManager>();
            customer3.SetNewOrder(this);

            customer3.SetIrritatedAtSeconds(irritatedAtSeconds);
            customer3.SetAngryAtSeconds(angryAtSeconds);
            customer3.spriteManager = spriteManager;

            //damn 3
            SetCustomerUI(customer3);

            // UI Order
            orderUI3.SetCustomer(customer3, leaveAtSeconds, true);
        }
        else
        {
            return;
        }

        amountOfCustomers++;
        
        timer = 0;
        source.PlayOneShot(newCustomerClip);
    }

    private void SetCustomerUI(CustomerManager customer)
    {
        Transform transform1 = customer.GetSpeechBubbleUISlotTransform();
        Vector3 newTransform = cam.WorldToScreenPoint(transform1.position);

        customer.SetUISpeechBubblePositionOnScreen(newTransform);
    }

    public void SetActivated(bool isActive)
    {
        startActivated = isActive;
    }

    public int GetCompletedRecipesCount() {
        return recipesCompleted;
    }

    public int GetScore()
    {
        return scoreTotal;
    }
}
