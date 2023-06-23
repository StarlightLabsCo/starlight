using UnityEngine;

public class CoalOreSmall : MineableEntity
{
    public CoalOreSmall(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "coal_ore_small_" + System.Guid.NewGuid().ToString();
        Name = "Coal Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
}