using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public void Start()
    {
        // Assuming full health by default
        setHealthbar(1, 1);
    }

    public void setHealthbar(float health, float maxHealth)
    {
        if (health >= maxHealth)
        {
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.enabled = true;

        float ratio = health / maxHealth;

        int spriteIndex = Mathf.FloorToInt(ratio * (sprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, sprites.Length - 1);

        spriteRenderer.sprite = sprites[sprites.Length - spriteIndex - 1];
    }
}
