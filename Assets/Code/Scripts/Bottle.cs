using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject[] slotsBackground;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Header("Other")]
    //[SerializeField] private Resource_Enum.Ingredient[] ingredients;

    [SerializeField] private SpriteManager spriteManager;
    [SerializeField] private MeshRenderer inside;
    [SerializeField] private Potion potion;

    private bool isEmpty = true;

    private string baseColorString = "_BaseColor";
    private string rippelColorString = "_RippelColor";
    private string rimColorString = "_RimColor";

    [SerializeField] GameObject donePotion;
    [SerializeField] GameObject undonePotoin;

    ColorHandeler ch;

    private void Awake()
    {
        spriteManager = FindObjectOfType<GameManagerScript>().GetComponent<SpriteManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isEmpty = true;
        //SetTransparanteMaterila();
        ch = FindObjectOfType<ColorHandeler>();
        inside.gameObject.SetActive(false);
        donePotion.SetActive(false);
        undonePotoin.SetActive(false);
    }
    public void Fill()
    {
        if(potion.ingredient3 != null)
        {
            if(potion.ingredient3.GetIsMagic())
            {
                ChangeFlaskColor(ch.GetColor("yellow")[0], ch.GetColor("yellow")[1]);
            }
            else
            {
                switch (potion.ingredient3.GetIngredientType())
                {
                    case Resource_Enum.Ingredient.Mushroom:
                        ChangeFlaskColor(ch.GetColor("red")[0], ch.GetColor("red")[1]);

                        break;
                    case Resource_Enum.Ingredient.MonsterEye:
                        ChangeFlaskColor(ch.GetColor("gray")[0], ch.GetColor("gray")[1]);

                        break;
                    case Resource_Enum.Ingredient.Water:
                        ChangeFlaskColor(ch.GetColor("blue")[0], ch.GetColor("blue")[1]);

                        break;
                    default:
                        ChangeFlaskColor(ch.GetColor("blue")[0], ch.GetColor("blue")[1]);
                        break;
                }
            }
        }
        else
        {
            ChangeFlaskColor(ch.GetColor("blue")[0], ch.GetColor("blue")[1]);
        }

        isEmpty = false;
        inside.gameObject.SetActive(true);
    }

    public void ChangeFlaskColor(UnityEngine.Color baseColor, UnityEngine.Color rippleColor)
    {
        inside.material.SetColor(baseColorString, baseColor);
        inside.material.SetColor(rippelColorString, rippleColor);
        inside.material.SetColor(rimColorString, rippleColor);
    }

    void UpdateUI()
    {
        if (potion != null)
        {
            //Debug.Log("Potion != null");
            if (potion.ingredient1 != null)
            {
                //Debug.Log("potion.ingredient1 != null");
                slotsBackground[0].SetActive(true);
                spriteRenderers[0].sprite = potion.ingredient1.GetImage();
            }

            if (potion.ingredient2 != null)
            {
                //Debug.Log("potion.ingredient2 != null");
                slotsBackground[1].SetActive(true);
                spriteRenderers[1].sprite = potion.ingredient2.GetImage();
            }

            if (potion.ingredient3 != null)
            {
                //Debug.Log("potion.ingredient3 != null");
                slotsBackground[2].SetActive(true);
                spriteRenderers[2].sprite = potion.ingredient3.GetImage();
            }
        }
    }

    public void SetPotion(Potion newPotion)
    {
        if (isEmpty)
        {
            potion = newPotion;
            if(potion.ingredient1 != null)
                potion.ingredient1.SetImages(spriteManager);
            if(potion.ingredient2 != null)
                potion.ingredient2.SetImages(spriteManager);
            if (potion.ingredient3 != null)
            potion.ingredient3.SetImages(spriteManager);

            Fill();
            UpdateUI();


            if (potion.IsDone())
            {
                Debug.Log("Potion är done");
                if (donePotion.activeSelf == false)
                {
                    donePotion.SetActive(true);
                    undonePotoin.SetActive(false);
                }

            }
            else
            {
                if (undonePotoin.activeSelf == false)
                {
                    donePotion.SetActive(false);
                    undonePotoin.SetActive(true);
                }
            }
        }
    }

    public Potion GetPotion()
    {
        return potion;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }
}
