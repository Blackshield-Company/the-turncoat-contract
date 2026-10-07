using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using static Wave3Gen.Vanilla;

namespace Wave3Gen;

/// <summary>
/// Record helpers shared by the third-wave generators (identical file in every repo). Quests, aliases,
/// unvoiced dialogue, notes, NPCs, enchanted weapons/armor/jewelry and abilities.
/// </summary>
internal sealed class Kit
{
    public SkyrimMod Mod { get; }
    public FormIdRegistry Ids { get; }
    public ItemData Items { get; }
    public static readonly SkyrimRelease R = SkyrimRelease.SkyrimSE;
    public const byte VanillaQuestFormVersion = 65;

    /// <summary>Who can speak each topic: (voice type folder, speaker label). One silent voice file per voice type.</summary>
    public Dictionary<FormKey, (string VoiceType, string Speaker)[]> Speakers { get; } = new();
    public HashSet<string> ExpectedUnfilled { get; } = new();

    public Kit(SkyrimMod mod, FormIdRegistry ids, ItemData items)
    {
        Mod = mod;
        Ids = ids;
        Items = items;
    }

    public FormKey Id(string editorId) => Ids.Get(editorId);
    public static FormLink<T> L<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);
    public static FormLinkNullable<T> N<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);

    public T Add<T>(T record) where T : SkyrimMajorRecord
    {
        Mod.GetTopLevelGroup<T>().Add(record);
        return record;
    }

    // ---------------------------------------------------------------------------------------------
    // Basic records
    // ---------------------------------------------------------------------------------------------

    public FormKey Global(string edid, float value)
    {
        var g = new GlobalFloat(Id(edid), R) { EditorID = edid, Data = value };
        Mod.Globals.Add(g);
        return g.FormKey;
    }

    public FormKey Msg(string edid, string text)
    {
        var m = new Message(Id(edid), R) { EditorID = edid, Description = text, Flags = Message.Flag.MessageBox, INAM = new byte[4] };
        Mod.Messages.Add(m);
        return m.FormKey;
    }

    public Faction Faction(string edid, string name) => Add(new Faction(Id(edid), R) { EditorID = edid, Name = name });

    public FormList List(string edid, IEnumerable<FormKey> items)
    {
        var l = Add(new FormList(Id(edid), R) { EditorID = edid });
        foreach (var i in items) l.Items.Add(L<ISkyrimMajorRecordGetter>(i));
        return l;
    }

    public Book Note(string edid, string name, string text, bool second = false) => Add(new Book(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ObjectBounds = Bounds(-8, -5, 0, 8, 5, 0),
        Model = new Model { File = second ? MeshNote02 : MeshNote01 },
        BookText = text,
        PickUpSound = N<ISoundDescriptorGetter>(ITMNoteUp),
        Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
        Type = Book.BookType.NoteOrScroll,
        Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
        Value = 0,
        Weight = 0,
        InventoryArt = N<IStaticGetter>(second ? InvArtNote02 : InvArtNote01),
        Description = "",
    });

    public Book Ledger(string edid, string name, string text) => Add(new Book(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ObjectBounds = Bounds(-9, -11, -1, 9, 11, 2),
        Model = new Model { File = MeshLedger },
        BookText = text,
        Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
        Type = Book.BookType.BookOrTome,
        Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
        Value = 0,
        Weight = 1,
        InventoryArt = N<IStaticGetter>(InvArtSpellTomeConjuration),
        Description = "",
    });

    public MiscItem Misc(string edid, string name, string mesh, uint value, float weight, ObjectBounds? bounds = null) => Add(new MiscItem(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ObjectBounds = bounds ?? Bounds(-4, -4, 0, 4, 4, 4),
        Model = new Model { File = mesh },
        PickUpSound = N<ISoundDescriptorGetter>(ITMGenericUpSD),
        PutDownSound = N<ISoundDescriptorGetter>(ITMGenericDownSD),
        Keywords = new() { L<IKeywordGetter>(VendorItemClutter) },
        Value = value,
        Weight = weight,
    });

    // ---------------------------------------------------------------------------------------------
    // Magic: enchantments, abilities, own value-modifier effects
    // ---------------------------------------------------------------------------------------------

    /// <summary>A constant-effect self enchantment for armor/jewelry built from vanilla or own effects.</summary>
    public FormKey ArmorEnchant(string edid, string name, params (FormKey Effect, float Magnitude)[] effects)
    {
        var e = new ObjectEffect(Id(edid), R)
        {
            EditorID = edid, Name = name, Flags = ObjectEffect.Flag.NoAutoCalc, CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self, EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment, EnchantmentCost = 0, EnchantmentAmount = 0,
        };
        foreach (var (fx, mag) in effects) e.Effects.Add(new Effect { BaseEffect = N<IMagicEffectGetter>(fx), Data = new EffectData { Magnitude = mag } });
        return Add(e).FormKey;
    }

    /// <summary>An on-strike weapon enchantment (fire and forget, touch).</summary>
    public FormKey WeaponEnchant(string edid, string name, uint costPerUse, params (FormKey Effect, float Magnitude, int Duration)[] effects)
    {
        var e = new ObjectEffect(Id(edid), R)
        {
            EditorID = edid, Name = name, Flags = ObjectEffect.Flag.NoAutoCalc, CastType = CastType.FireAndForget,
            TargetType = TargetType.Touch, EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment, EnchantmentCost = costPerUse, EnchantmentAmount = 0,
        };
        foreach (var (fx, mag, dur) in effects) e.Effects.Add(new Effect { BaseEffect = N<IMagicEffectGetter>(fx), Data = new EffectData { Magnitude = mag, Duration = dur } });
        return Add(e).FormKey;
    }

    /// <summary>Own constant self value modifier (e.g. DamageResist for armor rating), usable in abilities.</summary>
    public FormKey ValueEffect(string edid, string name, ActorValue av, string description)
        => Add(new MagicEffect(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            Flags = MagicEffect.Flag.Recover | MagicEffect.Flag.NoArea | MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoHitEvent,
            MagicSkill = ActorValue.None, ResistValue = ActorValue.None, SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.ValueModifier, ActorValue = av },
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = description,
        }).FormKey;

    /// <summary>A permanent ability (shows under Active Effects). Granted and removed by script.</summary>
    public FormKey Ability(string edid, string name, string description, params (FormKey Effect, float Magnitude)[] effects)
    {
        var s = new Spell(Id(edid), R)
        {
            EditorID = edid, Name = name, Description = description, Type = SpellType.Ability, CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self, Flags = SpellDataFlag.ManualCostCalc, BaseCost = 0, ObjectBounds = new ObjectBounds(),
        };
        foreach (var (fx, mag) in effects) s.Effects.Add(new Effect { BaseEffect = N<IMagicEffectGetter>(fx), Data = new EffectData { Magnitude = mag } });
        return Add(s).FormKey;
    }

    // ---------------------------------------------------------------------------------------------
    // Weapons and armor, built on the shape of a vanilla piece (vanilla-items.json)
    // ---------------------------------------------------------------------------------------------

    public Weapon WeaponLike(string vanillaName, string edid, string name, int damage, uint value, FormKey? enchantment, ushort charge = 0)
    {
        var w = Items.Weapons.Single(x => x.Name == vanillaName);
        var weapon = new Weapon(Id(edid), R)
        {
            EditorID = edid,
            ObjectBounds = Bounds(w.Bounds),
            Name = name,
            Model = new Model { File = w.Model },
            EquipmentType = N<IEquipTypeGetter>(K(w.EquipType)),
            BlockBashImpact = N<IImpactDataSetGetter>(K(w.BlockBashImpact)),
            AlternateBlockMaterial = N<IMaterialTypeGetter>(K(w.AltBlockMaterial)),
            Description = "",
            ImpactDataSet = N<IImpactDataSetGetter>(K(w.ImpactDataSet)),
            FirstPersonModel = N<IStaticGetter>(K(w.FirstPersonModel)),
            AttackFailSound = N<ISoundDescriptorGetter>(K(w.AttackFailSound)),
            EquipSound = N<ISoundDescriptorGetter>(K(w.EquipSound)),
            UnequipSound = N<ISoundDescriptorGetter>(K(w.UnequipSound)),
            BasicStats = new WeaponBasicStats { Value = value, Weight = w.Weight, Damage = (ushort)damage },
            Data = new WeaponData
            {
                AnimationType = Enum.Parse<WeaponAnimationType>(w.AnimationType),
                Speed = w.Speed, Reach = w.Reach, NumProjectiles = w.NumProjectiles, AnimationAttackMult = w.AnimationAttackMult,
                Unknown2 = w.Unknown2, RumbleLeftMotorStrength = w.RumbleLeft, RumbleRightMotorStrength = w.RumbleRight,
                RumbleDuration = w.RumbleDuration, Skill = Enum.Parse<Skill>(w.Skill), Stagger = w.Stagger,
            },
            Critical = new CriticalData { Damage = (ushort)(damage / 2), PercentMult = w.CritPercentMult, Flags = w.CritOnDeath ? CriticalData.Flag.OnDeath : 0 },
            DetectionSoundLevel = Enum.Parse<SoundLevel>(w.DetectionSoundLevel),
        };
        if (enchantment is FormKey ench)
        {
            weapon.ObjectEffect = N<IObjectEffectGetter>(ench);
            weapon.EnchantmentAmount = charge == 0 ? (ushort)2000 : charge;
        }
        weapon.Keywords = w.Keywords.Append(MagicDisallowEnchanting.ID.ToString("X6"))
            .Select(k => (IFormLinkGetter<IKeywordGetter>)L<IKeywordGetter>(K(k))).ToExtendedList();
        return Add(weapon);
    }

    public Armor ArmorLike(string vanillaName, string edid, string name, float rating, uint value, FormKey? enchantment)
    {
        var a = Items.Armors.Single(x => x.Name == vanillaName);
        var armor = new Armor(Id(edid), R)
        {
            EditorID = edid,
            ObjectBounds = Bounds(a.Bounds),
            Name = name,
            WorldModel = new GenderedItem<ArmorModel?>(
                new ArmorModel { Model = new Model { File = a.Model } },
                a.FemaleModel == null ? null : new ArmorModel { Model = new Model { File = a.FemaleModel } }),
            BodyTemplate = new BodyTemplate
            {
                FirstPersonFlags = a.FirstPersonFlags.Select(Enum.Parse<BipedObjectFlag>).Aggregate((BipedObjectFlag)0, (x, y) => x | y),
                ArmorType = Enum.Parse<ArmorType>(a.ArmorType),
            },
            Race = N<IRaceGetter>(K(a.Race)),
            Description = "",
            Value = value,
            Weight = a.Weight,
            ArmorRating = rating,
        };
        if (a.EquipType != null) armor.EquipmentType = N<IEquipTypeGetter>(K(a.EquipType));
        if (a.BashImpact != null) armor.BashImpactDataSet = N<IImpactDataSetGetter>(K(a.BashImpact));
        if (a.AltBlockMaterial != null) armor.AlternateBlockMaterial = N<IMaterialTypeGetter>(K(a.AltBlockMaterial));
        if (enchantment is FormKey ench) armor.ObjectEffect = N<IObjectEffectGetter>(ench);
        if (a.Shield) armor.MajorFlags |= Armor.MajorFlag.Shield;
        armor.Keywords = a.Keywords.Append(MagicDisallowEnchanting.ID.ToString("X6"))
            .Select(k => (IFormLinkGetter<IKeywordGetter>)L<IKeywordGetter>(K(k))).ToExtendedList();
        armor.Armature.AddRange(a.Armature.Select(x => (IFormLinkGetter<IArmorAddonGetter>)L<IArmorAddonGetter>(K(x))));
        return Add(armor);
    }

    public Armor Jewelry(string edid, string name, bool ring, string mesh, FormKey addon, uint value, FormKey enchantment) => Add(new Armor(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ObjectBounds = ring ? Bounds(-1, -1, 0, 1, 1, 0) : Bounds(-3, -9, 0, 3, 9, 1),
        ObjectEffect = N<IObjectEffectGetter>(enchantment),
        WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel { Model = new Model { File = mesh } }, null),
        BodyTemplate = new BodyTemplate { FirstPersonFlags = ring ? BipedObjectFlag.Ring : BipedObjectFlag.Amulet, ArmorType = ArmorType.Clothing, ActsLike44 = true },
        Race = N<IRaceGetter>(DefaultRace),
        Keywords = new()
        {
            L<IKeywordGetter>(ArmorJewelry), L<IKeywordGetter>(VendorItemJewelry), L<IKeywordGetter>(JewelryExpensive),
            L<IKeywordGetter>(ring ? ClothingRing : ClothingNecklace), L<IKeywordGetter>(MagicDisallowEnchanting),
        },
        Description = "",
        Armature = { L<IArmorAddonGetter>(addon) },
        Value = value,
        Weight = 0.25f,
    });

    // ---------------------------------------------------------------------------------------------
    // NPCs
    // ---------------------------------------------------------------------------------------------

    public const NpcConfiguration.TemplateFlag EnemyFlags = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats
        | NpcConfiguration.TemplateFlag.Factions | NpcConfiguration.TemplateFlag.SpellList | NpcConfiguration.TemplateFlag.AIData
        | NpcConfiguration.TemplateFlag.AIPackages | NpcConfiguration.TemplateFlag.ModelAnimation | NpcConfiguration.TemplateFlag.Inventory
        | NpcConfiguration.TemplateFlag.Script | NpcConfiguration.TemplateFlag.DefPackList | NpcConfiguration.TemplateFlag.AttackData
        | NpcConfiguration.TemplateFlag.Keywords;

    /// <summary>A hostile actor that takes everything from a vanilla leveled character list.</summary>
    public Npc Enemy(string edid, string name, string shortName, FormKey template, FormKey race) => Add(TemplatedNpc(edid, name, shortName, template, EnemyFlags, race));

    /// <summary>
    /// A speaking NPC: only Traits (face, race, voice type) come from a vanilla guard look NPC whose voice type is
    /// in its EditorID, so the silent voice files can be placed for it. Own stats, outfit, faction and AI.
    /// </summary>
    public Npc Speaker(string edid, string name, string shortName, FormKey traitsTemplate, FormKey outfit, FormKey faction, short minLevel, short maxLevel, params (FormKey Item, int Count)[] items)
    {
        var n = Add(TemplatedNpc(edid, name, shortName, traitsTemplate, NpcConfiguration.TemplateFlag.Traits, NordRace));
        n.Configuration.Level = new PcLevelMult { LevelMult = 1.1f };
        n.Configuration.CalcMinLevel = minLevel;
        n.Configuration.CalcMaxLevel = maxLevel;
        n.Configuration.Flags |= NpcConfiguration.Flag.Essential;
        n.CombatStyle = N<ICombatStyleGetter>(CsHumanBoss1H);
        n.DefaultOutfit = N<IOutfitGetter>(outfit);
        n.Factions.Add(new RankPlacement { Faction = L<IFactionGetter>(faction), Rank = 0 });
        n.AIData = new AIData { Aggression = Aggression.Unaggressive, Confidence = Confidence.Brave, Assistance = Assistance.HelpsAllies, EnergyLevel = 50 };
        n.Items = items.Select(i => new ContainerEntry { Item = new ContainerItem { Item = L<IItemGetter>(i.Item), Count = i.Count } }).ToExtendedList();
        return n;
    }

    public Npc TemplatedNpc(string edid, string name, string shortName, FormKey template, NpcConfiguration.TemplateFlag flags, FormKey race) => new(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ShortName = shortName,
        ObjectBounds = Bounds(-22, -14, 0, 22, 14, 128),
        Template = new FormLinkNullable<INpcSpawnGetter>(template),
        Race = L<IRaceGetter>(race),
        Class = L<IClassGetter>(ClassWarrior1H),
        CombatStyle = N<ICombatStyleGetter>(CsHumanMelee),
        Configuration = new NpcConfiguration
        {
            Flags = NpcConfiguration.Flag.AutoCalcStats,
            TemplateFlags = flags,
            Level = new PcLevelMult { LevelMult = 1 },
            CalcMinLevel = 1,
            CalcMaxLevel = 0,
            SpeedMultiplier = 100,
            DispositionBase = 35,
        },
        AIData = new AIData { Aggression = Aggression.Aggressive, Confidence = Confidence.Average, Assistance = Assistance.HelpsAllies, EnergyLevel = 50 },
        PlayerSkills = new PlayerSkills(),
        Height = 1,
        Weight = 50,
    };

    // ---------------------------------------------------------------------------------------------
    // Quests and aliases
    // ---------------------------------------------------------------------------------------------

    public Quest Quest(FormKey fk, string edid, string name, bool startGameEnabled, Quest.TypeEnum type, byte priority)
        => Add(new Quest(fk, R)
        {
            EditorID = edid,
            Name = name,
            Flags = startGameEnabled ? Mutagen.Bethesda.Skyrim.Quest.Flag.StartGameEnabled : 0,
            Priority = priority,
            Type = type,
            QuestFormVersion = VanillaQuestFormVersion,
        });

    public static void Stage(Quest q, ushort index, string? log, QuestLogEntry.Flag logFlags = 0, QuestStage.Flag flags = 0)
    {
        var s = new QuestStage { Index = index, Flags = flags };
        if (log != null) s.LogEntries.Add(new QuestLogEntry { Flags = logFlags, Entry = log });
        q.Stages.Add(s);
    }

    public static void Objective(Quest q, ushort index, string text, params int[] targets)
    {
        var o = new QuestObjective { Index = index, DisplayText = text };
        foreach (var t in targets) o.Targets.Add(new QuestObjectiveTarget { AliasID = t });
        q.Objectives.Add(o);
    }

    public static QuestAlias Player(int id) => new() { ID = (uint)id, Type = QuestAlias.TypeEnum.Reference, Name = "Player", ForcedReference = new FormLinkNullable<IPlacedGetter>(PlayerRef) };

    public static QuestAlias Forced(int id, string name, FormKey reference) => new() { ID = (uint)id, Type = QuestAlias.TypeEnum.Reference, Name = name, ForcedReference = new FormLinkNullable<IPlacedGetter>(reference) };

    public static QuestAlias Unique(int id, string name, FormKey npc, QuestAlias.Flag flags = 0)
        => new() { ID = (uint)id, Type = QuestAlias.TypeEnum.Reference, Name = name, UniqueActor = N<INpcGetter>(npc), Flags = flags == 0 ? null : flags };

    /// <summary>A dungeon with a map marker in one of the listed holds, reserved so other quests (and other aliases) don't take it.</summary>
    public static QuestAlias Dungeon(int id, string name, FormKey holdsList, FormKey locType)
    {
        var loc = new QuestAlias
        {
            ID = (uint)id, Type = QuestAlias.TypeEnum.Location, Name = name,
            Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText | QuestAlias.Flag.AllowCleared,
        };
        loc.Conditions.Add(Cond.InCurrentLocFormList(holdsList));
        loc.Conditions.Add(Cond.LocationHasKeyword(locType));
        loc.Conditions.Add(Cond.LocationHasRefType(RefTypeMapMarker));
        return loc;
    }

    public static QuestAlias MarkerOf(int id, string name, int locationAlias) => new()
    {
        ID = (uint)id, Type = QuestAlias.TypeEnum.Reference, Name = name,
        Location = new LocationAliasReference { AliasID = locationAlias, RefType = N<ILocationReferenceTypeGetter>(RefTypeMapMarker) },
    };

    public static QuestAlias Created(int id, string name, FormKey obj, int at, bool inside, Level level, QuestAlias.Flag flags = 0)
        => new()
        {
            ID = (uint)id, Type = QuestAlias.TypeEnum.Reference, Name = name,
            Flags = flags == 0 ? null : flags,
            CreateReferenceToObject = new CreateReferenceToObject
            {
                Object = L<ISkyrimMajorRecordGetter>(obj), AliasID = (short)at,
                Create = inside ? CreateReferenceToObject.CreateEnum.In : CreateReferenceToObject.CreateEnum.At, Level = level,
            },
        };

    // ---------------------------------------------------------------------------------------------
    // Unvoiced dialogue
    // ---------------------------------------------------------------------------------------------

    public void Branch(FormKey fk, string edid, FormKey quest, DialogTopic starting)
        => Add(new DialogBranch(fk, R)
        {
            EditorID = edid,
            Quest = L<IQuestGetter>(quest),
            Category = DialogBranch.CategoryType.Player,
            Flags = DialogBranch.Flag.TopLevel,
            StartingTopic = N<IDialogTopicGetter>(starting.FormKey),
        });

    public DialogTopic Topic(string edid, FormKey quest, FormKey branch, string text, params (string VoiceType, string Speaker)[] speakers)
    {
        var t = Add(new DialogTopic(Id(edid), R)
        {
            EditorID = edid,
            Name = text,
            Priority = 50,
            Branch = N<IDialogBranchGetter>(branch),
            Quest = N<IQuestGetter>(quest),
            Category = DialogTopic.CategoryEnum.Topic,
            Subtype = DialogTopic.SubtypeEnum.Custom,
            SubtypeName = new RecordType("CUST"),
        });
        Speakers[t.FormKey] = speakers;
        return t;
    }

    /// <summary>A top-level topic with its own branch (branch EditorID = topic EditorID + "B").</summary>
    public DialogTopic TopTopic(string edid, FormKey quest, string text, params (string VoiceType, string Speaker)[] speakers)
    {
        var b = Id(edid + "B");
        var t = Topic(edid, quest, b, text, speakers);
        Branch(b, edid + "B", quest, t);
        return t;
    }

    public void Info(DialogTopic topic, string edid, string[] lines, Condition[] conditions, string? prompt = null,
        bool goodbye = false, string? fragment = null, DialogTopic[]? links = null)
    {
        var info = new DialogResponses(Id(edid), R)
        {
            EditorID = edid,
            Flags = new DialogResponseFlags { Flags = goodbye ? DialogResponses.Flag.Goodbye : 0 },
            PreviousDialog = topic.Responses.Count == 0 ? new FormLinkNullable<IDialogResponsesGetter>() : N<IDialogResponsesGetter>(topic.Responses[^1].FormKey),
        };
        if (prompt != null) info.Prompt = prompt;
        for (int i = 0; i < lines.Length; i++)
            info.Responses.Add(new DialogResponse { Emotion = Emotion.Neutral, EmotionValue = 50, ResponseNumber = (byte)(i + 1), Text = lines[i] });
        info.Conditions.AddRange(conditions);
        if (links != null)
            foreach (var l in links) info.LinkTo.Add(L<IDialogTopicGetter>(l.FormKey));
        if (fragment != null) info.VirtualMachineAdapter = Vmad.InfoFragment(fragment);
        topic.Responses.Add(info);
    }

    // ---------------------------------------------------------------------------------------------
    // Conditions
    // ---------------------------------------------------------------------------------------------

    public static Condition StageIs(FormKey quest, CompareOperator op, int s) => Cond.Stage(quest, op, s);
    public static Condition GlobalIs(FormKey g, CompareOperator op, float v) => Cond.Global(g, op, v);
    public static Condition PlayerHas(FormKey item, int count = 1) => Cond.ItemCount(item, CompareOperator.GreaterThanOrEqualTo, count).OnRef(PlayerRef);
    public static Condition PlayerLacks(FormKey item, int count = 1) => Cond.ItemCount(item, CompareOperator.LessThan, count).OnRef(PlayerRef);
    public static Condition PlayerLevelAtLeast(FormKey global) => Cond.MakeVsGlobal(new GetLevelConditionData(), global, CompareOperator.GreaterThanOrEqualTo).OnRef(PlayerRef);
    public static Condition PlayerGoldAtLeast(FormKey global) => Cond.MakeVsGlobal(Cond.ItemCountData(Gold001), global, CompareOperator.GreaterThanOrEqualTo).OnRef(PlayerRef);
    public static Condition PlayerGoldBelow(FormKey global) => Cond.MakeVsGlobal(Cond.ItemCountData(Gold001), global, CompareOperator.LessThan).OnRef(PlayerRef);

    public static ObjectBounds Bounds(short x1, short y1, short z1, short x2, short y2, short z2)
        => new() { First = new P3Int16(x1, y1, z1), Second = new P3Int16(x2, y2, z2) };

    public static ObjectBounds Bounds(short[]? b)
    {
        b ??= new short[] { -28, -28, 0, 28, 28, 18 };
        return new() { First = new P3Int16(b[0], b[1], b[2]), Second = new P3Int16(b[3], b[4], b[5]) };
    }
}
