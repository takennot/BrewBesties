using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WS_MagicField : MonoBehaviour
{

    [SerializeField] private bool isActive = true;
    [SerializeField] private bool mustBeThrown = true;
    private GameObject field;

    [Header("Boost items passing through?")]
    [SerializeField] private bool givesBoost = false;
    [SerializeField] private float forceMagnitude = 1f;

    // Start is called before the first frame update
    void Start()
    {
        field = this.gameObject;
        field.SetActive(isActive);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ingredient"))
            return;

        if(mustBeThrown)
        {
            if (other.GetComponent<Item>().IsPickedUp())
            {
                return;
            }
        }
        Debug.Log(other.name + " passed through the field");

        // Apply force in the direction the object is already moving
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && givesBoost)
        {
            Vector3 forceDirection = other.GetComponent<Rigidbody>().velocity.normalized;  
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }

        other.GetComponent<Ingredient>().Magicify();
        other.GetComponent<Item>().SetIsProcessed(true);
        
    }

    public void SetIsActive(bool state)
    {
        field.SetActive(state);

    }

}
