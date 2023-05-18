using UnityEngine;

public class IronOreMedium : MineableEntity
{
    public IronOreMedium(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("IronOreSmall") as GameObject; set => base.entityOnDeath = value; }
}