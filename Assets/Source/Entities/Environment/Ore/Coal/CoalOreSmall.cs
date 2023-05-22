using UnityEngine;

public class CoalOreSmall : MineableEntity
{
    public CoalOreSmall(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "coal_ore_small";
        Name = "Coal Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
}