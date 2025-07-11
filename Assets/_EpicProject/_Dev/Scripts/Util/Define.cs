namespace Define
{
    public enum SlotType
    {
        None,
        InventorySlot,
        EngineSlot,
        ToolBoxSlot,
        SimpleSlot
    }

    public enum BlockType
    {
        PlayerControl = 1,
        Scale,
        Rotate,
        Light,
        Graphic,
        Speed,
        Emotion,
        Sound,
        Mess,
    }

    public enum EngineActivationType
    {
        LeftToRightType,
        BottomToTopType,
    }

    public enum BgmType
    {
        None, // BGM이 없는 상태
        Menu,
        Tutorial,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage4_2,
        Ending,
    }

    public enum SfxType
    {
        Open, Close, Text, Clear, Dial, Grass, CaveButton
    }

    public enum PlayerSkinType
    {
        Default,
        BaldHead,
    }

    public enum GraphicType
    {
        Low,
        Middle,
        High,
        None,
    }

    public enum PatrolStateType
    {
        Patrol,
        Control,
        Return,
        None,
    }

    public static class Tags
    {
        public const string Player = "Player";
        public const string Slash = "Slash";
        public const string Goal = "Goal";
        public const string Ground = "Ground";
    }

    public static class Layers
    {
        public const int Clickable = 6;
        public const int Interactable = 7;
        public const int BridgeWall = 8;
    }
}