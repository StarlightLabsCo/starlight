using UnityEngine;

public class Sword : ActionItem
{
    public Sword()
    {
        Id = "sword";
        Name = "Sword";
        sprite = Resources.Load<Sprite>("Sunnyside/UI/sword") as Sprite;

        Debug.Log("Loaded sword sprite: " + sprite);

        action = new SwingSword();
    }
}