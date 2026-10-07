Scriptname TTC_TIF_Fight extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: fight Vane.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_MainScript).Fight()
EndFunction
