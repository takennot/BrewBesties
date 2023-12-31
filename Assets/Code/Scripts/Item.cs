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

    [SerializeField] private GameObject[] gameObjectsToRenderOnPortrait; //For playerPortraits
    Dictionary<PlayerScript.PlayerType, string> playerLayerMap;

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
   
        if(gameObjectsToRenderOnPortrait.Length == 0) { Debug.LogWarning("No gameObjects set as gameObjectsToRenderOnPortrait in " + gameObject.name); }

        // Create a dictionary to map player types to layer names
        playerLayerMap = new Dictionary<PlayerScript.PlayerType, string>()
        {
            {PlayerScript.PlayerType.PlayerOne, "Player1Render"},
            {PlayerScript.PlayerType.PlayerTwo, "Player2Render"},
            {PlayerScript.PlayerType.PlayerThree, "Player3Render"},
            {PlayerScript.PlayerType.PlayerFour, "Player4Render"},
        };
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

            if (playerLayerMap.ContainsKey(lastHeldPlayer.playerType))
            {
                foreach (GameObject obj in gameObjectsToRenderOnPortrait) { 
                    obj.layer = LayerMask.NameToLayer(playerLayerMap[lastHeldPlayer.playerType]);
                }
            }

        }
        else if(itemState == ItemStateMachine.ItemState.IsBeingDragged || itemState == ItemStateMachine.ItemState.IsBeingThrown)
        {
            gameObject.layer = LayerMask.NameToLayer("IngredientThrown");
            foreach (GameObject obj in gameObjectsToRenderOnPortrait)
            {
                obj.layer = 0;
            }
        }
        else
        {
            gameObject.layer = 6;
            foreach (GameObject obj in gameObjectsToRenderOnPortrait)
            {
                obj.layer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemState == ItemStateMachine.ItemState.IsBeingThrown)
        {
            Debug.Log("slay: " + other.gameObject);

            //check if player can be found in the collider hit or in its parent

            PlayerScript foundPlayer = other.GetComponentInParent<PlayerScript>();
            if(!foundPlayer)
                foundPlayer = other.gameObject.GetComponent<PlayerScript>();

            if (foundPlayer)
            {
                Debug.Log("slay found player");
                if(foundPlayer != lastHeldPlayer)
                {
                    Debug.Log("Item HitPlayer");
                    foundPlayer.Grab(this);
                    lastHeldPlayer = foundPlayer;
                }
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
                Debug.Log("Item HitPlayer");
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
            Debug.Log("HELLO HELLO HELLO HELLO HELLO");

            itemState = ItemStateMachine.ItemState.None;

            return currentCounter.PlaceItem(this.gameObject);
        }

        return false;
    }

    public void SetItemState(ItemStateMachine.ItemState newItemState)
    {
        itemState = newItemState;
    }
}
