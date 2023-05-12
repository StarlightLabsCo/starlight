public abstract class AnimationAction : Action
{
    protected AnimationAction(string id, string name, string description) : base(id, name, description)
    {
    }

    // Trigger the effect of the action - mainly for animations
    public abstract void TriggerEffect(Character character);

}