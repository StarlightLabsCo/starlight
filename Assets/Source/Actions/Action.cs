public abstract class Action
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Action(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public override bool Equals(object obj)
    {
        if (obj is Action)
        {
            Action action = (Action)obj;
            return Id == action.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    // Start the action
    public abstract void Execute(Character character);

    // Update loop for the character 
    public abstract void Update(Character character);

    // Fixed update loop for the character - for physics, etc
    public abstract void FixedUpdate(Character character);

    public abstract void Cleanup(Character character);

}