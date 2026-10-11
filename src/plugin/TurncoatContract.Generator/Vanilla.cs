using Mutagen.Bethesda.Plugins;

namespace Wave3Gen;

/// <summary>
/// Vanilla Skyrim.esm records the third-wave mods link to (shared file, identical in every repo).
/// Only FormIDs are referenced; no Bethesda data or assets are copied. FormIDs were looked up with the
/// Mutagen.Bethesda.FormKeys.SkyrimSE 3.4.0 package. None of these records is overridden.
/// </summary>
internal static class Vanilla
{
    public static readonly ModKey Skyrim = ModKey.FromFileName("Skyrim.esm");
    public static FormKey K(uint id) => new(Skyrim, id);
    public static FormKey K(string hex) => new(Skyrim, Convert.ToUInt32(hex, 16));

    public static readonly FormKey PlayerRef = K(0x000014);
    public static readonly FormKey PlayerFaction = K(0x000DB1);
    public static readonly FormKey Gold001 = K(0x00000F);

    // Actors
    public static readonly FormKey NordRace = K(0x013746), DraugrRace = K(0x000D53);
    public static readonly FormKey ClassWarrior1H = K(0x013176);
    public static readonly FormKey CsHumanMelee = K(0x068849);
    public static readonly FormKey CsHumanBoss1H = K(0x03DECE);
    public static readonly FormKey LCharBanditMeleeAny = K(0x03DECD);
    public static readonly FormKey LCharBanditMeleeNordM = K(0x01A320);
    public static readonly FormKey LCharBanditBoss = K(0x03DF16);
    public static readonly FormKey LCharBanditBossNordM = K(0x01A341);
    public static readonly FormKey LCharDraugrMeleeAllMale = K(0x055955);
    public static readonly FormKey LCharDraugrBossNoDragonPriest = K(0x0DD9D8);
    public static readonly FormKey LCharForswornMeleeFemale = K(0x0442C8);
    // Traits templates (face, race, voice type) from vanilla guard look NPCs; the voice type is in the EditorID.
    public static readonly FormKey EncGuardSonsM01MaleNordCommander = K(0x0AA932);
    public static readonly FormKey EncGuardSonsF01FemaleNord = K(0x0AA922);
    public static readonly FormKey EncGuardSonsF02FemaleNord = K(0x0AA924);
    public static readonly FormKey EncGuardSonsM05MaleGuard = K(0x0AA942);
    public static readonly FormKey EncGuardImperialM05MaleGuard = K(0x0AA8FC);
    public static readonly FormKey EncGuardImperialM02MaleNordCommander = K(0x0AA8D5);
    public static readonly FormKey EncGuardImperialM07MaleGuard = K(0x0AA901);
    // Vanilla NPCs (unique actor fills only, never edited)
    public static readonly FormKey AdrianneAvenicci = K(0x013BB9); // voice type FemaleCommander (UESP)
    // Outfits
    public static readonly FormKey FineClothesOutfit02 = K(0x0E40DD);
    public static readonly FormKey ArmorSteelNoHelmetOutfit = K(0x028B44);
    public static readonly FormKey ArmorSteelPlateNoHelmetOutfit = K(0x0579A3);

    // Places
    public static readonly FormKey WhiterunHoldLocation = K(0x016772), HaafingarHoldLocation = K(0x016770), FalkreathHoldLocation = K(0x01676F),
        HjaalmarchHoldLocation = K(0x01676E), PaleHoldLocation = K(0x01676D), RiftHoldLocation = K(0x01676C), WinterholdHoldLocation = K(0x01676B),
        EastmarchHoldLocation = K(0x01676A), ReachHoldLocation = K(0x016769);
    public static readonly FormKey[] AllHolds =
    {
        K(0x016772), K(0x016770), K(0x01676F), K(0x01676E), K(0x01676D), K(0x01676C), K(0x01676B), K(0x01676A), K(0x016769),
    };
    public static readonly FormKey LocTypeDungeon = K(0x0130DB), LocTypeDraugrCrypt = K(0x0130E2), LocTypeBanditCamp = K(0x0130DF), LocTypeMilitaryFort = K(0x0130E7);
    public static readonly FormKey RefTypeMapMarker = K(0x10F63C);
    public static readonly FormKey RefTypeBoss = K(0x0130F7);
    public static readonly FormKey WhiterunStablesMapMarker = K(0x072879), SolitudeStablesMapMarker = K(0x01635E), MarkarthStablesMapMarker = K(0x0E962F),
        RiftenStablesMapMarker = K(0x096A46), RiverwoodMapMarker = K(0x0162A4), FalkreathMapMarker = K(0x017760), WhiterunWatchtowerMapMarker = K(0x0DB889);

    // Quests the openers stay out of the way of
    public static readonly FormKey CWSiegeObj = K(0x096E71);

    // Items, materials
    public static readonly FormKey IngotIron = K(0x05ACE4), IngotSteel = K(0x05ACE5), IngotEbony = K(0x05AD9D), IngotCorundum = K(0x05AD93),
        IngotQuicksilver = K(0x05ADA0), Leather01 = K(0x0DB5D2), DragonBone = K(0x03ADA4), DragonScales = K(0x03ADA3), DaedraHeart = K(0x03AD5B);
    public static readonly FormKey VendorItemBook = K(0x0937A2), VendorItemClutter = K(0x0914E9), MagicDisallowEnchanting = K(0x0C27BD);
    public static readonly FormKey ITMNoteUp = K(0x0C7A54), ITMGenericUpSD = K(0x03C7BA), ITMGenericDownSD = K(0x03C7B9);
    public static readonly FormKey InvArtNote01 = K(0x097788), InvArtNote02 = K(0x01541B), InvArtSpellTomeConjuration = K(0x02FBB6);
    public const string MeshLedger = @"Clutter\Books\SpellTomeConjurationLowPoly.nif";

    // Enchantment effects (MGEF) reused by this series' own enchantments
    public static readonly FormKey EnchFireDamageFFContact = K(0x04605A), EnchFrostDamageFFContact = K(0x04605B), EnchShockDamageFFContact = K(0x04605C),
        EnchAbsorbHealthFFContact = K(0x0AA155), EnchAbsorbStaminaFFContact = K(0x0AA157), EnchTurnUndeadFFContact = K(0x05B46B), EnchBanishFFContact = K(0x0ACBB5),
        EnchParalysisFFContact = K(0x0ACBB6);
    public static readonly FormKey EnchFortifyHealthConstantSelf = K(0x0493AA), EnchFortifyStaminaConstantSelf = K(0x049507), EnchFortifyMagickaConstantSelf = K(0x049504),
        EnchResistFireConstantSelf = K(0x048C8B), EnchResistFrostConstantSelf = K(0x048F45), EnchResistShockConstantSelf = K(0x049295),
        EnchResistMagicConstantSelf = K(0x0B7A35), EnchFortifyOneHandedConstantSelf = K(0x07A0FF), EnchFortifyTwoHandedConstantSelf = K(0x07A106),
        EnchFortifyBlockConstantSelf = K(0x07A0F3), EnchFortifyHeavyArmorConstantSelf = K(0x07A0F9), EnchFortifyHealRateConstantSelf = K(0x07A0F8),
        EnchFortifyStaminaRateConstantSelf = K(0x07A105), EnchFortifySmithingConstantSelf = K(0x07A102);

    // Jewelry (from Bounties of Oblivion's verified list)
    public static readonly FormKey DefaultRace = K(0x000019), ArmorJewelry = K(0x06BBE9), VendorItemJewelry = K(0x08F95A), JewelryExpensive = K(0x0A8664),
        ClothingNecklace = K(0x10CD0A), ClothingRing = K(0x10CD09);
    public static readonly FormKey GoldAmuletRubyAA = K(0x091720), GoldAmuletGemsAA = K(0x09171F), SilverAmuletEmeraldAA = K(0x09171E), SilverAmuletAA = K(0x09171A),
        RingSilverAmethystAA = K(0x08E840), RingGoldSapphireAA = K(0x08E83E), RingGoldEmeraldAA = K(0x08E83D), RingGoldDiamondAA = K(0x08E83C);
    public const string MeshAmuletGoldRuby = @"Armor\AmuletsandRings\GoldAmuletRubyGO.nif", MeshAmuletGoldGems = @"Armor\AmuletsandRings\GoldAmuletGemsGO.nif",
        MeshAmuletSilverEmerald = @"Armor\AmuletsandRings\SilverAmuletEmeraldGO.nif", MeshAmuletSilver = @"Armor\AmuletsandRings\SilverAmuletGO.nif",
        MeshRingSilverAmethyst = @"Armor\AmuletsandRings\SilverRingAmethystGO.nif", MeshRingGoldSapphire = @"Armor\AmuletsandRings\GoldRingSapphireGO.nif",
        MeshRingGoldEmerald = @"Armor\AmuletsandRings\GoldRingEmeraldGO.nif", MeshRingGoldDiamond = @"Armor\AmuletsandRings\GoldRingDiamondGO.nif";
    public const string MeshAmethyst = @"Clutter\Gemstones\Amethyst.nif", MeshSkull = @"Clutter\Bones\HumanSkullFull.nif",
        MeshGyro = @"Clutter\Dwemer\DwarvenGyro.nif";

    // Meshes (by path)
    public const string MeshNote01 = @"Clutter\Books\Note01.nif";
    public const string MeshNote02 = @"Clutter\Books\Note02.nif";
    public const string MeshHorkerTusk = @"Clutter\HorkerTusk\HorkerTusk.nif";
}
