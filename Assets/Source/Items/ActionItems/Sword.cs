using System.Linq;
using UnityEngine;

public class Sword : ActionItem
{
    public Sword()
    {
        Id = "sword";
        Name = "Sword";

        action = new SwingSword();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_sword");
    }
}