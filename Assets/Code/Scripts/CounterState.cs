using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CounterState : MonoBehaviour {
    public GameObject storedItem = null;

    [Tooltip("Position item will be stored on counter. 0f = center")]
    public float yItemOffset = 1f;
    [SerializeField] private Outline counterOutline;

    private void Start()
    {

    }

    private void Update()
    {
        if (!counterOutline)
            return;

        if (storedItem == null)
        {
            //counterOutline.enabled = true;
        }
        else
        {
            //counterOutline.enabled = false;
        }
    }

    public bool PlaceItem(GameObject item) 
    {
        if (storedItem == null && !item.gameObject.GetComponentInParent<CounterState>()) 
        {
            Debug.Log("Item placed on counter");
            // Set the item as a child of the counter and update its position
            storedItem = item;
            item.transform.parent = transform;
            item.transform.localPosition = new Vector3(0, yItemOffset, 0); 
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.transform.rotation = item.GetComponent<Item>().originalRotation;
            //counterOutline.enabled = false;
            return true;
        } 
        else 
        {
            //counterOutline.enabled = true;
            Debug.Log("Counter is occupied!");
            return false;
        }
    }

    public GameObject PickUpItem() {
        if (storedItem != null) {
            GameObject itemToPickUp = storedItem;

            ReleaseItem(itemToPickUp);

            return itemToPickUp;
        } else {
            //counterOutline.enabled = false;
            Debug.Log("Counter is empty!");
            return null;
        }
    }

    public void ReleaseItem(GameObject itemToPickUp)
    {
        storedItem = null;
        itemToPickUp.transform.parent = null;
        itemToPickUp.GetComponent<Rigidbody>().isKinematic = false;
        //counterOutline.enabled = true;
    }

    
}

