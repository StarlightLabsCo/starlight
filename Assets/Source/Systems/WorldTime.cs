using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldTime : MonoBehaviour
{
    public float duration = 5f;

    [SerializeField]
    private Gradient gradient;
    private Light2D light2D;
    private float startTime;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        startTime = Time.time;
    }

    private void Update()
    {
        float timeElapsed = Time.time - startTime;
        float percentage = Mathf.Sin(timeElapsed / duration * Mathf.PI * 2) / 2 + 0.5f;

        percentage = Mathf.Clamp(percentage, 0, 1);

        light2D.color = gradient.Evaluate(percentage);
    }
}