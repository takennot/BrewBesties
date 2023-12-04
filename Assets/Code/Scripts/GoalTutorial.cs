using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting.Antlr3.Runtime;

public class GoalTutorial : MonoBehaviour, GoalInterface
{
    [SerializeField] private CounterState counter;
    [SerializeField] private int scoreTotal = 0;
    [SerializeField] private int recipesCompletedCount = 0;

    private AudioSource source;
    [SerializeField] private AudioClip completedClip;
    [SerializeField] private AudioClip notCompletedClip;

    [Header("Seats")]
    [SerializeField] private Transform seat1;

    [Header("Customers")]
    public GameObject customerPrefab;
    [SerializeField] private CustomerManager customer1;
    [SerializeField] private int amountOfCustomers;

    private float timer = 0;

    public Camera cam;
    private int PlayersCollidingWIth = 0;

    public Sprite spriteMushroom { get; private set; }
    public Sprite magicSpriteMushroom { get; private set; }
    public Sprite spriteEye { get; private set; }
    public Sprite magicSpriteEye { get; private set; }

    public SpriteManager spriteManager;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        counter = GetComponent<CounterState>();
        //text.text = "Recipes Completed:";
        scoreTotal = 0;

        source = GetComponent<AudioSource>();
        source.clip = completedClip;

        //NewCustomer();

        spriteManager = FindObjectOfType<SpriteManager>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter.storedItem != null)
        {

            if (counter.storedItem.GetComponent<Bottle>())
            {
                Bottle incomingBottle = counter.storedItem.GetComponent<Bottle>();
                Potion incomingPotion = incomingBottle.GetPotion();
                if (incomingPotion.isPotionDone)
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
                            recipesCompletedCount++;
                        }

                        Debug.Log("satisfied1:" + satisfied);
                    }


                    if (foundSatisfiedCustomer)
                    {
                        switch (satisfiedCustomer)
                        {
                            case 1:
                            ServeCustomer(customer1);

                            break;
                            default:
                            break;
                        }
                    } else
                    {
                        Debug.Log("Fail potion");
                        FailPotion();

                    }

                } else
                {
                    FailPotion();
                }

                Destroy(counter.storedItem);
                counter.storedItem = null;
            }
        }

        // customer trigger stuff

        if (PlayersCollidingWIth > 0)
        {
            if (customer1)
            {
                customer1.ShowSpeechBubble();
            }
        } else
        {
            if (customer1)
            {
                customer1.HideSpeechBubble();
            }
        }
    }

    private void FailPotion()
    {
        source.PlayOneShot(notCompletedClip);
    }

    private void ServeCustomer(CustomerManager customer)
    {
        amountOfCustomers--;

        source.PlayOneShot(completedClip);

        //GivePoints(CalculatePoints(customer));
        Destroy(customer.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            PlayersCollidingWIth++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            PlayersCollidingWIth--;
        }
    }

    public void NewCustomer(IngredientAbstract ingredient1, IngredientAbstract ingredient2, IngredientAbstract ingredient3)
    {
        GameObject customer;
        cam = Camera.main;
        if (customer1 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat1.transform.position;

            customer1 = customer.GetComponent<CustomerManager>();
            customer1.SetNewOrderTutorial(this, ingredient1, ingredient2, ingredient3);
            customer1.spriteManager = spriteManager;

            Transform transform1 = customer1.GetSpeechBubbleUISlotTransform();
            Vector3 newTransform = cam.WorldToScreenPoint(transform1.position);

            customer1.SetUISpeechBubblePositionOnScreen(newTransform);
        } else
        {
            return;
        }

        amountOfCustomers++;
        timer = 0;
    }

    public void NewCustomerAnyIngredient()
    {
        GameObject customer;
        cam = Camera.main;
        if (customer1 == null)
        {
            customer = Instantiate<GameObject>(customerPrefab);
            customer.transform.position = seat1.transform.position;

            customer1 = customer.GetComponent<CustomerManager>();
            customer1.SetNewOrderTutorial(this, new IngredientAbstract(), new IngredientAbstract(), new IngredientAbstract());
            customer1.spriteManager = spriteManager;

            Transform transform1 = customer1.GetSpeechBubbleUISlotTransform();
            Vector3 newTransform = cam.WorldToScreenPoint(transform1.position);

            customer1.SetUISpeechBubblePositionOnScreen(newTransform);
        } else
        {
            return;
        }

        amountOfCustomers++;
        timer = 0;
    }

    public int GetCompletedRecipesCount()
    {
        return recipesCompletedCount;
    }

    public int GetScore()
    {
        return scoreTotal;
    }
}
