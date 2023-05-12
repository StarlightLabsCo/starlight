using UnityEngine;

[CreateAssetMenu(fileName = "New Action Item", menuName = "Item/ActionItem")]
public class ActionItem : Item
{
    [SerializeField]
    public Action action;
}