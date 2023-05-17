using System.Linq;
using UnityEngine;

public class Stone : Item
{
    public Stone()
    {
        Id = "stone";
        Name = "Stone";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_stone");
    }
}