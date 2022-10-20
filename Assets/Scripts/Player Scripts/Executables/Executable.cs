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

[System.Serializable, DefaultExecutionOrder(2)]
public abstract class Executable : MonoBehaviour
{
    [Header("Executable Fields")]
    [SerializeField] protected ExecutableStats Stats;
    public bool OnCooldown = false;
    public bool Usable = false;

    public ExecutableStats GetStats()
    {
        return Stats;
    }

    protected virtual void Awake()
    {
        Usable = false;
        Stats.Name = "";
        Stats.Description = "";
        Stats.Sprite = "Player/UI Images/None";
        Stats.Duration = 0f;
        Stats.Cooldown = 0f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    protected virtual void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            Stats.Upkeep += Time.unscaledDeltaTime;
            if (Stats.Upkeep >= Stats.Cooldown)
            {
                Stats.Upkeep = 0;
                OnCooldown = false;
                this.enabled = false;
            }
        }
    }

    public Sprite GetSprite(int slot)
    {
        return Resources.Load<Sprite>(Stats.Sprite);
    }
}
