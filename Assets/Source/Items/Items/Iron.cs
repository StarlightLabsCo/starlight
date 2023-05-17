using System.Linq;
using UnityEngine;

public class Iron : Item
{
    public Iron()
    {
        Id = "iron";
        Name = "Iron";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_iron");
    }
}