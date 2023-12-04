using System.Collections;
using UnityEngine;

public class AnimationScale : MonoBehaviour
{
    public float animationDuration = 0.5f; // Duration of the animation
    public AnimationCurve scaleCurve; // Animation curve for wobbling effect

    [Header("Trigger with scale. Warning! Setting same object will also scale the trigger to 0,0,0")]
    public bool scaleInTrigger = false;
    public GameObject gameObjectToScaleInTrigger;
    private Vector3 scaleSizeWithTrigger = Vector3.one;

    private void Start()
    {
        if(scaleInTrigger && gameObjectToScaleInTrigger == null)
        {
            Debug.LogWarning("(AnimationScale)" + gameObject + ": ScaleInTrigger set to True, \n but no gameObject assigned, attempting to set children");
            gameObjectToScaleInTrigger = gameObject.GetComponentInChildren<GameObject>();
            scaleSizeWithTrigger = gameObjectToScaleInTrigger.transform.localScale;
        }
    }

    public void ScaleUp(GameObject objToScale)
    {
            StartCoroutine(ScaleUpAnimation(objToScale, Vector3.one)); // Default target scale: (1, 1, 1)
    }

    public void ScaleUp(GameObject objToScale, Vector3 targetScale)
    {
            StartCoroutine(ScaleUpAnimation(objToScale, targetScale));
    }

    public void ScaleDown(GameObject objToScale)
    {
            StartCoroutine(ScaleDownAnimation(objToScale, false));
    }
    public void ScaleDownAndDestroy(GameObject objToScale)
    {
        StartCoroutine(ScaleDownAnimation(objToScale, true));
    }

    IEnumerator ScaleUpAnimation(GameObject objToScale, Vector3 targetScale)
    {
        Vector3 initialScale = objToScale.transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(t);
            objToScale.transform.localScale = Vector3.Lerp(initialScale, targetScale, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objToScale.transform.localScale = targetScale;
    }

    IEnumerator ScaleDownAnimation(GameObject objToScale, bool destroy)
    {
        Vector3 initialScale = objToScale.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(t);
            objToScale.transform.localScale = Vector3.Lerp(initialScale, targetScale, curveValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (objToScale != null)
            objToScale.transform.localScale = targetScale;

        if (destroy && elapsedTime >= animationDuration)
        {
            Destroy(objToScale, animationDuration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(scaleInTrigger && other.CompareTag("Player"))
        {
            ScaleUp(gameObjectToScaleInTrigger, scaleSizeWithTrigger);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (scaleInTrigger && other.CompareTag("Player"))
        {
            ScaleDown(gameObjectToScaleInTrigger);
        }
    }
}
