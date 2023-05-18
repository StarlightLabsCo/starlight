using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerCharacterController : ICharacterController
{
    public Vector2 direction;
    public bool isMouseClicked;
    private float lastActionTime;
    private float actionCooldown = 0f; // Cooldown in seconds between actions

    // Tools
    private List<string> availableTools = new List<string> { "Pickaxe", "Axe" };
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
        if (scroll != 0)
        {
            if (scroll > 0) currentToolIndex--; // Scroll up
            else if (scroll < 0) currentToolIndex++; // Scroll down

            // Ensure the index is within the bounds of the list
            currentToolIndex = Mathf.Clamp(currentToolIndex, 0, availableTools.Count - 1);

            // Update current tool
            currentTool = availableTools[currentToolIndex];
        }

        // Only register a mouse click if enough time has passed since the last action
        if (Input.GetMouseButtonDown(0) && Time.time - lastActionTime > actionCooldown)
        {
            isMouseClicked = true;
            lastActionTime = Time.time;
        }
        else
        {
            isMouseClicked = false;
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
                    break;
                case "Axe":
                    actionQueue.Enqueue(new SwingAxe());
                    break;
            }
        }

        return Task.FromResult(actionQueue);
    }
}