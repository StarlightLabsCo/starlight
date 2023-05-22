using UnityEngine;

public class CopperOreSmall : MineableEntity
{
    public CopperOreSmall(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "copper_ore_small";
        Name = "Copper Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
}