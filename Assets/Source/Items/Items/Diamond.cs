using System.Linq;
using UnityEngine;

public class Diamond : Item
{
    public Diamond()
    {
        Id = "diamond";
        Name = "Diamond";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_diamond");
    }
}