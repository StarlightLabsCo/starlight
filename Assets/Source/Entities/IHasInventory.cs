public interface IHasInventory
{
    public Inventory EntityInventory { get; set; }
    public int InventoryCapacity { get; set; }
}