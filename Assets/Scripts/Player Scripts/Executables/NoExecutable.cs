using UnityEngine; 
using UnityEngine.UI;
public class NoExecutable : Executable
{
    void Awake()
    {
        Usable = false;
        Stats.Name = "None";
        Stats.Description = "None";
        Stats.Sprite = "Player/UI Images/None";
    }
}
