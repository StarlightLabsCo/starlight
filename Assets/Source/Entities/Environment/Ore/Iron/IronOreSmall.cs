using UnityEngine;

public class IronOreSmall : MineableEntity
{
    public IronOreSmall(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "iron_ore_small_" + System.Guid.NewGuid().ToString();
        Name = "Iron Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
}