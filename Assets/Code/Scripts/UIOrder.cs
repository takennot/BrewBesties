using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("Audio")]
    [SerializeField] private AudioClip swooshClip;
    [SerializeField] private AudioClip swooshAwayClip;
    [SerializeField] private AudioSource audioSource;

    [Header("Customer")]
    [SerializeField] private CustomerManager customer;
    private bool updateCustomer = false;

    [Header("Referenses to UI")]
    [SerializeField] private bool showPaper = false;
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderFill;

    public UnityEngine.UI.Image ingredientSlot1;
    public UnityEngine.UI.Image ingredientSlot2;
    public UnityEngine.UI.Image ingredientSlot3;

    Gradient gradient;

    private void Start()
    {
        SetGradient();
    }

    private void SetGradient()
    {
        gradient = new Gradient();

        var colors = new GradientColorKey[3];

        colors[0] = new GradientColorKey(Color.red, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 0.5f);
        colors[2] = new GradientColorKey(Color.green, 1.0f);

        //Debug.Log("0.0");
        //Debug.Log("" + slider.maxValue / 2);
        //Debug.Log("" + slider.maxValue);

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);
    }

    // Update is called once per frame
    void Update()
    {
        if (customer)
        {
            if (updateCustomer)
            {
                UpdateCustomer();
            }

            //set slider
            slider.value = slider.maxValue - customer.GetPatienceTimer();
        }
        else
        {
            showPaper = false;
            slider.value = 0;
            //empty images?
        }

        //old color change
        //sliderFill.color = Color.Lerp(Color.red, Color.green, slider.value / slider.maxValue);

        // calculate color at the given time
        Color newColor = gradient.Evaluate(slider.value / slider.maxValue);
        sliderFill.color = newColor;
        if(customer)
            customer.SetGhostColor(newColor);

        if(animator.GetBool("Show") != showPaper)
        {
            if (showPaper)
                audioSource.pitch = 1;
            else
                audioSource.pitch = 0.8f;

            audioSource.PlayOneShot(swooshClip);
        }

        animator.SetBool("Show", showPaper);
    }

    private void UpdateCustomer()
    {
        SetImages();
    }

    public void SetPaperVisibility(bool newState)
    {
        showPaper = newState;
    }

    private void SetImages()
    {
        ingredientSlot1.sprite = customer.GetOrder().GetIngredients()[0].GetImage();
        ingredientSlot2.sprite = customer.GetOrder().GetIngredients()[1].GetImage();
        ingredientSlot3.sprite = customer.GetOrder().GetIngredients()[2].GetImage();
    }

    public void SetCustomer(CustomerManager newCustomer, float maxAngryUntilLeave, bool newCust)
    {
        customer = newCustomer;
        updateCustomer = true;
        slider.maxValue = maxAngryUntilLeave;
        if(newCust)
            slider.value = slider.maxValue;

        SetGradient();
    }
}
