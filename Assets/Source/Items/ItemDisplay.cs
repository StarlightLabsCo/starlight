using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    public string Id;
    public Item item;

    Sprite sprite;
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        Id = System.Guid.NewGuid().ToString();

        sprite = item.sprite;
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
    }

    public void FixedUpdate()
    {
        // Bob up and down
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * 2) * 0.005f, transform.position.z);
    }
}