Scriptname TTC_MainScript extends Quest
{The Turncoat Contract - questline controller.
Stages: 10 find the ambush, 20 payroll ledger taken, 25 orders read (Vane revealed), 30 ledger returned (blade II),
40 Vane confronted, 50 Vane arrested, 60 Vane dead, 100/110/120 completed (arrest / killed / bribed).}

ReferenceAlias Property Hjalla Auto
ReferenceAlias Property Payroll Auto
ReferenceAlias Property Traitor Auto
GlobalVariable Property TTC_State Auto
GlobalVariable Property TTC_RewardGold Auto
GlobalVariable Property TTC_BribeGold Auto
GlobalVariable Property TTC_CompanyBonus Auto
GlobalVariable Property TTC_SeriesBSC Auto
Weapon Property TTC_OathbladeI Auto
Weapon Property TTC_OathbladeII Auto
Weapon Property TTC_OathbladeMercy Auto
Weapon Property TTC_OathbladeJudgement Auto
Armor Property TTC_CaptainShield Auto
Armor Property TTC_VaneRing Auto
Message Property TTC_MsgUpgrade Auto
Faction Property TTC_TurncoatFaction Auto
Faction Property PlayerFaction Auto
MiscObject Property Gold001 Auto

Bool OrdersWereRead = False
Bool PendingDismiss = False
Bool PendingStop = False

Function Begin(ObjectReference akHjalla)
	If akHjalla
		Hjalla.ForceRefTo(akHjalla)
	EndIf
	Game.GetPlayer().AddItem(TTC_OathbladeI, 1)
	SetStage(10)
	SetObjectiveDisplayed(10)
EndFunction

; Payroll alias: the ledger moved into the player's inventory.
Function PayrollTaken()
	If GetStage() != 10
		Return
	EndIf
	SetObjectiveCompleted(10)
	SetStage(20)
	If OrdersWereRead
		Reveal()
	Else
		SetObjectiveDisplayed(20)
	EndIf
EndFunction

; Orders alias: the sealed orders were read (possibly before the ledger was picked up).
Function OrdersRead()
	OrdersWereRead = True
	If GetStage() == 20
		Reveal()
	EndIf
EndFunction

Function Reveal()
	SetObjectiveCompleted(20)
	SetStage(25)
	ObjectReference vane = Traitor.GetReference()
	If vane && vane.IsDisabled()
		vane.Enable()
	EndIf
	SetObjectiveDisplayed(21)
EndFunction

; Hjalla dialogue: hand over the ledger and the name. First upgrade.
Function ReturnPayroll()
	If GetStage() != 25
		Return
	EndIf
	ObjectReference item = Payroll.GetReference()
	If item
		Game.GetPlayer().RemoveItem(item.GetBaseObject(), 1, True)
	EndIf
	Upgrade(TTC_OathbladeI, TTC_OathbladeII)
	SetObjectiveCompleted(21)
	SetStage(30)
	SetObjectiveDisplayed(30)
EndFunction

Function Arrest()
	If GetStage() < 30 || GetStage() >= 50
		Return
	EndIf
	SetStage(40)
	SetObjectiveCompleted(30)
	SetStage(50)
	SetObjectiveDisplayed(50)
	PendingDismiss = True
	RegisterForSingleUpdate(4.0)
EndFunction

Function Fight()
	If GetStage() < 30 || GetStage() >= 50
		Return
	EndIf
	SetStage(40)
	TTC_TurncoatFaction.SetEnemy(PlayerFaction)
	Actor vane = Traitor.GetActorReference()
	If vane && !vane.IsDead()
		vane.SetActorValue("Aggression", 1.0)
		vane.StartCombat(Game.GetPlayer())
	EndIf
EndFunction

; Traitor alias OnDeath, whoever killed him.
Function TraitorDied()
	Int stage = GetStage()
	If stage < 25 || stage >= 50
		Return
	EndIf
	If stage == 25
		; Killed before the ledger went back: Hjalla takes both at the report.
		SetObjectiveDisplayed(21, False)
	EndIf
	SetObjectiveCompleted(30)
	SetStage(60)
	SetObjectiveDisplayed(60)
EndFunction

Function Betray()
	If GetStage() < 30 || GetStage() >= 50
		Return
	EndIf
	SetStage(40)
	Actor player = Game.GetPlayer()
	player.AddItem(Gold001, TTC_BribeGold.GetValueInt())
	player.AddItem(TTC_VaneRing, 1)
	SetObjectiveFailed(30)
	TTC_State.SetValue(4)
	SetStage(120)
	Finish(True)
EndFunction

; Hjalla dialogue: report. Final upgrade depends on how Vane's story ended.
Function Reward()
	Int stage = GetStage()
	If stage != 50 && stage != 60
		Return
	EndIf
	Actor player = Game.GetPlayer()
	If !GetStageDone(30)
		; Vane died before the ledger was returned.
		ObjectReference item = Payroll.GetReference()
		If item
			player.RemoveItem(item.GetBaseObject(), 1, True)
		EndIf
		Upgrade(TTC_OathbladeI, TTC_OathbladeII)
	EndIf
	Int gold = TTC_RewardGold.GetValueInt()
	If TTC_SeriesBSC.GetValueInt() >= 1
		gold += TTC_CompanyBonus.GetValueInt()
	EndIf
	player.AddItem(Gold001, gold)
	If stage == 50
		Upgrade(TTC_OathbladeII, TTC_OathbladeMercy)
		player.AddItem(TTC_CaptainShield, 1)
		SetObjectiveCompleted(50)
		TTC_State.SetValue(2)
		SetStage(100)
	Else
		Upgrade(TTC_OathbladeII, TTC_OathbladeJudgement)
		SetObjectiveCompleted(60)
		TTC_State.SetValue(3)
		SetStage(110)
	EndIf
	Finish(False)
EndFunction

; Swap one tier of the blade for the next, keeping it equipped. If the old tier was sold or lost, the new one is still given.
Function Upgrade(Weapon akFrom, Weapon akTo)
	Actor player = Game.GetPlayer()
	Bool wasEquipped = player.IsEquipped(akFrom)
	If player.GetItemCount(akFrom) > 0
		player.RemoveItem(akFrom, 1, True)
	EndIf
	player.AddItem(akTo, 1)
	If wasEquipped
		player.EquipItem(akTo, False, True)
	EndIf
	TTC_MsgUpgrade.Show()
EndFunction

Function Finish(Bool abDismiss)
	PendingDismiss = abDismiss
	PendingStop = True
	RegisterForSingleUpdate(4.0)
EndFunction

Event OnUpdate()
	If PendingDismiss
		PendingDismiss = False
		Actor vane = Traitor.GetActorReference()
		If vane && !vane.IsDead() && !vane.IsDisabled()
			vane.Disable(True)
		EndIf
	EndIf
	If PendingStop
		PendingStop = False
		Debug.Trace("TTC_ questline finished at stage " + GetStage() + ", state " + TTC_State.GetValueInt())
		Stop()
	EndIf
EndEvent
