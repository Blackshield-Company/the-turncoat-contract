Scriptname TTC_TIF_Recover extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment (0.1.1): the captain had no ledger; Hjalla hands it over.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).RecoverPayroll()
EndFunction
