Scriptname TTC_TIF_Accept extends TopicInfo Hidden
{The Turncoat Contract - dialogue fragment: accept the contract.}

Function Fragment_0(ObjectReference akSpeakerRef)
	(GetOwningQuest() as TTC_IntroScript).Accept()
EndFunction
