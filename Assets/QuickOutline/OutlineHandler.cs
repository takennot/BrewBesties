using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    public Outline outline;

    [SerializeField] private float timeOut = 1f;

    private void Update()
    {
        if(timeOut >= 1f)
        {
            // Debug.Log("Disable outline");
            outline.HideOutline();

            outline.enabled = false;
        }
        else
        {
            timeOut += Time.deltaTime;
        }
    }

    public void ShowOutline(Color playerColor, bool thick)
    {
        //Debug.Log("Enable outline");
        outline.enabled = true;
        timeOut = 0f;

        outline.ShowOutline(playerColor, thick);
    }
}
