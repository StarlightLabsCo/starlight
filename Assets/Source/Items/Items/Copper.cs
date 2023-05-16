using System.Linq;
using UnityEngine;

public class Copper : Item
{
    public Copper()
    {
        Id = "copper";
        Name = "Copper";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_copper");
    }
}