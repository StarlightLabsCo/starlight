using UnityEngine;
using UnityEngine.UI;

public class StatUIManager : MonoBehaviour
{
    public static StatUIManager Instance { get; private set; }

    public Image satietyBar;
    public Sprite[] satietyBarSprites;

    public IHasStomach displayedSatiety;

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
    }

    void Start()
    {
        if (displayedSatiety != null)
        {
            setSatietyBar(1, 1);
        } else
        {
            gameObject.SetActive(false);
        }
    }

    public void setSatietyBar(float satiety, float maxSatiety)
    {
        if (satiety >= maxSatiety)
        {
            satietyBar.enabled = false;
            return;
        }

        satietyBar.enabled = true;

        float ratio = satiety / maxSatiety;

        // For Satiety we're rounding because it looks nicer, whereas with health bars we don't round atm.
        int spriteIndex = Mathf.RoundToInt(ratio * (satietyBarSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, satietyBarSprites.Length - 1);

        satietyBar.sprite = satietyBarSprites[spriteIndex];
    }
}
