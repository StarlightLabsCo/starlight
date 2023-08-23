using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    public string Id;
    public Item item;
    private float minShadowScale = 0.3f;
    private float maxShadowScale = 0.6f;
    public SpriteRenderer itemRenderer;
    public SpriteRenderer shadowRenderer;

    private Dictionary<string, float> immuneCharacters = new Dictionary<string, float>();
    private const float immunityDuration = 3f;

    void Start()
    {
        Id = System.Guid.NewGuid().ToString();

        itemRenderer.sprite = item.sprite;

        Sprite shadowSprite = CreateShadowSprite(32, 16);
        shadowRenderer.sprite = shadowSprite;
        shadowRenderer.color = new Color(0, 0, 0, 0.5f);
        shadowRenderer.transform.localPosition = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        float time = Time.time * 2;
        float newY = Mathf.Sin(time) * 0.1f;
        itemRenderer.transform.localPosition = new Vector3(0, newY, 0);

        float shadowScale = Mathf.Lerp(minShadowScale, maxShadowScale, (newY + 0.1f) / 0.2f);
        shadowRenderer.transform.localScale = new Vector3(shadowScale, shadowScale, 1);

        shadowRenderer.color = new Color(0, 0, 0, Mathf.Lerp(0.25f, 0.35f, (newY + 0.1f) / 0.2f));
    }


    Texture2D GenerateSolidEllipse(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width * 2, height * 2);
        Color[] pixels = new Color[width * 2 * height * 2];
        for (int x = 0; x < width * 2; x++)
        {
            for (int y = 0; y < height * 2; y++)
            {
                float distanceFromCenter = Mathf.Pow(x - width, 2) / Mathf.Pow(width, 2) + Mathf.Pow(y - height, 2) / Mathf.Pow(height, 2);
                pixels[y * width * 2 + x] = (distanceFromCenter <= 1) ? color : Color.clear;
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    Sprite CreateShadowSprite(int width, int height)
    {
        Texture2D texture = GenerateSolidEllipse(width, height, Color.black);
        return Sprite.Create(texture, new Rect(0, 0, width * 2, height * 2), new Vector2(0.5f, 0.5f));
    }

    public void AddImmunity(string characterId)
    {
        immuneCharacters[characterId] = Time.time + immunityDuration;
    }

    public bool IsImmune(string characterId)
    {
        if (immuneCharacters.TryGetValue(characterId, out float immunityEndTime))
        {
            if (Time.time < immunityEndTime)
                return true;

            immuneCharacters.Remove(characterId);
        }

        return false;
    }
}
