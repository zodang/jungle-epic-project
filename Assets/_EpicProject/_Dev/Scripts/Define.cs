namespace Define
{
    public enum SlotType
    {
        Inventory,
        Inspector,
    }

    public enum BlockType
    {
        PlayerControl = 1,
        Scale,
        Rotate,
        Light,
        Sound,
        Mess,
    }

    public enum SfxType
    {
        Click, Close, Deny, Dial, Get, Open, Text, Put, Clear, Grass
    }

    public enum PlayerSkinType
    {
        Default,
        BaldHead,
    }

    public static class Tags
    {
        public const string Player = "Player";
        public const string Slash = "Slash";
        public const string Goal = "Goal";
    }

    public static class Layers
    {
        public const int Clickable = 6;
        public const int Interactable = 7;
        public const int BridgeWall = 8;
    }
}