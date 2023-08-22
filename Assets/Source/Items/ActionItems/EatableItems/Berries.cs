using System.Linq;
using UnityEngine;

public class Berries : EatableItem
{
    public Berries()
    {
        Id = "berries";
        Name = "Berries";

        action = new Eat(this);
        satiety = 10;

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_berries");
    }
}
