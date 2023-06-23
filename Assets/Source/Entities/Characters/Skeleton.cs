using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Character
{
    public Animator Animator;

    public Skeleton(string id, string name) : base(id, name)
    {
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "skeleton_" + System.Guid.NewGuid().ToString();
        Name = "Skeleton";
    }

    public override List<Action> GetAvailableActions()
    {
        List<Action> actions = new List<Action>();

        // Get base actions
        foreach (Action action in BaseActions)
        {
            actions.Add(action);
        }

        return actions;
    }

    public override void PlayAnimation(string animationName)
    {
        base.PlayAnimation(animationName);

        Animator.Play("skeleton_" + animationName);
    }
}