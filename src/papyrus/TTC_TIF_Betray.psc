Scriptname TTC_TIF_Betray extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: take Vane's bribe.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).Betray()
EndFunction
