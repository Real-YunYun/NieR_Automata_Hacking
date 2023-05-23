using Items.Executables;

namespace Items {
    public class NoExecutable : Executable {
        protected override void Awake() {
            Usable = false;
            Stats.Name = "None";
            Stats.Description = "None";
            Stats.Sprite = "Player/UI Images/None";
        }
    }
}