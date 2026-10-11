Scriptname TTC_CaptainAliasScript extends ReferenceAlias
{The Turncoat Contract - on the raider captain. 0.1.1: re-checks that he carries the payroll ledger and the orders.}

Event OnLoad()
	(GetOwningQuest() as TTC_MainScript).EnsureItems()
EndEvent

Event OnDeath(Actor akKiller)
	(GetOwningQuest() as TTC_MainScript).EnsureItems()
EndEvent
