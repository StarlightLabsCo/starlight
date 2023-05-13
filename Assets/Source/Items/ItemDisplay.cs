using UnityEngine;

public class ItemDisplay : MonoBehaviour
{


    public Item item;

    [SerializeField]
    public ItemInstanceFactory.Items Item;

    Sprite sprite;
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        item = ItemInstanceFactory.Create(Item);

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