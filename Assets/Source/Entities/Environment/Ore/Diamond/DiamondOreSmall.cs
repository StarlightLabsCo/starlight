using UnityEngine;

public class DiamondOreSmall : MineableEntity
{
    public DiamondOreSmall(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
}