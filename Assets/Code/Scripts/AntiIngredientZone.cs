using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntiIngredientZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ingredient") || other.CompareTag("Bottle") || other.CompareTag("Wood"))
        {
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.GetComponent<Rigidbody>().drag= 3;
            other.gameObject.GetComponent<Rigidbody>().angularDrag = 3;
            StartCoroutine(KillIngredient(other.gameObject));
        }
    }

    IEnumerator KillIngredient(GameObject ingredient)
    {
        MeshRenderer ingredientRenderer = ingredient.GetComponentInChildren<MeshRenderer>();
        if (ingredientRenderer != null)
        {
            ingredient.layer = LayerMask.NameToLayer("Ignore Raycast");

            Collider[] colliders = ingredient.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            if(ingredient.GetComponent<Item>().lastHeldPlayer != null)
            {
                if(ingredient.GetComponent<Item>().lastHeldPlayer.TryGetComponent<PlayerScript>(out var player))
                {
                    if(player.GetObjectInHands() != null)
                    {
                        if (player.GetObjectInHands().GetInstanceID() == ingredient.GetInstanceID())
                        {
                            player.SetPlayerState(PlayerStateMashineHandle.HoldingState.HoldingNothing);
                            player.Drop(false);
                        }
                    }

                }

            }

            Material originalMaterial = ingredientRenderer.material;
            Color startColor = originalMaterial.color;
            Color targetColor = Color.black;

            float duration = 1.5f;
            float timer = 0f;

            AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

            Vector3 originalScale = ingredient.transform.localScale;

            GameObject desteroyIngridenseEffekt = ingredient.transform.Find("DestoryItem").gameObject;
            if(desteroyIngridenseEffekt != null)
            {
                desteroyIngridenseEffekt.SetActive(true);
            }

            while (timer < duration)
            {
                float t = timer / duration;
                float scaleValue = scaleCurve.Evaluate(t);

                // Color interpolation
                Color lerpedColor = Color.Lerp(startColor, targetColor, t);
                ingredientRenderer.material.color = lerpedColor;

                // Scale down the ingredient
                ingredient.transform.localScale = originalScale * scaleValue;

                timer += Time.deltaTime;
                yield return null;
            }
        }
        Destroy(ingredient);
    }
}
