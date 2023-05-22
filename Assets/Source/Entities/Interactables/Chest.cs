using System.Collections.Generic;
using UnityEngine;

public class Chest : Entity, IHasInventory
{
    private Inventory entityInventory;
    public Inventory EntityInventory { get => entityInventory; set => entityInventory = value; }

    [SerializeField]
    private int inventoryCapacity;
    public int InventoryCapacity { get => inventoryCapacity; set => inventoryCapacity = value; }

    public Chest(string id, string name) : base("chest", "Chest")
    {

    }

    protected void Start()
    {
        EntityInventory = new Inventory(InventoryCapacity);
        Id = "chest";
        Name = "Chest";
    }

    public List<Item> ViewItems()
    {
        return EntityInventory.Items;
    }

    public bool AddItem(Item item)
    {
        return EntityInventory.Add(item);
    }

    public bool RemoveItem(Item item)
    {
        return EntityInventory.Remove(item);
    }
}