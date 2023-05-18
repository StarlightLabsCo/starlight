using UnityEngine;

public class CoalOreSmall : MineableEntity
{
    public CoalOreSmall(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
}