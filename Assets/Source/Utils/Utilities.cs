
using System.Linq;
using UnityEngine;

public static class Utilities
{
    // Action collision detection and execution
    public static Collider2D[] DetectCollisions(GameObject self, Transform transform, Vector2 offset, Vector2 size, LayerMask m_LayerMask)
    {
        // Calculate the position of the overlap box in front of the character
        Vector3 overlapBoxPosition = transform.position + transform.right * offset.x + transform.up * offset.y;

        // Use the OverlapBox to detect colliders within this box area
        // Use the calculated position, the specified size, and the character's rotation
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(overlapBoxPosition, size, 0, m_LayerMask);

        // Remove self from the list of colliders
        hitColliders = hitColliders.Where(collider => collider.gameObject != self).ToArray();

        return hitColliders;
    }
}

