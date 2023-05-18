using UnityEngine;

public class DiamondOreMedium : MineableEntity
{
    public DiamondOreMedium(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("DiamondOreSmall") as GameObject; set => base.entityOnDeath = value; }
}