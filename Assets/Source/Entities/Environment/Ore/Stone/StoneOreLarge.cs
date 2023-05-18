using UnityEngine;

public class StoneOreLarge : MineableEntity
{
    public StoneOreLarge(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("StoneOreMedium") as GameObject; set => base.entityOnDeath = value; }
}