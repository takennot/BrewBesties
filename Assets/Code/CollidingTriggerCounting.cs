using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingTriggerCounting : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjectsCollidingWith = new List<GameObject>();

    private void OnTriggerEnter(Collider otherCollider)
    {
        Debug.Log("Collision!!!: " + otherCollider.gameObject);

        if (otherCollider.gameObject != this.gameObject && !gameObjectsCollidingWith.Contains(otherCollider.gameObject))
        {
            gameObjectsCollidingWith.Add(otherCollider.gameObject);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        Debug.Log("CollisionExit!!!: " + otherCollider.gameObject);

        if (gameObjectsCollidingWith.Contains(otherCollider.gameObject))
        {
            gameObjectsCollidingWith.Remove(otherCollider.gameObject);
        }
    }

    public List<GameObject> GetGameobjectsCollidingWith()
    {
        return gameObjectsCollidingWith;
    }
}
