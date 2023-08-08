using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldTime : MonoBehaviour
{
    public static WorldTime Instance { get; private set; }

    public float dayDuration = 5f;

    [SerializeField]
    private Gradient gradient;
    private Light2D light2D;

    public float worldTime = 0f;

    private float previousTime = 0f;

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

        light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        // Calculate the difference in time since the last frame
        float difference = Time.time - previousTime;
        previousTime = Time.time;

        worldTime += difference;

        float percentage = (worldTime / dayDuration) % 1;

        light2D.color = gradient.Evaluate(percentage);
    }

    public void SetTime(float time)
    {
        worldTime = time;
    }
}
