using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

    // TODO: add handling item displays as well, maybe that should be an entity
    // TODO: move this into the entity class, make it so all entities can see other entities? would be very systemic
    public static HashSet<Entity> UpdateObservedEntities(Character character, HashSet<Entity> observedEntities, Transform transform, float radius)
    {
        // First filter out any null entities in observedEntities
        observedEntities = new HashSet<Entity>(observedEntities.Where(entity => entity != null));

        // Get all GameObjects within the radius
        Vector2 detectionCenter = transform.position;

        int layerMask = ~0; // This selects all layers. Adjust as needed to exclude specific layers.
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(detectionCenter, radius, layerMask);

        HashSet<Entity> newEntitiesInRange = new HashSet<Entity>(collidersInRange.Select(gameObject => gameObject.GetComponent<Entity>()).Where(entity => entity != null && entity != character));

        HashSet<Entity> addedEntities = new HashSet<Entity>(newEntitiesInRange.Except(observedEntities));
        HashSet<Entity> removedEntities = new HashSet<Entity>(observedEntities.Except(newEntitiesInRange));

        foreach (Entity entity in addedEntities)
        {
            Debug.Log(character.name + " sees " + entity.name + " at X: " + entity.transform.position.x + ", Y: " + entity.transform.position.y);

            string json = JsonConvert.SerializeObject(new
            {
                type = "Observation",
                data = new
                {
                    observerId = character.Id.ToString(),
                    observation = character.name + " sees " + entity.name + " at X: " + entity.transform.position.x + ", Y: " + entity.transform.position.y
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        foreach (Entity entity in removedEntities)
        {
            Debug.Log(character.name + " no longer sees " + entity.name);
            string json = JsonConvert.SerializeObject(new
            {
                type = "Observation",
                data = new
                {
                    observerId = character.Id.ToString(),
                    observation = character.name + " no longer sees " + entity.name + " at X: " + entity.transform.position.x + ", Y: " + entity.transform.position.y
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        return newEntitiesInRange;
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