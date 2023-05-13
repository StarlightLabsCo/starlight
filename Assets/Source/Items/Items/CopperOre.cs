using UnityEngine;

public class CopperOre : Item
{
    public CopperOre()
    {
        Id = "copper_ore";
        Name = "Copper Ore";

        Object[] sprites = Resources.LoadAll("Sunnyside/Tileset/spr_tileset_sunnysideworld_16px");

        sprite = sprites[1199] as Sprite;
    }
}