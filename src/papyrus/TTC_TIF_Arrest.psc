Scriptname TTC_TIF_Arrest extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: Vane surrenders.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).Arrest()
EndFunction
