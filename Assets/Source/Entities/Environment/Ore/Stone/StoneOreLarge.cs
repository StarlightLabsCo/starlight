using UnityEngine;

public class StoneOreLarge : MineableEntity
{
    public StoneOreLarge(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "stone_ore_large_" + System.Guid.NewGuid().ToString();
        Name = "Stone Ore Deposit (Large)";
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("StoneOreMedium") as GameObject; set => base.entityOnDeath = value; }
}