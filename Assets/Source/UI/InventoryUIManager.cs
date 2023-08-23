using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public List<InventorySlot> slots = new List<InventorySlot>();

    public Inventory displayedInventory;

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
        } else
        {
            gameObject.SetActive(false);
        }
        Render(); 
    }

    public void Render()
    {
        for (int i = 0; i < displayedInventory.Items.Length; i++)
        {
            slots[i].SetItem(displayedInventory.Items[i]);
        }
    }
}
