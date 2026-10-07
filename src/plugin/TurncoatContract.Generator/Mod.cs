using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using static Wave3Gen.Kit;
using static Wave3Gen.Vanilla;

namespace Wave3Gen;

/// <summary>The Turncoat Contract: everything specific to this mod. Kit/Program/Vanilla/ItemData are shared.</summary>
internal static class ModInfo
{
    public const string PluginName = "TurncoatContract.esp";
    public const string Description = "The Turncoat Contract: a Blackshield Company questline with a traitor, three endings and an upgradeable blade.";

    // TTC_Main aliases
    const int APlayer = 0, AHjalla = 1, AAmbushLoc = 2, AAmbushMarker = 3, ACaptain = 4, APayroll = 5, AOrders = 6,
        ATraitorMarker = 7, ATraitor = 8, AWrit = 9;
    // TTC_Intro aliases
    const int IPlayer = 0, IMarker = 1, IHjalla = 2;

    public static void Build(Kit k)
    {
        var qIntro = k.Id("TTC_Intro");
        var qMain = k.Id("TTC_Main");

        // --- Globals --------------------------------------------------------------------------------
        var gState = k.Global("TTC_State", 0);        // 0 new, 1 running, 2 done (arrest), 3 done (fight), 4 done (betrayed)
        var gMinLevel = k.Global("TTC_MinLevel", 8);
        var gReward = k.Global("TTC_RewardGold", 500);
        var gBribe = k.Global("TTC_BribeGold", 1500);
        var gBonus = k.Global("TTC_CompanyBonus", 250);
        var gBSC = k.Global("TTC_SeriesBSC", 0);      // 0 = Blackshield Company not installed, else BSC_Rank + 1
        var gSOT = k.Global("TTC_SeriesSOT", 0);      // 1 = Steel Oath installed
        var gCHT = k.Global("TTC_SeriesCHT", 0);      // 1 = Contract Hunter installed

        var company = k.Faction("TTC_CompanyFaction", "Blackshield Company");
        var turncoats = k.Faction("TTC_TurncoatFaction", "Vane's Turncoats");
        var holds = k.List("TTC_AmbushHolds", new[] { HaafingarHoldLocation, ReachHoldLocation, HjaalmarchHoldLocation, WhiterunHoldLocation, FalkreathHoldLocation });

        // --- Enchantments and items ------------------------------------------------------------------
        var enchII = k.WeaponEnchant("TTC_EnchCompanyEdge", "Company Edge", 25, (EnchShockDamageFFContact, 12, 0));
        var enchMercy = k.WeaponEnchant("TTC_EnchMercy", "Oath of Mercy", 35, (EnchAbsorbHealthFFContact, 15, 0), (EnchShockDamageFFContact, 12, 0));
        var enchJudgement = k.WeaponEnchant("TTC_EnchJudgement", "Oath of Judgement", 35, (EnchFireDamageFFContact, 25, 0), (EnchAbsorbStaminaFFContact, 15, 0));
        var bladeI = k.WeaponLike("Steel Sword", "TTC_OathbladeI", T.BladeI, 9, 120, null);
        var bladeII = k.WeaponLike("Steel Sword", "TTC_OathbladeII", T.BladeII, 11, 600, enchII, 1500);
        var bladeMercy = k.WeaponLike("Ebony Sword", "TTC_OathbladeMercy", T.BladeMercy, 15, 2200, enchMercy, 2500);
        var bladeJudge = k.WeaponLike("Ebony Sword", "TTC_OathbladeJudgement", T.BladeJudge, 15, 2200, enchJudgement, 2500);
        var shieldEnch = k.ArmorEnchant("TTC_EnchCaptainShield", "Captain's Guard", (EnchFortifyBlockConstantSelf, 15), (EnchResistMagicConstantSelf, 10));
        var shield = k.ArmorLike("Steel Shield", "TTC_CaptainShield", T.ShieldName, 26, 900, shieldEnch);
        var ringEnch = k.ArmorEnchant("TTC_EnchVaneRing", "Turncoat's Edge", (EnchFortifyOneHandedConstantSelf, 15), (EnchFortifyStaminaConstantSelf, 20));
        var ring = k.Jewelry("TTC_VaneRing", T.RingName, true, MeshRingGoldEmerald, RingGoldEmeraldAA, 650, ringEnch);

        var writ = k.Note("TTC_Writ", T.WritName, T.Writ);
        var orders = k.Note("TTC_Orders", T.OrdersName, T.Orders, second: true);
        var payroll = k.Ledger("TTC_Payroll", T.PayrollName, T.Payroll);

        // --- Actors ------------------------------------------------------------------------------------
        var hjalla = k.Speaker("TTC_Hjalla", T.HjallaName, T.HjallaShort, EncGuardSonsF01FemaleNord, ArmorSteelNoHelmetOutfit, company.FormKey, 10, 50,
            (K(0x013989), 1)); // Steel Sword
        var vane = k.Speaker("TTC_Vane", T.VaneName, T.VaneShort, EncGuardImperialM05MaleGuard, ArmorSteelPlateNoHelmetOutfit, turncoats.FormKey, 12, 50,
            (K(0x013989), 1), (ring.FormKey, 1), (Gold001, 220));
        vane.Configuration.Flags &= ~NpcConfiguration.Flag.Essential;
        var captain = k.Enemy("TTC_RaiderCaptain", T.CaptainName, T.CaptainShort, LCharBanditBoss, NordRace);

        // --- Messages ----------------------------------------------------------------------------------
        var msgNoStart = k.Msg("TTC_MsgNoStart", T.NoStart);
        var msgUpgrade = k.Msg("TTC_MsgUpgrade", T.Upgrade);

        // --- Intro quest (start game enabled: Hjalla and her opening topic) ----------------------------
        var intro = k.Quest(qIntro, "TTC_Intro", T.QuestName, true, Quest.TypeEnum.None, 50);
        intro.Aliases.Add(Player(IPlayer));
        intro.Aliases.Add(Forced(IMarker, "Camp", SolitudeStablesMapMarker));
        intro.Aliases.Add(Created(IHjalla, "Hjalla", hjalla.FormKey, IMarker, false, Level.Medium, QuestAlias.Flag.StoresText));
        intro.NextAliasID = 3;
        intro.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("TTC_IntroScript",
                Vmad.Obj("TTC_Main", qMain), Vmad.Obj("TTC_State", gState), Vmad.Obj("TTC_MsgNoStart", msgNoStart),
                Vmad.Alias("Hjalla", qIntro, IHjalla)))
            .WithAliasScripts(qIntro, IPlayer, Vmad.Script("TTC_SeriesScript",
                Vmad.Obj("TTC_SeriesBSC", gBSC), Vmad.Obj("TTC_SeriesSOT", gSOT), Vmad.Obj("TTC_SeriesCHT", gCHT)));

        // --- Main quest ------------------------------------------------------------------------------------
        var main = k.Quest(qMain, "TTC_Main", T.QuestName, false, Quest.TypeEnum.SideQuest, 60);
        main.TextDisplayGlobals.Add(L<IGlobalGetter>(gBribe));
        Stage(main, 0, null, flags: QuestStage.Flag.StartUpStage);
        Stage(main, 10, T.Log10);
        Stage(main, 20, T.Log20);
        Stage(main, 25, T.Log25);
        Stage(main, 30, T.Log30);
        Stage(main, 40, T.Log40);
        Stage(main, 50, T.Log50);
        Stage(main, 60, T.Log60);
        Stage(main, 100, T.Log100, QuestLogEntry.Flag.CompleteQuest);
        Stage(main, 110, T.Log110, QuestLogEntry.Flag.CompleteQuest);
        Stage(main, 120, T.Log120, QuestLogEntry.Flag.CompleteQuest);
        Objective(main, 10, T.Obj10, ACaptain);
        Objective(main, 20, T.Obj20);
        Objective(main, 21, T.Obj21, AHjalla);
        Objective(main, 30, T.Obj30, ATraitor);
        Objective(main, 50, T.Obj50, AHjalla);
        Objective(main, 60, T.Obj60, AHjalla);

        main.Aliases.Add(Player(APlayer));
        main.Aliases.Add(new QuestAlias { ID = AHjalla, Type = QuestAlias.TypeEnum.Reference, Name = "Hjalla", Flags = QuestAlias.Flag.Optional | QuestAlias.Flag.StoresText });
        main.Aliases.Add(Dungeon(AAmbushLoc, "AmbushLoc", holds.FormKey, LocTypeDungeon));
        main.Aliases.Add(MarkerOf(AAmbushMarker, "AmbushMarker", AAmbushLoc));
        main.Aliases.Add(Created(ACaptain, "Captain", captain.FormKey, AAmbushMarker, false, Level.Hard, QuestAlias.Flag.StoresText));
        main.Aliases.Add(Created(APayroll, "Payroll", payroll.FormKey, ACaptain, true, Level.Easy, QuestAlias.Flag.QuestObject));
        main.Aliases.Add(Created(AOrders, "Orders", orders.FormKey, ACaptain, true, Level.Easy, 0));
        main.Aliases.Add(Forced(ATraitorMarker, "TraitorMarker", FalkreathMapMarker));
        main.Aliases.Add(Created(ATraitor, "Traitor", vane.FormKey, ATraitorMarker, false, Level.Medium, QuestAlias.Flag.InitiallyDisabled | QuestAlias.Flag.StoresText));
        main.Aliases.Add(Created(AWrit, "Writ", writ.FormKey, APlayer, true, Level.Easy, 0));
        main.NextAliasID = (uint)main.Aliases.Count;
        main.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("TTC_MainScript",
                Vmad.Alias("Hjalla", qMain, AHjalla), Vmad.Alias("Payroll", qMain, APayroll), Vmad.Alias("Traitor", qMain, ATraitor),
                Vmad.Obj("TTC_State", gState), Vmad.Obj("TTC_RewardGold", gReward), Vmad.Obj("TTC_BribeGold", gBribe),
                Vmad.Obj("TTC_CompanyBonus", gBonus), Vmad.Obj("TTC_SeriesBSC", gBSC),
                Vmad.Obj("TTC_OathbladeI", bladeI.FormKey), Vmad.Obj("TTC_OathbladeII", bladeII.FormKey),
                Vmad.Obj("TTC_OathbladeMercy", bladeMercy.FormKey), Vmad.Obj("TTC_OathbladeJudgement", bladeJudge.FormKey),
                Vmad.Obj("TTC_CaptainShield", shield.FormKey), Vmad.Obj("TTC_VaneRing", ring.FormKey),
                Vmad.Obj("TTC_MsgUpgrade", msgUpgrade), Vmad.Obj("TTC_TurncoatFaction", turncoats.FormKey),
                Vmad.Obj("PlayerFaction", PlayerFaction), Vmad.Obj("Gold001", Gold001)))
            .WithAliasScripts(qMain, APayroll, Vmad.Script("TTC_PayrollAliasScript"))
            .WithAliasScripts(qMain, AOrders, Vmad.Script("TTC_OrdersAliasScript"))
            .WithAliasScripts(qMain, ATraitor, Vmad.Script("TTC_TraitorAliasScript"));

        // --- Dialogue ---------------------------------------------------------------------------------------
        var sHjalla = ("FemaleNord", T.HjallaName);
        var sVane = ("MaleGuard", T.VaneName);
        var isHjalla = Cond.IsID(hjalla.FormKey);
        var isVane = Cond.IsID(vane.FormKey);
        Condition State(int v) => GlobalIs(gState, CompareOperator.EqualTo, v);
        Condition St(CompareOperator op, int s) => StageIs(qMain, op, s);

        // Opening
        var bIntro = k.Id("TTC_IntroBranch");
        var tStart = k.Topic("TTC_I_Start", qIntro, bIntro, T.TopicStart, sHjalla);
        var tAccept = k.Topic("TTC_I_Accept", qIntro, bIntro, T.TopicAccept, sHjalla);
        var tDecline = k.Topic("TTC_I_Decline", qIntro, bIntro, T.TopicDecline, sHjalla);
        k.Branch(bIntro, "TTC_IntroBranch", qIntro, tStart);
        var offer = new[] { isHjalla, State(0), PlayerLevelAtLeast(gMinLevel), Cond.QuestRunning(CWSiegeObj, false), Cond.QuestRunning(qMain, false) };
        k.Info(tStart, "TTC_I_StartBSC", T.StartBSC, offer.Append(GlobalIs(gBSC, CompareOperator.GreaterThanOrEqualTo, 1)).ToArray(), links: new[] { tAccept, tDecline });
        k.Info(tStart, "TTC_I_StartOffer", T.Start, offer, links: new[] { tAccept, tDecline });
        k.Info(tStart, "TTC_I_StartDone", T.Done, new[] { isHjalla, GlobalIs(gState, CompareOperator.GreaterThanOrEqualTo, 2), GlobalIs(gState, CompareOperator.LessThanOrEqualTo, 3) }, prompt: T.PromptDone);
        k.Info(tStart, "TTC_I_StartCold", T.Cold, new[] { isHjalla, State(4) }, prompt: T.PromptCold, goodbye: true);
        k.Info(tAccept, "TTC_I_AcceptInfo", T.Accept, new[] { isHjalla }, goodbye: true, fragment: "TTC_TIF_Accept");
        k.Info(tDecline, "TTC_I_DeclineInfo", T.Decline, new[] { isHjalla }, goodbye: true);

        // Hjalla during the questline
        var tStatus = k.TopTopic("TTC_H_Status", qMain, T.TopicStatus, sHjalla);
        k.Info(tStatus, "TTC_H_Status10", T.Status10, new[] { isHjalla, St(CompareOperator.EqualTo, 10) });
        k.Info(tStatus, "TTC_H_Status20", T.Status20, new[] { isHjalla, St(CompareOperator.EqualTo, 20) });
        k.Info(tStatus, "TTC_H_Status30", T.Status30, new[] { isHjalla, St(CompareOperator.GreaterThanOrEqualTo, 30), St(CompareOperator.LessThan, 50) });

        var tReturn = k.TopTopic("TTC_H_Return", qMain, T.TopicReturn, sHjalla);
        k.Info(tReturn, "TTC_H_ReturnInfo", T.Return, new[] { isHjalla, St(CompareOperator.EqualTo, 25), PlayerHas(payroll.FormKey) }, fragment: "TTC_TIF_Return");

        var tReport = k.TopTopic("TTC_H_Report", qMain, T.TopicReport, sHjalla);
        k.Info(tReport, "TTC_H_ReportArrest", T.ReportArrest, new[] { isHjalla, St(CompareOperator.EqualTo, 50) }, goodbye: true, fragment: "TTC_TIF_Reward");
        k.Info(tReport, "TTC_H_ReportDead", T.ReportDead, new[] { isHjalla, St(CompareOperator.EqualTo, 60) }, goodbye: true, fragment: "TTC_TIF_Reward");

        // Vane
        var bVane = k.Id("TTC_VaneBranch");
        var tHello = k.Topic("TTC_V_Hello", qMain, bVane, T.TopicHello, sVane);
        var tWhy = k.Topic("TTC_V_Why", qMain, bVane, T.TopicWhy, sVane);
        var tArrest = k.Topic("TTC_V_Arrest", qMain, bVane, T.TopicArrest, sVane);
        var tFight = k.Topic("TTC_V_Fight", qMain, bVane, T.TopicFight, sVane);
        var tBribe = k.Topic("TTC_V_Bribe", qMain, bVane, T.TopicBribe, sVane);
        k.Branch(bVane, "TTC_VaneBranch", qMain, tHello);
        var facing = new[] { isVane, St(CompareOperator.GreaterThanOrEqualTo, 30), St(CompareOperator.LessThan, 50) };
        var choices = new[] { tWhy, tArrest, tFight, tBribe };
        k.Info(tHello, "TTC_V_HelloOath", T.HelloOath, facing.Append(GlobalIs(gSOT, CompareOperator.EqualTo, 1)).ToArray(), links: choices);
        k.Info(tHello, "TTC_V_HelloInfo", T.Hello, facing, links: choices);
        k.Info(tWhy, "TTC_V_WhyInfo", T.Why, facing, links: new[] { tArrest, tFight, tBribe });
        k.Info(tArrest, "TTC_V_ArrestInfo", T.Arrest, facing, goodbye: true, fragment: "TTC_TIF_Arrest");
        k.Info(tFight, "TTC_V_FightInfo", T.Fight, facing, goodbye: true, fragment: "TTC_TIF_Fight");
        k.Info(tBribe, "TTC_V_BribeInfo", T.Bribe, facing, goodbye: true, fragment: "TTC_TIF_Betray");
    }
}

/// <summary>All player-facing text. Dialogue responses stay within 149 characters.</summary>
internal static class T
{
    public const string QuestName = "The Turncoat Contract";
    public const string HjallaName = "Sergeant Hjalla Iron-Vow", HjallaShort = "Hjalla";
    public const string VaneName = "Corporal Edric Vane", VaneShort = "Vane";
    public const string CaptainName = "Turncoat Raider Captain", CaptainShort = "Raider Captain";

    public const string TopicStart = "You wear the Blackshield.";
    public static readonly string[] Start =
    {
        "Sharp eyes. Blackshield Company, Solitude detachment. What's left of it this week.",
        "Our payroll wagon never reached Falkreath. Four of ours with it. Raiders don't know our routes. Someone told them.",
        "I need a blade from outside the company. Someone who isn't on the rolls, so the traitor can't see them coming. Interested?",
    };
    public static readonly string[] StartBSC =
    {
        "I know your face from the company rolls. Good. I need someone I can trust, and that list got short this week.",
        "Our payroll wagon never reached Falkreath. Four of ours with it. Raiders don't know our routes. Someone told them.",
        "Find the wagon, find the coin, and find out who sold us. The captain will hear your name for it.",
    };
    public const string PromptDone = "How fares the company, Sergeant?";
    public static readonly string[] Done = { "Paid, fed and sleeping with both eyes closed again. That's your doing. Carry that blade with pride." };
    public const string PromptCold = "Sergeant...";
    public static readonly string[] Cold = { "You took Vane's coin. You're not company and you're not welcome. Walk on." };
    public const string TopicAccept = "I'll take the contract.";
    public static readonly string[] Accept =
    {
        "Then take this writ, and this. A company blade. Plain steel, but it'll grow with the work you do for us.",
        "The ambush was at <Alias=AmbushLoc>. Bring back the payroll ledger. It carries every route we've ridden.",
    };
    public const string TopicDecline = "Find someone else.";
    public static readonly string[] Decline = { "Then forget we spoke. Loose tongues got four of mine killed." };

    public const string TopicStatus = "About the payroll contract...";
    public static readonly string[] Status10 = { "The ambush was at <Alias=AmbushLoc>. Whoever led those raiders will have the ledger. Bring it back." };
    public static readonly string[] Status20 = { "You have the ledger? Then read what else the raider carried. I want a name before I want the coin." };
    public static readonly string[] Status30 = { "Vane's waiting near Falkreath. Arrest him if you can. Kill him if you must. Don't let him talk you sideways." };

    public const string TopicReturn = "I have the payroll ledger, and a name.";
    public static readonly string[] Return =
    {
        "Edric Vane. My own corporal. I gave him that stripe myself.",
        "Give me the ledger. And give me your blade a moment... There. Company steel, properly edged now. You've earned it.",
        "Vane fled toward Falkreath. Bring him back in chains, and I'll see the company's justice done. Or end him. Your call.",
    };

    public const string TopicReport = "It's done. Vane won't sell the company again.";
    public static readonly string[] ReportArrest =
    {
        "In chains, and alive to name everyone who paid him. That's worth more than his head.",
        "Hand me the blade one more time. Ebony, folded over the old steel. The Oath of Mercy. And take my old shield. You've earned both.",
    };
    public static readonly string[] ReportDead =
    {
        "Dead, then. I'll not pretend I'm sorry. Traitors don't get second oaths.",
        "Give me the blade. Ebony, folded over the old steel. The Oath of Judgement. Carry it the way you carried this contract.",
    };

    public const string TopicHello = "Edric Vane. The company sent me.";
    public static readonly string[] Hello = { "Hjalla sent a stranger. Clever. She always was. So: are you here to talk, or to bleed?" };
    public static readonly string[] HelloOath = { "You wear an oath like a ring, I can see it. So did I once. Are you here to talk, or to bleed?" };
    public const string TopicWhy = "Why sell out your own company?";
    public static readonly string[] Why =
    {
        "Eleven years in the Blackshield. Eleven. Then the company took a contract against my own village and called it business.",
        "So I sold them a wagon. Coin for coin. And I have more of it than any honest sergeant ever will.",
    };
    public const string TopicArrest = "Surrender. You'll answer to Hjalla.";
    public static readonly string[] Arrest = { "...Fine. I'm tired of running. Tell her I'll name names, if she keeps the noose off my neck." };
    public const string TopicFight = "There's only one answer for a turncoat.";
    public static readonly string[] Fight = { "Then let's see what the company taught you!" };
    public const string TopicBribe = "Make me an offer.";
    public static readonly string[] Bribe = { "<Global=TTC_BribeGold> gold and my ring. Tell Hjalla I'm dead in a ditch. Nobody ever needs to know otherwise." };

    public const string Log10 = "Sergeant Hjalla Iron-Vow of the Blackshield Company hired me to find an ambushed payroll wagon at <Alias=AmbushLoc>. She suspects a traitor in the company. She gave me a plain company blade that she says will grow with the work.";
    public const string Log20 = "I recovered the Blackshield payroll ledger from the raider captain. He carried sealed orders too.";
    public const string Log25 = "The sealed orders name the traitor: Corporal Edric Vane, Hjalla's own man. He is hiding near Falkreath. I should bring the ledger and the name to Hjalla.";
    public const string Log30 = "Hjalla took the ledger and re-edged my company blade. Now she wants Edric Vane brought to justice near Falkreath, in chains or otherwise.";
    public const string Log40 = "I confronted Edric Vane.";
    public const string Log50 = "Edric Vane surrendered and will answer to the company. I should report to Hjalla.";
    public const string Log60 = "Edric Vane is dead. I should report to Hjalla.";
    public const string Log100 = "Vane faces company justice, alive. Hjalla reforged my blade into the Oathblade of Mercy and gave me her old captain's shield.";
    public const string Log110 = "Vane is dead. Hjalla reforged my blade into the Oathblade of Judgement.";
    public const string Log120 = "I took Edric Vane's bribe and let him go. The Blackshield Company will not forget it.";

    public const string Obj10 = "Recover the payroll ledger at <Alias=AmbushLoc>";
    public const string Obj20 = "Read the raider captain's sealed orders";
    public const string Obj21 = "Bring the payroll ledger to Hjalla near the Solitude Stables";
    public const string Obj30 = "Deal with Corporal Edric Vane near Falkreath";
    public const string Obj50 = "Report Vane's arrest to Hjalla";
    public const string Obj60 = "Report Vane's death to Hjalla";

    public const string WritName = "Blackshield Contract Writ";
    public const string Writ = "BLACKSHIELD COMPANY - CONTRACT WRIT\n\nThe bearer acts for the Company in the matter of the lost payroll. Render aid. Ask no questions.\n\nAmbush site: <Alias=AmbushLoc>.\n\n- Sgt. Hjalla Iron-Vow, Solitude detachment";
    public const string OrdersName = "Sealed Orders";
    public const string Orders = "Captain,\n\nThe Blackshield wagon leaves Solitude on the third morning and takes the low road. Four guards, no outriders. Keep the coin, burn the wagon, leave no one to talk.\n\nIf the company comes asking, I'll be lying low near Falkreath until my share arrives.\n\n- E.V.";
    public const string PayrollName = "Blackshield Payroll Ledger";
    public const string Payroll = "BLACKSHIELD COMPANY - PAYROLL\n\nSolitude detachment, quarter's pay. Routes, wagons, and the names of every sword who draws company coin.\n\nThe last line, in a hurried hand: \"Low road. Third morning. Vane chose the route.\"";
    public const string BladeI = "Blackshield Oathblade";
    public const string BladeII = "Blackshield Oathblade, Edged";
    public const string BladeMercy = "Oathblade of Mercy";
    public const string BladeJudge = "Oathblade of Judgement";
    public const string ShieldName = "Hjalla's Captain Shield";
    public const string RingName = "Vane's Emerald Signet";
    public const string NoStart = "Hjalla shakes her head. \"Not today. The trail's not ready.\"\n\n(The questline could not start: there must be a free dungeon for the ambush site. Nothing has changed; try again later.)";
    public const string Upgrade = "Your Blackshield Oathblade has been reforged.";
}
