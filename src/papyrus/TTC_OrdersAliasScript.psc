Scriptname TTC_OrdersAliasScript extends ReferenceAlias
{The Turncoat Contract - on the Orders alias. Reading the orders names the traitor.}

Event OnRead()
	Utility.Wait(0.1)
	(GetOwningQuest() as TTC_MainScript).OrdersRead()
EndEvent
