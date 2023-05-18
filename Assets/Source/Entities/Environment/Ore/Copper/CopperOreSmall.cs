using UnityEngine;

public class CopperOreSmall : MineableEntity
{
    public CopperOreSmall(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
}