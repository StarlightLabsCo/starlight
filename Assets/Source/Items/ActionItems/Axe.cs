using System.Linq;
using UnityEngine;

public class Axe : ActionItem
{
    public Axe()
    {
        Id = "axe";
        Name = "Axe";

        action = new SwingAxe();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites.Single(s => s.name == "spr_axe");
    }
}