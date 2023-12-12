using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WS_MagicField : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isActive = true;
    private bool originalActive;
    [SerializeField] private bool mustBeThrown = true;

    [Header("Boost items passing through?")]
    [SerializeField] private bool givesBoost = false;
    [SerializeField] private float forceMagnitude = 1f;

    [SerializeField] private GameObject field;

    [Header("VFX magic ingredient")]
    private Ingredient ingredientObject;

    [SerializeField] private Material magiMaterialMushroom;
    [SerializeField] private Material magiMaterialEye;

    [SerializeField] GameObject MagicObejctEffekt;
    [SerializeField] float timeBeforeDestory = 2f;

    [Header("Other")]
    
    [SerializeField] GameObject startPortal;

    // Start is called before the first frame update
    void Start()
    {
        field = this.gameObject;
        field.SetActive(isActive);
        originalActive = isActive;
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
        ingredientObject = other.GetComponent<Ingredient>();
        StartCoroutine(AddMagic(timeBeforeDestory));
    }

    public void SetIsActive(bool state)
    {
        field.SetActive(state);
    }

    public void ReverseState(bool returnToOriginal)
    {
        if(returnToOriginal)
        {
            isActive = originalActive;
        } else
        {
            isActive = !originalActive;
        }

        field.SetActive(isActive);
    }


    private IEnumerator AddMagic(float time)
    {
        if (ingredientObject)
        {
            ingredientObject.Magicify();

            ingredientObject.GetMagicController().MagicOnIngredient();
            ingredientObject.GetMagicController().CreateParticle(1f);
            ingredientObject.GetMagicController().onlyOnePartical = false;
            ingredientObject.GetMagicController().createOnce = false;

            yield return new WaitForSeconds(time);

            ingredientObject.GetMagicController().onlyOnePartical = true;
            ingredientObject.GetMagicController().createOnce = true;
            ingredientObject.GetMagicController().DestoryParticle();

        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        


        // //material
        // Material[] material1 = ingredientObject.GetComponentInChildren<MeshRenderer>().materials;
        // Material[] newArry1 = new Material[ingredientObject.GetComponentInChildren<MeshRenderer>().materials.Length + 1];

        // Debug.Log("material is " + material1.Length + " new är " + newArry1.Length);
        // int index = 0;
        // foreach (Material m in material1)
        // {
        //     newArry1[index] = material1[index];
        //     index++;

        // }
        // newArry1[index] = magiMaterial;
        // ingredientObject.GetComponentInChildren<MeshRenderer>().materials = newArry1;

        // GameObject effekt = Instantiate(MagicObejctEffekt, ingredientObject.transform);
        // // time
        // yield return new WaitForSeconds(time);

        // Destroy(effekt);

        // Material[] material2 = ingredientObject.GetComponentInChildren<MeshRenderer>().materials;
        // Material[] newArry2 = new Material[ingredientObject.GetComponentInChildren<MeshRenderer>().materials.Length - 1];

        // for (int i = 0; i < newArry2.Length; i++)
        // {
        //     newArry2[i] = material2[i];

        // }

        //ingredientObject.GetComponentInChildren<MeshRenderer>().materials = newArry2;
    }

    void startPortalMetod()
    {
        Transform postion = this.transform.Find("PortalMagic");
        GameObject oldPortal = postion.gameObject;
        Destroy(oldPortal);
        Instantiate(startPortal, postion);

    }

}
