using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTypewriter : MonoBehaviour
{
    public float letterAppearDelay = 0.05f; // Time delay between each letter
    public float nextCycleDelay = 3.5f; // Time delay between each letter
    public TMP_Text textComponent; // Reference to the TextMeshPro text component
    public List<TMP_Text> displayText = new List<TMP_Text>(); // List of TMP_Text components

    private int currentTextIndex = 0; // Index for the current text
    private Coroutine typingCoroutine; // Coroutine reference for the typewriter effect

    private void Start()
    {
        StartTypewriter();
    }

    private void StartTypewriter()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        int textsCount = displayText.Count;

        // If there's only one text, play the typewriter animation only once
        if (textsCount == 1)
        {
            TMP_Text currentText = displayText[0];
            textComponent.text = currentText.text; // Set text to display

            int totalVisibleCharacters = textComponent.text.Length;
            int counter = 0;

            textComponent.maxVisibleCharacters = 0;

            while (counter < totalVisibleCharacters)
            {
                textComponent.maxVisibleCharacters = counter + 1;
                yield return new WaitForSeconds(letterAppearDelay);
                counter++;
            }
        } else // If there are multiple texts, cycle through them
        {
            while (true)
            {
                TMP_Text currentText = displayText[currentTextIndex];

                textComponent.text = currentText.text; // Set text to display

                int totalVisibleCharacters = textComponent.text.Length;
                int counter = 0;

                textComponent.maxVisibleCharacters = 0;

                while (counter < totalVisibleCharacters)
                {
                    textComponent.maxVisibleCharacters = counter + 1;
                    yield return new WaitForSeconds(letterAppearDelay);
                    counter++;
                }

                yield return new WaitForSeconds(nextCycleDelay);

                textComponent.maxVisibleCharacters = 0;
                // Increment the index or cycle back to 0 when reaching the end
                currentTextIndex = (currentTextIndex + 1) % textsCount;
            }
        }
    }

    public void SetNewText(List<TMP_Text> newTexts)
    {
        displayText = newTexts;
        currentTextIndex = 0;
        StartTypewriter();
    }
}
