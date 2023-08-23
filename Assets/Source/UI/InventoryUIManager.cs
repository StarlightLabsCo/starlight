using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public List<InventorySlot> slots = new List<InventorySlot>();

    public Inventory displayedInventory;

    public int selectedItemIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (displayedInventory != null)
        {
            displayedInventory.subscribedUI = this;
            Render();
        } else
        {
            gameObject.SetActive(false);
        }
    }

    public void Render()
    {
        if (displayedInventory == null)
        {
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < displayedInventory.Items.Length; i++)
        {
            slots[i].SetItem(displayedInventory.Items[i]);

            if (i == selectedItemIndex)
            {
                slots[i].indicator.enabled = true;
            } else
            {
                slots[i].indicator.enabled = false;
            }
        }
    }
}
