using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AbilityStats
{
    public string Name;
    public string Description;
    public int Slot;
    public string Sprite;
    public float Duration;
    public float Cooldown;
    public float Upkeep;

    public AbilityStats(string name = "None", string description = "None", int slot = 0, string sprite = "Player/UI Images/None", float duration = 0f, float cooldown = 0f, float upkeep = 0f)
    {
        Name = name;
        Description = description;
        Slot = slot;
        Sprite = sprite;
        Duration = duration;
        Cooldown = cooldown;
        Upkeep = upkeep;
    }
}

[System.Serializable]
public abstract class Ability : MonoBehaviour
{
    [Header("Abilities String")]
    [SerializeField] protected AbilityStats Stats;
    public bool OnCooldown = false;
    public bool Usable = false;

    public AbilityStats GetStats()
    {
        return Stats;
    }

    public Sprite GetSprite(int slot)
    {
        return Resources.Load<Sprite>(Stats.Sprite);
    }
}
