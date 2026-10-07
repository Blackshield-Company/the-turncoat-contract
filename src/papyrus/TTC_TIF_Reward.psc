Scriptname TTC_TIF_Reward extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: report to Hjalla.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).Reward()
EndFunction
