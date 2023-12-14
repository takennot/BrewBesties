using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider))] // Ensures the object has a collider component
public class ColliderVisualizer : MonoBehaviour
{

    [SerializeField] private Color solidColor = new(0, 0, 1, 0.05f); // Blue with 5% opacity
    [SerializeField] private Color wireframeColor = new(1, 1, 1, 0.5f); // White with 50% opacity



    private void OnDrawGizmos()
    {
        DrawAllColliderGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        DrawAllColliderGizmos();
    }

    private void DrawAllColliderGizmos()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                Gizmos.color = wireframeColor;

                if (collider is BoxCollider)
                {
                    DrawBoxColliderGizmo((BoxCollider)collider);
                } else if (collider is SphereCollider)
                {
                    DrawSphereColliderGizmo((SphereCollider)collider);
                }
                  // Add other collider types as needed
                  else
                {
                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                }
            }
        }
    }

    private void DrawBoxColliderGizmo(BoxCollider boxCollider)
    {
        Gizmos.matrix = Matrix4x4.TRS(
            boxCollider.transform.position,
            boxCollider.transform.rotation,
            boxCollider.transform.lossyScale
        );

        Gizmos.color = solidColor;
        Gizmos.DrawCube(boxCollider.center, boxCollider.size);

        Gizmos.color = wireframeColor;
        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
    }

    private void DrawSphereColliderGizmo(SphereCollider sphereCollider)
    {
        Gizmos.matrix = Matrix4x4.TRS(
            sphereCollider.transform.position,
            sphereCollider.transform.rotation,
            sphereCollider.transform.lossyScale
        );
        Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
    }
}