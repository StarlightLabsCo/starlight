using System.Linq;
using UnityEngine;

public class Pickaxe : ActionItem
{
    public Pickaxe()
    {
        Id = "pickaxe";
        Name = "Pickaxe";

        action = new SwingPickaxe();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_pickaxe");
    }
}