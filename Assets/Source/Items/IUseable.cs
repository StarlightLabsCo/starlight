public interface IUseable
{
    public Action Action { get; set; }

    public abstract void Use(Character character);
}