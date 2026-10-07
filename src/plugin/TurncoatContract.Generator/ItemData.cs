using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wave3Gen;

/// <summary>Vanilla weapon and armor descriptions read from vanilla-items.json (copied from Ember Smithing).</summary>
internal sealed class ItemData
{
    [JsonPropertyName("weapons")] public List<WeaponDef> Weapons { get; set; } = new();
    [JsonPropertyName("armors")] public List<ArmorDef> Armors { get; set; } = new();

    public static ItemData Load(string path)
        => JsonSerializer.Deserialize<ItemData>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
           ?? throw new InvalidDataException(path);
}

internal sealed class WeaponDef
{
    public string FormId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Material { get; set; } = "";
    public string Type { get; set; } = "";
    public float Weight { get; set; }
    public uint Value { get; set; }
    public ushort Damage { get; set; }
    public ushort Crit { get; set; }
    public float Speed { get; set; }
    public float Reach { get; set; }
    public float Stagger { get; set; }
    public string Model { get; set; } = "";
    public short[]? Bounds { get; set; }
    public string EquipType { get; set; } = "";
    public string BlockBashImpact { get; set; } = "";
    public string AltBlockMaterial { get; set; } = "";
    public List<string> Keywords { get; set; } = new();
    public string ImpactDataSet { get; set; } = "";
    public string FirstPersonModel { get; set; } = "";
    public string AttackFailSound { get; set; } = "";
    public string EquipSound { get; set; } = "";
    public string UnequipSound { get; set; } = "";
    public string AnimationType { get; set; } = "";
    public string Skill { get; set; } = "";
    public int Unknown2 { get; set; }
    public byte NumProjectiles { get; set; }
    public float AnimationAttackMult { get; set; }
    public float RumbleLeft { get; set; }
    public float RumbleRight { get; set; }
    public float RumbleDuration { get; set; }
    public float CritPercentMult { get; set; }
    public bool CritOnDeath { get; set; }
    public string DetectionSoundLevel { get; set; } = "";
}

internal sealed class ArmorDef
{
    public string FormId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Material { get; set; } = "";
    public string Slot { get; set; } = "";
    public float Weight { get; set; }
    public uint Value { get; set; }
    public float Rating { get; set; }
    public string Model { get; set; } = "";
    public string? FemaleModel { get; set; }
    public short[]? Bounds { get; set; }
    public List<string> Keywords { get; set; } = new();
    public List<string> Armature { get; set; } = new();
    public List<string> FirstPersonFlags { get; set; } = new();
    public string ArmorType { get; set; } = "";
    public string Race { get; set; } = "";
    public string? EquipType { get; set; }
    public string? BashImpact { get; set; }
    public string? AltBlockMaterial { get; set; }
    public bool Shield { get; set; }
}
