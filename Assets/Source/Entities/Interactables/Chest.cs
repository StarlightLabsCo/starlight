using System.Collections;
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

    protected override void Awake()
    {
        base.Awake();

        Id = "chest_(" + gameObject.transform.position.x + "," + gameObject.transform.position.y + ")";
        Name = "Chest";
    }

    protected void Start()
    {
        EntityInventory = new Inventory(InventoryCapacity);
        Id = "chest_(" + gameObject.transform.position.x + "," + gameObject.transform.position.y + ")";
    }

    public List<Item> ViewItems()
    {
        return entityInventory.AsList();
    }

    public bool AddItem(Item item)
    {
        bool added = EntityInventory.Add(item);

        Debug.Log("Added " + item.Name + " to chest? " + added);

        if (added)
        {
            Debug.Log("Added " + item.Name + " to chest");
        }
        else
        {
            Debug.LogError("Failed to add " + item.Name + " to chest");
        }

        return added;
    }

    public bool RemoveItem(Item item)
    {
        return EntityInventory.Remove(item);
    }
}