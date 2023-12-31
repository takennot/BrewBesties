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
    [SerializeField] private bool activated = true;

    [Header("Refs")]
    [SerializeField] private Camera cam;
    [SerializeField] private CounterState counter;
    [SerializeField] private StartAndEnd startAndEnd;

    [Header("ShowOrdersPlate")]
    [SerializeField] private List<PlayerScript> playersColliding = new List<PlayerScript>();
    [SerializeField] private CollidingTriggerCounting orderPlateTriggerCounting;
    public int playersCollidingWIth = 0;

    [Header("dont edit")]
    [SerializeField] private int scoreTotal = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip completedClip;
    [SerializeField] private AudioClip notCompletedClip;
    [SerializeField] private AudioClip newCustomerClip;

    [Header("Customer times")]
    [SerializeField] private int timeBeforeScorePenalty = 20;
    [SerializeField] private int leaveAfterSeconds = 90;
    [SerializeField] private float patienceFirstInQueueMultiplier = 1;
    [SerializeField] private float patienceSecondInQueueMultiplier = 0.7f;
    [SerializeField] private float patienceThirdInQueueMultiplier = 0.4f;

    [SerializeField] private PopUpManager popUpManager;

    [Header("Scoring")]
    [SerializeField] private int maxPenaltyLoseScore = -25;
    [SerializeField] private int scoreBaseRecipeDone = 70;
    [SerializeField] private int magicIngredientScore = 5;
    [SerializeField] private int notFirstOrderDividePenalty = 2;
    [SerializeField] private int customerLeavesPenalty = -20;

    [Header("Scoring PLayer Adaptability")]
    [SerializeField] private float twoPlayersMultiplier = 1f;
    [SerializeField] private float threePlayersMultiplier = 0.92f;
    [SerializeField] private float fourPlayersMultiplier = 0.885f;

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

    [SerializeField] private float waitOffsetFromStart = 5; // s�tt till timerOffsetStart
    [SerializeField] private float startRateSecondCustomer = 2;
    [SerializeField] private float startRateThirdCustomer = 15;
    [SerializeField] private float newCustomerRate = 5;
    private int customersSpawned = 0;

    private float timer = 0;
    private float timeForNextCustomer = 0;

    [Header("Orders")]
    [Range(0.0f, 1.0f)] public float mushroomInOrderPerc;
    [Range(0.0f, 1.0f)] public float magicMushroomPercent;

    [Range(0.0f, 1.0f)] public float eyeInOrderPerc;
    [Range(0.0f, 1.0f)] public float magicEyePercent;

    [Header("Orders UI")]
    [SerializeField] private UIOrder orderUI1;
    [SerializeField] private UIOrder orderUI2;
    [SerializeField] private UIOrder orderUI3;



    [Header("Tutorial")]
    [SerializeField] private bool showScoreUI = true;
    [SerializeField] private int recipesCompleted = 0;
    [SerializeField] private bool isTutorial = false;



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

        if(startAndEnd)
            waitOffsetFromStart = startAndEnd.timeToWaitForStart;

        //counter = GetComponent<CounterState>();
        scoreTotal = 0;

        source.clip = completedClip;

        spriteManager = FindObjectOfType<SpriteManager>();

        ResetCustomerTimer();

        if (!showScoreUI)
        {
            //Do stuff
        }
    }

    private void FixedUpdate()
    {
        //Tutorial won't start until activated
        if (!activated) return;

        if (amountOfCustomers < 3)
        {
            if (timer >= timeForNextCustomer)
            {
                if(!isTutorial)
                NewCustomer();
            }

            timer += Time.deltaTime;
        }

        // make customer leave if angry
        if (customer1 && customer1.GetPatienceTimer() >= leaveAfterSeconds)
        {
            CustomerLeave(customer1);
        }
        if (customer2 && customer2.GetPatienceTimer() >= leaveAfterSeconds)
        {
            CustomerLeave(customer2);
        }
        if (customer3 && customer3.GetPatienceTimer() >= leaveAfterSeconds)
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
        // CHEATCODE: REMOVE AT RELEASE
        if (Input.GetKeyDown(KeyCode.L))
        {
            GivePoints(50);
        }

        if (!activated) return;

        // is there a bottle on the goal?
        CheckStoredItem();

        //show order or not
        UpdateOrdersActiveOrNot();

        // Are players checking orders atm?
        int playersCount = 0;

        foreach (GameObject gameObject in orderPlateTriggerCounting.GetGameobjectsCollidingWith())
        {
            if (gameObject && gameObject.GetComponent<PlayerScript>())
            {
                playersCount++;
            }
        }

        // show the active orderPapers or not
        if (playersCount > 0)
        {
            if (customer1)
            {
                //customer1.ShowSpeechBubble();
                orderUI1.SetPaperVisibility(true);
            }
            if (customer2)
            {
                //customer2.ShowSpeechBubble();
                orderUI2.SetPaperVisibility(true);
            }
            if (customer3)
            {
                //customer3.ShowSpeechBubble();
                orderUI3.SetPaperVisibility(true);
            }   
        }
        else
        {
            if (customer1)
            {
                //customer1.HideSpeechBubble();
                orderUI1.SetPaperVisibility(false);
            }
            if (customer2)
            {
                //customer2.HideSpeechBubble();
                orderUI2.SetPaperVisibility(false);
            }
            if (customer3)
            {
                //customer3.HideSpeechBubble();
                orderUI3.SetPaperVisibility(false);
            }
        }
    }

    private void UpdateOrdersActiveOrNot()
    {
        if (orderUI1.gameObject.activeSelf && customer1 == null)
        {
            orderUI1.gameObject.SetActive(false);
        }
        else if (!orderUI1.gameObject.activeSelf && customer1 != null)
        {
            orderUI1.gameObject.SetActive(true);
        }

        if (orderUI2.gameObject.activeSelf && customer2 == null)
        {
            orderUI2.gameObject.SetActive(false);
        }
        else if (!orderUI2.gameObject.activeSelf && customer2 != null)
        {
            orderUI2.gameObject.SetActive(true);
        }

        if (orderUI3.gameObject.activeSelf && customer3 == null)
        {
            orderUI3.gameObject.SetActive(false);
        }
        else if (!orderUI3.gameObject.activeSelf && customer3 != null)
        {
            orderUI3.gameObject.SetActive(true);
        }
    }

    private void CheckStoredItem()
    {
        if (counter.storedItem != null)
        {

            if (counter.storedItem.GetComponent<Bottle>())
            {
                Debug.Log("Bottle delivered");

                Bottle incomingBottle = counter.storedItem.GetComponent<Bottle>();
                Potion incomingPotion = incomingBottle.GetPotion();

                if (incomingPotion != null)
                {
                    Debug.Log("Potion delivered: " + incomingPotion.GetString());

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
                    if (customer3 && !foundSatisfiedCustomer)
                    {
                        bool satisfied = customer3.IsCustomerSatisfied(incomingPotion);

                        if (satisfied)
                        {
                            foundSatisfiedCustomer = true;
                            satisfiedCustomer = 3;
                        }
                        Debug.Log("satisfied3:" + satisfied);
                    }

                    if (foundSatisfiedCustomer)
                    {
                        if (incomingPotion.isPotionDone)
                        {
                            switch (satisfiedCustomer)
                            {
                                case 1:
                                    Debug.Log("serve first customer");
                                    ServeCustomer(customer1);

                                    break;
                                case 2:
                                    Debug.Log("serve second customer");
                                    ServeCustomer(customer2);

                                    break;
                                case 3:
                                    Debug.Log("serve third customer");
                                    ServeCustomer(customer3);

                                    break;
                                default:
                                    Debug.Log("serve no one");
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

                orderUI1.SetCustomer(customer1, leaveAfterSeconds, false);
                orderUI2.SetCustomer(customer2, leaveAfterSeconds, false);
            }
            else if(customer3 != null)
            {
                customer1 = customer3;
                customer3 = null;

                orderUI1.SetCustomer(customer1, leaveAfterSeconds, false);
                orderUI3.SetCustomer(customer3, leaveAfterSeconds, false);
            }
        }
        else if (customer2 == null)
        {
            if (customer3 != null)
            {
                customer2 = customer3;
                customer3 = null;

                orderUI2.SetCustomer(customer2, leaveAfterSeconds, false);
                orderUI3.SetCustomer(customer3, leaveAfterSeconds, false);
            }
        }

        // fix positions ANIMATION NEEDED
        if (customer1)
        {
            customer1.transform.position = seat1.transform.position + seatOffset;
            SetCustomerUI(customer1);
            customer1.SetPatienceQueueMultiplier(patienceFirstInQueueMultiplier);
        }

        //orderUI1.SetCustomer(customer1, leaveAtSeconds, false);

        if (customer2)
        {
            customer2.transform.position = seat2.transform.position + seatOffset;
            SetCustomerUI(customer2);
            customer2.SetPatienceQueueMultiplier(patienceSecondInQueueMultiplier);
        }

        //orderUI2.SetCustomer(customer2, leaveAtSeconds, false);

        if (customer3)
        {
            customer3.transform.position = seat3.transform.position + seatOffset;
            SetCustomerUI(customer3);
            customer3.SetPatienceQueueMultiplier(patienceThirdInQueueMultiplier);
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
            Debug.Log("Failed customer: " + customerLeavesPenalty);
            GivePoints(customerLeavesPenalty);

            popUpManager.SpawnPopUp(cam, customer.gameObject.transform, "!?#@! " + customerLeavesPenalty, Color.red);
            //popUpManager.SpawnPopUp(cam, this.transform, "" + customerLeavesPenalty, Color.red);
        }

        //customer1 = null;

        if (customer == customer1)
            customer1 = null;
        if (customer == customer2)
            customer2 = null;
        if (customer == customer3)
            customer3 = null;

        customer.LeaveGoal(customer.isServed);

        //UpdateCustomerSeatOrder();
        readyToUpdate = false;
        waitToUpdateOrders = true;
    }

    private int CalculatePoints(CustomerManager customer)
    {
        int score = 0;

        string processCalc = "";

        if (customer.isServed)
        {
            score = scoreBaseRecipeDone;

            processCalc += " +" + scoreBaseRecipeDone;

            // patience calc
            float customerPatience = customer.GetPatienceTimer();

            int penalty = -0;

            if (customerPatience >= timeBeforeScorePenalty)
            {
                float procent = (customerPatience - timeBeforeScorePenalty) / (leaveAfterSeconds - timeBeforeScorePenalty);
                penalty =  (int) (maxPenaltyLoseScore * procent);
            }

            score += penalty;
            processCalc += " +" + penalty;

            // add magic
            IngredientAbstract[] ingredients = customer.GetOrder().GetIngredients();

            if (ingredients[0].GetIsMagic())
            {
                score += magicIngredientScore;
                processCalc += " +" + magicIngredientScore;
            }
            if (ingredients[1].GetIsMagic())
            {
                score += magicIngredientScore;
                processCalc += " +" + magicIngredientScore;
            }
            if (ingredients[2].GetIsMagic())
            {
                score += magicIngredientScore;
                processCalc += " +" + magicIngredientScore;
            }

            // divide score if not the first one
            if (customer != customer1)
            {
                Debug.Log("Not the first one");
                score /= notFirstOrderDividePenalty;
                processCalc += " /" + notFirstOrderDividePenalty;
            }
        }

        switch (startAndEnd.GetPlayerAmount())
        {
            case 2:
                score = (int) (score * twoPlayersMultiplier);
                processCalc += " *" + twoPlayersMultiplier;
                break;
            case 3:
                score = (int)(score * threePlayersMultiplier);
                processCalc += " *" + threePlayersMultiplier;
                break;
            case 4:
                score = (int)(score * fourPlayersMultiplier);
                processCalc += " *" + fourPlayersMultiplier;
                break;
            default:
                goto case 2;
        }

        processCalc += " = " + score;
        Debug.Log("ScoreAdd: " + processCalc);
        //popUpManager.SpawnPopUp(cam, this.transform, processCalc, Color.blue); // popUp

        return score;
    }

    private void GivePoints(int points)
    {
        Debug.Log("Submitted completed recipe");
        scoreTotal += points;

        if (scoreTotal < 0)
            scoreTotal = 0;

        Debug.Log("Total score: " + scoreTotal);

        //text.text = "Recipes Completed: " + scoreTotal;
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
            customer1.SetPatienceQueueMultiplier(patienceFirstInQueueMultiplier);

            customer1.spriteManager = spriteManager;

            //damn 1
            SetCustomerUI(customer1);

            // UI Order
            orderUI1.SetCustomer(customer1, leaveAfterSeconds, true);
        }
        else if(customer2 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat2.transform.position + seatOffset;

            customer2 = customer.GetComponent<CustomerManager>();
            customer2.SetNewOrder(this);
            customer2.SetPatienceQueueMultiplier(patienceSecondInQueueMultiplier);

            customer2.spriteManager = spriteManager;

            //damn 2
            SetCustomerUI(customer2);

            // UI Order
            orderUI2.SetCustomer(customer2, leaveAfterSeconds, true);
        }
        else if(customer3 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat3.transform.position + seatOffset;

            customer3 = customer.GetComponent<CustomerManager>();
            customer3.SetNewOrder(this);
            customer2.SetPatienceQueueMultiplier(patienceThirdInQueueMultiplier);

            customer3.spriteManager = spriteManager;

            //damn 3
            SetCustomerUI(customer3);

            // UI Order
            orderUI3.SetCustomer(customer3, leaveAfterSeconds, true);
        }
        else
        {
            return;
        }

        customer.GetComponent<CustomerManager>().SetPatienceTimerMax(leaveAfterSeconds);
        amountOfCustomers++;
        customersSpawned++;

        ResetCustomerTimer();
        
        source.PlayOneShot(newCustomerClip);
    }

    private void ResetCustomerTimer()
    {
        switch (customersSpawned)
        {
            case 0:
                timeForNextCustomer = waitOffsetFromStart;
                break;
            case 1:
                timeForNextCustomer = startRateSecondCustomer;
                break;
            case 2:
                timeForNextCustomer = startRateThirdCustomer;
                break;
            default:
                timeForNextCustomer = newCustomerRate;
                break;
        }

        timer = 0;
    }

    private void SetCustomerUI(CustomerManager customer)
    {
        Transform transform1 = customer.GetSpeechBubbleUISlotTransform();
        Vector3 newTransform = cam.WorldToScreenPoint(transform1.position);

        customer.SetUISpeechBubblePositionOnScreen(newTransform);
    }

    public void SetActivated(bool isActive)
    {
        activated = isActive;
    }

    public int GetCompletedRecipesCount() {
        return recipesCompleted;
    }

    public int GetScore()
    {
        return scoreTotal;
    }
}
