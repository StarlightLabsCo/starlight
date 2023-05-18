using UnityEngine;

public class IronOreSmall : MineableEntity
{
    public IronOreSmall(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
}