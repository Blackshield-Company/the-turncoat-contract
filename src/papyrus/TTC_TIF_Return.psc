Scriptname TTC_TIF_Return extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: return the payroll ledger.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).ReturnPayroll()
EndFunction
