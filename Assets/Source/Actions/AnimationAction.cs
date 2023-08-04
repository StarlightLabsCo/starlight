public abstract class AnimationAction : Action
{
    protected AnimationAction(string id, string name, string description, string parameters) : base(id, name, description, parameters)
    {
    }

    // Trigger the effect of the action - mainly for animations
    public abstract void TriggerEffect(Character character);

}