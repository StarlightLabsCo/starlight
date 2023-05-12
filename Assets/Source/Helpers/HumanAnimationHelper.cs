using UnityEngine;

public class HumanAnimationHelper : MonoBehaviour
{
    private Human Human;

    public void Start()
    {
        Human = GetComponentInParent<Human>();
    }

    public void TriggerActionEffect()
    {
        Human.TriggerActionEffect();
    }

    public void FinishAction()
    {
        Human.FinishAction();
    }
}