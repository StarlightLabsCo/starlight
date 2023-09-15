using UnityEngine;

public class House : Entity
{
    [SerializeField]
    SpriteRenderer sleepIndicator;

    Character occupant;

    public House(string id, string name) : base("house", "House")
    {

    }

    protected override void Awake()
    {
        base.Awake();

        Id = "house_(" + gameObject.transform.position.x + "," + gameObject.transform.position.y + ")";
        Name = "House";
    }

    protected void Start()
    {
        WebSocketClient.Instance.houseDictionary.Add(this.Id, this);
    }

    public bool IsOccupied()
    {
        return occupant != null;
    }

    public bool Occupy(Character character)
    {
        if (!IsOccupied())
        {
            occupant = character;
            sleepIndicator.enabled = true;

            return true;
        } else
        {
            return false;
        }
    }

    public Character deoccupy()
    {
        Character character;
        if (IsOccupied())
        {
            character = occupant;
            occupant = null;
            sleepIndicator.enabled = false;

            return character;
        } else
        {
            return null;
        }
    }

    public void Update()
    {
        if (IsOccupied())
        {
            float worldTime = WorldTime.Instance.worldTime;
            float dayDuration = WorldTime.Instance.dayDuration;
            float morningStart = dayDuration * 0.2f;
            float morningEnd = dayDuration * 0.4f;

            float currentTimeInDay = worldTime % dayDuration;

            if (currentTimeInDay >= morningStart && currentTimeInDay <= morningEnd)
            {
                occupant.gameObject.SetActive(true);
                occupant.CurrentAnimation = null; // otherwise the animator plays the wrong hair type
                occupant.FinishAction();
            }
        }
    }
}
