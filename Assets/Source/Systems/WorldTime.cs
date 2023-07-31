using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldTime : MonoBehaviour
{
    public float duration = 5f;

    [SerializeField]
    private Gradient gradient;
    private Light2D light2D;
    private float startTime;

    public static WorldTime Instance { get; private set; }

    float hour = 0;
    float minute = 0;

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
        startTime = Time.time;
    }

    private void Update()
    {
        float timeElapsed = Time.time - startTime;
        float percentage = (timeElapsed / duration) % 1;

        hour = percentage * 24;
        minute = (hour % 1) * 60;

        light2D.color = gradient.Evaluate(percentage);
    }

    public string GetTime()
    {
        return Mathf.FloorToInt(hour) + ":" + Mathf.FloorToInt(minute).ToString("D2");
    }
}