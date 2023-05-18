using UnityEngine;

public class StoneOreMedium : MineableEntity
{
    public StoneOreMedium(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("StoneOreSmall") as GameObject; set => base.entityOnDeath = value; }
}