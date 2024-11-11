public class InputManager {
    static InputManager instance;
    public static InputManager Instance {
        get {
            if (instance == null) {
                instance = new InputManager();
                instance.Character.Enable();
                instance.World.Enable();
                instance.Game.Enable();
            }
            return instance;
        }
    }
    private InputManager() {
        Control = new PlayerControl();
        Character = new CharacterControl();
        World = new WorldControl();
        Dialogue = new DialogueControl();
        Game = new GameControl();
        UI = new UIControl();
    }

    public PlayerControl Control { get; private set; }

    public CharacterControl Character { get; private set; }
    public WorldControl World { get; private set; }
    public DialogueControl Dialogue { get; private set; }
    public GameControl Game { get; private set; }
    public UIControl UI { get; private set; }

    interface IControl {
        public void Enable();
        public void Disable();
    }

    public class CharacterControl : IControl {
        public void Enable() { Instance.Control.Character.Enable(); }
        public void Disable() { Instance.Control.Character.Disable(); }
        public PlayerControl.CharacterActions Actions {
            get {
                // if (!Instance.Control.Character.enabled) {
                //     Debug.LogWarning("CharacterActions are currently not enabled");
                // }
                return Instance.Control.Character;
            }
        }
        public float HorizontalMove => Actions.HorizontalMove.ReadValue<float>();
        public float VerticalMove => Actions.VerticalMove.ReadValue<float>();
    }

    public class WorldControl : IControl {
        public void Enable() { Instance.Control.World.Enable(); }
        public void Disable() { Instance.Control.World.Disable(); }
        public PlayerControl.WorldActions Actions {
            get {
                // if (!Instance.Control.World.enabled) {
                //     Debug.LogWarning("WorldActions are currently not enabled");
                // }
                return Instance.Control.World;
            }
        }
    }

    public class DialogueControl : IControl {
        public void Enable() { Instance.Control.Dialogue.Enable(); }
        public void Disable() { Instance.Control.Dialogue.Disable(); }
        public PlayerControl.DialogueActions Actions {
            get {
                // if (!Instance.Control.Dialogue.enabled) {
                //     Debug.LogWarning("WorldActions are currently not enabled");
                // }
                return Instance.Control.Dialogue;
            }
        }
    }

    public class GameControl : IControl {
        public void Enable() { Instance.Control.Game.Enable(); }
        public void Disable() { Instance.Control.Game.Disable(); }
        public PlayerControl.GameActions Actions {
            get {
                // if (!Instance.Control.Game.enabled) {
                //     Debug.LogWarning("GameActions are currently not enabled");
                // }
                return Instance.Control.Game;
            }
        }
    }

    public class UIControl : IControl {
        public void Enable() { Instance.Control.UI.Enable(); }
        public void Disable() { Instance.Control.UI.Disable(); }
        public PlayerControl.UIActions Actions {
            get {
                // if (!Instance.Control.UI.enabled) {
                //     Debug.LogWarning("UIActions are currently not enabled");
                // }
                return Instance.Control.UI;
            }
        }
    }
}
