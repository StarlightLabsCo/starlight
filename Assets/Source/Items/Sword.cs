public class Sword : Item, IUseable, IDropable, IPickupable
{
    public Action Action { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    public Sword(string id, string name) : base(id, name)
    {

    }

    public void Use(Character character)
    {
        throw new System.NotImplementedException();
    }

    public void Pickup(Character character)
    {
        throw new System.NotImplementedException();
    }

    public void Drop(Character character)
    {
        throw new System.NotImplementedException();
    }


}