using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ExecutableStats
{
    public string Name;
    public string Description;
    public int Slot;
    public string Sprite;
    public float Duration;
    public float Cooldown;
    public float Upkeep;

    public ExecutableStats(string name = "None", string description = "None", int slot = 0, string sprite = "Player/UI Images/None", float duration = 0f, float cooldown = 0f, float upkeep = 0f)
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
public abstract class Executable : MonoBehaviour
{
    [Header("Abilities String")]
    [SerializeField] protected ExecutableStats Stats;
    public bool OnCooldown = false;
    public bool Usable = false;

    public ExecutableStats GetStats()
    {
        return Stats;
    }

    public Sprite GetSprite(int slot)
    {
        return Resources.Load<Sprite>(Stats.Sprite);
    }
}
