using UnityEngine;

public class IronOreSmall : MineableEntity
{
    public IronOreSmall(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "iron_ore_small";
        Name = "Iron Ore Deposit (Small)";
    }


    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
}