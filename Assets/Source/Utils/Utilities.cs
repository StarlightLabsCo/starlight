using System.Linq;
using UnityEngine;

public static class Utilities
{
    // Action collision detection and execution
    public static Collider2D[] DetectCollisions(Character character, Vector2 offset, Vector2 size, LayerMask m_LayerMask)
    {
        Vector3 offsetVector3 = new Vector3(offset.x, offset.y, 0);

        Vector3 overlapBoxPosition = character.transform.position + offsetVector3;

        // Use the OverlapBox to detect colliders within this box area
        // Use the calculated position, the specified size, and the character's rotation
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(overlapBoxPosition, size, 0, m_LayerMask);

        // Remove self from the list of colliders
        hitColliders = hitColliders.Where(collider => collider.gameObject != character.gameObject).ToArray();

        // Begin Debug drawing
        Vector3 boxHalfExtents = new Vector3(size.x, size.y, 0) / 2;

        Vector3 topLeft = overlapBoxPosition + new Vector3(-boxHalfExtents.x, boxHalfExtents.y, 0);
        Vector3 topRight = overlapBoxPosition + new Vector3(boxHalfExtents.x, boxHalfExtents.y, 0);
        Vector3 bottomLeft = overlapBoxPosition + new Vector3(-boxHalfExtents.x, -boxHalfExtents.y, 0);
        Vector3 bottomRight = overlapBoxPosition + new Vector3(boxHalfExtents.x, -boxHalfExtents.y, 0);

        // Draw the four sides of the box
        Debug.DrawLine(topLeft, topRight, Color.red, 2f);
        Debug.DrawLine(topRight, bottomRight, Color.red, 2f);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red, 2f);
        Debug.DrawLine(bottomLeft, topLeft, Color.red, 2f);
        // End Debug drawing

        return hitColliders;
    }

    public static Item idToItem(string itemId)
    {
        switch (itemId)
        {
            case "wood":
                return new Wood();
            case "stone":
                return new Stone();
            case "iron":
                return new Iron();
            case "diamond":
                return new Diamond();
            case "copper":
                return new Copper();
            case "coal":
                return new Coal();
            case "axe":
                return new Axe();
            case "pickaxe":
                return new Pickaxe();
            case "sword":
                return new Sword();
            default:
                return null;
        }
    }
}