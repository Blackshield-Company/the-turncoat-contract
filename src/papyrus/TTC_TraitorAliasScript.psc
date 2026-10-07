Scriptname TTC_TraitorAliasScript extends ReferenceAlias
{The Turncoat Contract - on the Traitor alias.}

Event OnDeath(Actor akKiller)
	(GetOwningQuest() as TTC_MainScript).TraitorDied()
EndEvent
