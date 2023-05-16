using System.Linq;
using UnityEngine;

public class Wood : Item
{
    public Wood()
    {
        Id = "wood";
        Name = "Wood";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_wood");
    }
}