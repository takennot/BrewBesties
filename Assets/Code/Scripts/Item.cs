using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private bool isProcessed;

    //[SerializeField] private bool isPickedUp;
    //[SerializeField] private bool isBeingDragged;
    //[SerializeField] private bool isBeingThrown;
    [SerializeField] private ItemStateMachine.ItemState itemState = ItemStateMachine.ItemState.None;

    [HideInInspector] public Quaternion originalRotation;

    [SerializeField] private Resource_Enum.Resource itemType;

    [SerializeField] LayerMask originalLayer;

    [SerializeField] private GameObject poofVFX;

    private bool isUsed = false;

    public PlayerScript lastHeldPlayer;

    private void Awake()
    {
        originalRotation = transform.rotation;

        //Debug.Log(originalRotation.eulerAngles.ToString());
    }

    private void Start()
    {
        //gameObject.layer = originalLayer;

        if (Resource_Enum.IsIngredient(itemType))
        {
            //ingredientType = resourceEnumHandler.GetIngredientFromResource(itemType);
        }
    }
    public Item(bool isPickedUp, bool isProcessed)
    {
        if (isPickedUp)
        {
            itemState = ItemStateMachine.ItemState.IsBeingHeld;
        }
        else
        {
            itemState = ItemStateMachine.ItemState.None;
        }

        this.isProcessed = isProcessed;
    }

    private void Update()
    {
        if (itemState == ItemStateMachine.ItemState.IsBeingHeld)
        {
            gameObject.layer = 2;
        }
        else if(itemState == ItemStateMachine.ItemState.IsBeingDragged || itemState == ItemStateMachine.ItemState.IsBeingThrown)
        {
            gameObject.layer = LayerMask.NameToLayer("IngredientThrown");
        }
        else
        {
            gameObject.layer = 6;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemState == ItemStateMachine.ItemState.IsBeingThrown)
        {
            if (other.gameObject.GetComponent<PlayerScript>() && other.gameObject.GetComponent<PlayerScript>() != lastHeldPlayer)
            {
                Debug.Log("Item HitPlayer");
                other.gameObject.GetComponent<PlayerScript>().Grab(this);
                lastHeldPlayer = other.gameObject.GetComponent<PlayerScript>();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isUsed)
        {
            return;
        }

        Debug.Log("isBeingThrown: " + itemState);

        bool foundCounterOrFloor = false;

        if (itemState == ItemStateMachine.ItemState.IsBeingThrown)
        {
            if (collision.gameObject.GetComponent<CounterState>())
            {
                Debug.Log("found a counter");
                if (collision.gameObject.GetComponent<Goal>())
                {
                    if(itemType == Resource_Enum.Resource.Bottle)
                    {
                        foundCounterOrFloor = PlaceItselfOnCounter(collision.gameObject.GetComponent<CounterState>());
                    }
                }
                else
                {
                    Debug.Log("Place on counter!!!");
                    foundCounterOrFloor = PlaceItselfOnCounter(collision.gameObject.GetComponent<CounterState>());
                }
            }
            else if (collision.gameObject.GetComponent<CauldronState>() && itemState == ItemStateMachine.ItemState.IsBeingThrown)
            {
                //Debug.Log("Collision!!");
                if (gameObject.GetComponent<Ingredient>())
                {
                    CauldronState cauldronState = collision.gameObject.GetComponent<CauldronState>();

                    bool result = cauldronState.AddIngredient(gameObject.GetComponent<Ingredient>());
                    if (result)
                    {
                        isUsed = true;
                        Destroy(this.gameObject);
                    }
                }
            }
            else if (collision.gameObject.GetComponent<FireState>())
            {
                if (GetItemType() == Resource_Enum.Resource.FireWood)
                {
                    FireState fireState = collision.gameObject.GetComponent<FireState>();
                    fireState.AddWood();
                    isUsed = true;
                    Destroy(this.gameObject);
                }
            }
            else if (collision.gameObject.GetComponent<PlayerScript>())
            {
                //Debug.Log("Item HitPlayer");
                collision.gameObject.GetComponent<PlayerScript>().Grab(this);
            }
            else if (collision.gameObject.GetComponent<Item>())
            {
                //do nothingg
            }
            else
            {
                //Debug.Log("ELSEEE");
                foundCounterOrFloor = true;
                poofVFX.GetComponent<ParticleSystem>().Play();
            }
        }

        //if(!collision.gameObject.GetComponent<PlayerScript>())
        Debug.Log("Hit: " + collision.gameObject + " (" + foundCounterOrFloor + ") ");

        if (foundCounterOrFloor)
        {
            Debug.Log("reset itemState!!!");
            itemState = ItemStateMachine.ItemState.None;
        }
        
    }

    public Resource_Enum.Resource GetItemType() { return itemType; }  

    public bool IsBeingDragged()
    {
        return itemState == ItemStateMachine.ItemState.IsBeingDragged;
    }
    public void SetIsBeingDragged(bool isBeingDragged)
    {
        if (isBeingDragged)
        {
            itemState = ItemStateMachine.ItemState.IsBeingDragged;
        }
        else
        {
            Debug.Log("Dragged none");
            itemState = ItemStateMachine.ItemState.None;
        }
    }
    public string GetName()
    {
        return name;
    }

    public bool IsPickedUp()
    {
        return itemState == ItemStateMachine.ItemState.IsBeingHeld;
    }

    public void SetIsPickedUp(bool isPickedUp)
    {
        if (isPickedUp)
        {
            itemState = ItemStateMachine.ItemState.IsBeingHeld;
        }
        else
        {
            Debug.Log("Held none");
            itemState = ItemStateMachine.ItemState.None;
        }
    }

    public bool IsProcessed()
    {
        return isProcessed;
    }

    public void SetIsProcessed(bool isProcessed)
    {
        this.isProcessed = isProcessed;
    }

    public void SetIsBeingTrown(bool stateIsThrown)
    {
        if (stateIsThrown)
        {
            Debug.Log("GEt Thrown item");
            itemState = ItemStateMachine.ItemState.IsBeingThrown;
        }
        else
        {
            Debug.Log("Throw none");
            itemState = ItemStateMachine.ItemState.None;
        }
    }


    private bool PlaceItselfOnCounter(CounterState currentCounter)
    {
        if (currentCounter != null)
        {
            Debug.Log("current counter isnt null");

            return currentCounter.PlaceItem(this.gameObject);
        }

        return false;
    }
}
