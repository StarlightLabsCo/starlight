using System.Linq;
using UnityEngine;

public class Coal : Item
{
    public Coal()
    {
        Id = "coal";
        Name = "Coal";

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_coal");
    }
}