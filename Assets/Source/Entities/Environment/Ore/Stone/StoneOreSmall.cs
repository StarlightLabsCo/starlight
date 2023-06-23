using UnityEngine;

public class StoneOreSmall : MineableEntity
{
    public StoneOreSmall(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "stone_ore_small_" + System.Guid.NewGuid().ToString();
        Name = "Stone Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
}