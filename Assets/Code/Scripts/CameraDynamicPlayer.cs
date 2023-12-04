using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDynamicPlayer : MonoBehaviour
{
    [SerializeField] private float originalWeight = 0.8f; // Weight for original camera position
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    private List<GameObject> players; // Array of player transforms

    private Vector3 originalPos; // Original camera position

    [SerializeField] private GameManagerScript gameManager;

    private void Start()
    {
        originalPos = transform.position;
        gameManager = FindObjectOfType<GameManagerScript>();
        foreach (PlayerScript player in FindObjectsOfType<PlayerScript>())
        {
            players.Add(player.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (players == null || players.Count == 0)
            return;

        Vector3 averagePos = CalculateAveragePlayerPosition();
        Vector3 desiredPos = Vector3.Lerp(originalPos, averagePos + offset, originalWeight);

        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }

    private Vector3 CalculateAveragePlayerPosition()
    {
        Vector3 avgPos = Vector3.zero;

        foreach (var player in players)
        {
            if (player != null)
                avgPos += player.transform.position;
        }

        avgPos /= players.Count;
        return avgPos;
    }
}
