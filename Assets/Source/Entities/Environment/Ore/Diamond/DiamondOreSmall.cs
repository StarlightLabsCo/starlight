using UnityEngine;

public class DiamondOreSmall : MineableEntity
{
    public DiamondOreSmall(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "diamond_ore_small_" + System.Guid.NewGuid().ToString();
        Name = "Diamond Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
}