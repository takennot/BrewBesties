using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGrabHandler : MonoBehaviour
{
    //[SerializeField] private List<Collision> collisions = new List<Collision>();
    [SerializeField] private List<GameObject> gameObjectsCollidingWith = new List<GameObject>();

    private void OnTriggerEnter(Collider otherCollider)
    {
        //Debug.Log("Collision!!!: " + otherCollider.gameObject);

        if (!gameObjectsCollidingWith.Contains(otherCollider.gameObject))
        {
            //collisions.Add(collision);
            gameObjectsCollidingWith.Add(otherCollider.gameObject);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (gameObjectsCollidingWith.Contains(otherCollider.gameObject))
        {
            //collisions.Remove(collision);
            gameObjectsCollidingWith.Remove(otherCollider.gameObject);
        }
    }

    public List<GameObject> GetGameobjectsCollidingWith()
    {
        return gameObjectsCollidingWith;
    }

}
