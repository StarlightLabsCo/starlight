using UnityEngine;

public class StoneOreMedium : MineableEntity
{
    public StoneOreMedium(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "stone_ore_medium_" + System.Guid.NewGuid().ToString();
        Name = "Stone Ore Deposit (Medium)";
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("StoneOreSmall") as GameObject; set => base.entityOnDeath = value; }
}