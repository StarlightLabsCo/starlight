using System;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

using static Utilities;


public class Eat : Action
{
    // TODO: make a way for it to be constructed with all the available options a character has in their inventory so I can adjust the function
    // parameters accordingly

    // List<FoodId> availableFoodOptions = new List<FoodId>();

    EatableItem selectedItem;

    public Eat(EatableItem eatableItem) : base("eat", "eat", "Eat some food.", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is adding the item to the chest."
            },
            foodId = new
            {
                type = "string",
                description = "The food ID of the food to be eaten.",
                values = Enum.GetNames(typeof(FoodId))
            }
        }
    }))
    {
        selectedItem = eatableItem;
    }

    public override void Execute(Character character)
    {
        // Does the character have an inventory?
        if (character is IHasInventory)
        {
            // Does the character have a stomach and are they less than full satiety?
            if (character is IHasStomach  && ((IHasStomach)character).Satiety < ((IHasStomach)character).MaxSatiety)
            {

                // Attempt to remove item from inventory
                bool wasRemoved = ((IHasInventory)character).EntityInventory.Remove(selectedItem);
                if (wasRemoved)
                {
                    Debug.Log($"{character.Name} just ate {selectedItem.Name} which restored {selectedItem.satiety} satiety.");

                    // TODO: add eating specific animation?
                    character.PlayRemoveItemAnimation(selectedItem.sprite);

                    // TODO: clamp between 0 and MaxSatiety - at least until you add overeat debuff and/or puke
                    ((IHasStomach)character).Satiety = Mathf.Clamp(((IHasStomach)character).Satiety + selectedItem.satiety, 0, ((IHasStomach)character).MaxSatiety);
                }
                else
                {
                    Debug.Log($"{character.Name} was unable to eat {selectedItem.Name}. Guessing because it wasn't actually in their inventory.");
                }
            }
        }

        character.FinishAction();
    }

    public override void Update(Character character)
    {
       
    }

    public override void FixedUpdate(Character character)
    {
        
    }

    public override void Cleanup(Character character)
    {
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "ActionExecuted",
                data = new
                {
                    characterId = character.Id.ToString(),
                    result = $"{character.Name} ate {selectedItem.Name} which restored {selectedItem.satiety} satiety.",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}
