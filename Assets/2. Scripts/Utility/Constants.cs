// enum
public enum SceneType
{
    Title,
    Tutorial,
    Home,
    PoorTown,
    RichTown,
    RuralTown,
    End,
}
public enum Sound
{
    Bgm,
    Sfx,
    SfxLoop,
    MaxCount
}
public enum Ambience
{
    BonFire,
    Breeze,
    Clock,
    Coughing,
    DesertWind,
    Electric,
    EmptyHouse,
    Fly,
    Hospital,
    Mart,
    MotoCycles,
    Rain,
    Sewer,
}
public enum BGM
{
    Title,
    Tutorial,
    PoorTown,
    RichTown,
    RuralTown,
    End
}
public enum SFX
{
    AnimalWalk,
    CardKey,
    Click,
    Dialog,
    Door,
    DropItem,
    DropAntibiotics,
    Gain,
    Inventory,
    Menu,
    PossessCat,
    PossessDog,
    Save,
    Soul,
    SoulCancel,
    Throw,
    Walk,
    DoorClick,
    DoorClear,
    DoorFail
}

// class

// 경로 상수
public static class Prefab
{
    public const string PLAYER = "Player";
    public const string MUSIC = "Music";
    public const string INVENTORY = "Inventory";
    public const string NPC = "NPC";
}
public static class ResourcePath
{
    public const string CHARACTER = "Character/";
    public const string MAP = "Map/";
    public const string STAGE = "Stage/";
    public const string BGM = "Music/Background";
    public const string SFX = "Music/Effect";
    public const string AMBIENCE = "Music/Ambience";
    public const string UI = "Prefabs/UI/";
    public const string SPRITE = "Sprite/";
}

// 오디오 상수
public static class AudioVolume
{
    public const string BGM = "BgmVolume";
    public const string SFX = "SfxVolume";
}

// UI
public static class UI
{
    public const string FADEIN = "FadeIn";
    public const string FADEOUT = "FadeOut";
}

// 상수
public static class Constant
{
    public const float ZERO = 0.0f;
    public const float FIVE = 5.0f;
}