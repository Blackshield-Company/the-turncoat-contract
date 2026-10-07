Scriptname TTC_PayrollAliasScript extends ReferenceAlias
{The Turncoat Contract - on the Payroll alias.}

Event OnContainerChanged(ObjectReference akNewContainer, ObjectReference akOldContainer)
	If akNewContainer == Game.GetPlayer()
		(GetOwningQuest() as TTC_MainScript).PayrollTaken()
	EndIf
EndEvent
