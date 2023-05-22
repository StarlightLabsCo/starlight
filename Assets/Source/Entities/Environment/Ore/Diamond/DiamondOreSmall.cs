using UnityEngine;

public class DiamondOreSmall : MineableEntity
{
    public DiamondOreSmall(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "diamond_ore_small";
        Name = "Diamond Ore Deposit (Small)";
    }


    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
}