using UnityEngine; 
using UnityEngine.UI;
public class NoAbility : Ability
{
    void Awake()
    {
        Usable = false;
        Stats.Name = "None";
        Stats.Description = "None";
        Stats.Sprite = "Player/UI Images/None";
    }
}
