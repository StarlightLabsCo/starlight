using UnityEngine;
using UnityEngine.UI;

public class StatUIManager : MonoBehaviour
{
    public static StatUIManager Instance { get; private set; }

    public Image satietyBar;
    public Sprite[] satietyBarSprites;

    public Image energyBar;
    public Sprite[] energyBarSprites;

    public IHasStomach displayedSatiety;

    public Character displayedCharacter;

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
        if (displayedSatiety == null && displayedCharacter == null)
        {
            gameObject.SetActive(false);
        }

        if (displayedSatiety != null)
        {
            setSatietyBar(1, 1);
        }

        if (displayedCharacter != null)
        {
            setEnergyBar(1, 1);
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

    public void setEnergyBar(float energy, float maxEnergy)
    {
        Debug.Log("SetEnergyBar");

        if (energy >= maxEnergy)
        {
            energyBar.enabled = false;
            return;
        }

        energyBar.enabled = true;

        float ratio = energy / maxEnergy;

        int spriteIndex = Mathf.RoundToInt(ratio * (energyBarSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, energyBarSprites.Length - 1);

        energyBar.sprite = energyBarSprites[spriteIndex];
    }
}
