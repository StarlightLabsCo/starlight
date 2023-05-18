using UnityEngine;

public class StoneOreSmall : MineableEntity
{
    public StoneOreSmall(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
}