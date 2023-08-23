using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image image;
    public Image indicator;
    Item displayedItem = null;

    public InventorySlot()
    {

    }

    public void SetItem(Item item)
    {
        displayedItem = item;
       
        if (displayedItem == null)
        {
            image.enabled = false;
        }
        else
        {
            image.sprite = displayedItem.sprite;
            image.enabled = true;
        }
    }
}
