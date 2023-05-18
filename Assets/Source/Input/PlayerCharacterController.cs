using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacterController : ICharacterController
{
    public Vector2 direction;
    public bool isMouseClicked;
    public bool isEKeyPressed;
    public bool isQKeyPressed;
    private float lastActionTime;
    private float actionCooldown = 0f; // Cooldown in seconds between actions

    private float lastScrollTime;
    private float scrollCooldown = 0.2f;

    // Tools
    private List<string> availableTools = new List<string> { "Pickaxe", "Sword", "Axe" };
    private string currentTool;
    private int currentToolIndex = 0;



    public PlayerCharacterController()
    {
        currentTool = availableTools[currentToolIndex];
    }

    public void ProcessInput(Character character)
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Handle tool switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && Time.time - lastScrollTime > scrollCooldown)
        {
            if (scroll > 0) currentToolIndex--; // Scroll up
            else if (scroll < 0) currentToolIndex++; // Scroll down

            // Ensure the index is within the bounds of the list
            currentToolIndex = Mathf.Clamp(currentToolIndex, 0, availableTools.Count - 1);

            // Update current tool
            currentTool = availableTools[currentToolIndex];

            Debug.Log("Current tool: " + currentTool);

            lastScrollTime = Time.time; // Update the last scroll time
        }

        // Only register a mouse click if enough time has passed since the last action
        if (Input.GetMouseButtonDown(0) && Time.time - lastActionTime > actionCooldown)
        {
            isMouseClicked = true;
            lastActionTime = Time.time;
        }

        // Check for "E" key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            isEKeyPressed = true;
        }

        // Check for "Q" key press
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isQKeyPressed = true;
        }
    }

    public Task<Queue<Action>> GetActionQueueAsync(Character character, List<Action> availableActions)
    {
        Queue<Action> actionQueue = new Queue<Action>();

        if (direction != Vector2.zero)
        {
            actionQueue.Enqueue(new Move(direction));
        }

        if (isMouseClicked)
        {
            switch (currentTool)
            {
                case "Pickaxe":
                    actionQueue.Enqueue(new SwingPickaxe());
                    isMouseClicked = false; // Reset the flag after enqueuing the action
                    break;
                case "Sword":
                    actionQueue.Enqueue(new SwingSword());
                    isMouseClicked = false; // Reset the flag after enqueuing the action
                    break;
                case "Axe":
                    actionQueue.Enqueue(new SwingAxe());
                    isMouseClicked = false; // Reset the flag after enqueuing the action
                    break;
            }
        }

        // Check if "E" key was pressed
        if (isEKeyPressed)
        {
            // Find the first PickupItem action in the available actions list
            Action pickupItemAction = availableActions.Find(action => action is PickupItem);
            if (pickupItemAction != null)
            {
                actionQueue.Enqueue(pickupItemAction);
                isEKeyPressed = false; // Reset the flag after enqueuing the action
            }
        }

        // Check if "Q" key was pressed
        if (isQKeyPressed && character is IHasInventory)
        {
            IHasInventory inventory = character as IHasInventory;
            if (inventory.EntityInventory.Items.Count > 0)
            {
                // Enqueue a DropItem action for the first item in the inventory
                actionQueue.Enqueue(new DropItem(inventory.EntityInventory.Items[0]));
                isQKeyPressed = false; // Reset the flag after enqueuing the action
            }
        }

        return Task.FromResult(actionQueue);
    }
}
