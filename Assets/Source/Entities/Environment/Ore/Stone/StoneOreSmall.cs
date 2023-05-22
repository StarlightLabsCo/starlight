using UnityEngine;

public class StoneOreSmall : MineableEntity
{
    public StoneOreSmall(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "stone_ore_small";
        Name = "Stone Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
}