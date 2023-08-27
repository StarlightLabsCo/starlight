using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : ICharacterController
{
    public Vector2 direction;
    public bool isMouseClicked;
    public bool isEKeyPressed;
    public bool isQKeyPressed;

    // Inventory
    int selectedInventoryIndex = 0;
    int inventorySize = 0;

    public PlayerCharacterController(Character character)
    {
        if (character is IHasInventory iHasInventory)
        {
            inventorySize = iHasInventory.InventoryCapacity;
        }
    }

    // One thing to note is we have a bit of sphagetti design here
    // The player character controller completely ignores the GetAvailableActions function on the Human and just does it's own thing
    // The agent controller only does actions from the GetAvailableActions function
    public void ProcessInput(Character character)
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Handle tool switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int previousIndex = selectedInventoryIndex;
            if (scroll > 0) selectedInventoryIndex--; // Scroll up
            else if (scroll < 0) selectedInventoryIndex++; // Scroll down

            // Ensure the index is within the bounds of the list
            selectedInventoryIndex = Mathf.Clamp(selectedInventoryIndex, 0, inventorySize - 1);

            // If the selection has changed, update the UI
            if (previousIndex != selectedInventoryIndex)
            {
                InventoryUIManager.Instance.selectedItemIndex = selectedInventoryIndex;
                InventoryUIManager.Instance.Render();
            }
        }

        // Check for "E" key press
        if (Input.GetKeyDown(KeyCode.E) && character.interactableAction != null)
        {
            isEKeyPressed = true;
        }
       

        // If selected inventory index is not null
        if (character is IHasInventory iHasInventory)
        {
            // Handle switching inventory items with number keys
            for (int i = 0; i < Mathf.Min(9, iHasInventory.EntityInventory.Items.Length); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectedInventoryIndex = Mathf.Clamp(i, 0, inventorySize - 1);

                    InventoryUIManager.Instance.selectedItemIndex = selectedInventoryIndex;
                    InventoryUIManager.Instance.Render();                 
                }
            }

            // Handle 0 unique case
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                selectedInventoryIndex = inventorySize - 1;
                InventoryUIManager.Instance.selectedItemIndex = selectedInventoryIndex;
                InventoryUIManager.Instance.Render();
            }

            // Handle + or - key for switching inventory
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
            {
                selectedInventoryIndex = (selectedInventoryIndex + 1) % iHasInventory.EntityInventory.Items.Length;
                InventoryUIManager.Instance.selectedItemIndex = selectedInventoryIndex;
                InventoryUIManager.Instance.Render();
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                selectedInventoryIndex = (selectedInventoryIndex - 1 + iHasInventory.EntityInventory.Items.Length) % iHasInventory.EntityInventory.Items.Length;
                InventoryUIManager.Instance.selectedItemIndex = selectedInventoryIndex;
                InventoryUIManager.Instance.Render();
            }

            if (iHasInventory.EntityInventory.Items[selectedInventoryIndex] != null && character.CurrentAction == null)
            {
                // Register a mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    isMouseClicked = true;
                }

                // Check for "Q" key press
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    isQKeyPressed = true;
                }
            }
        }
    }

    public void RequestAction(Character character, List<Action> availableActions)
    {
        Queue<Action> actionQueue = new Queue<Action>();

        if (direction != Vector2.zero)
        {
            actionQueue.Enqueue(new Move(direction));
        }

        if (isMouseClicked && character is IHasInventory iHasInventory && iHasInventory.EntityInventory.Items[selectedInventoryIndex] is ActionItem)
        {
            actionQueue.Enqueue(((ActionItem)iHasInventory.EntityInventory.Items[selectedInventoryIndex]).action);
            isMouseClicked = false;
        }

        // Check if "Q" key was pressed
        if (isQKeyPressed && character is IHasInventory)
        {
            IHasInventory inventory = character as IHasInventory;

            if (inventory.EntityInventory.Items[selectedInventoryIndex] != null)
            {
                // Enqueue a DropItem action for the selected item in the inventory
                actionQueue.Enqueue(new DropItem(inventory.EntityInventory.Items[selectedInventoryIndex]));
                isQKeyPressed = false;
            }
            
        }

        // TODO: Check if "E" key is pressed and an interactable action is available
        if (isEKeyPressed)
        {
            actionQueue.Enqueue(character.interactableAction);
            isEKeyPressed = false;
        }

        for (int i = 0; i < actionQueue.Count; i++)
        {
            character.ActionQueue.Enqueue(actionQueue.Dequeue());
        }
    }
}
