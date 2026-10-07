Scriptname TTC_SeriesScript extends ReferenceAlias
{The Turncoat Contract - soft integration with the rest of the series, on the player alias of TTC_Intro.
Nothing here is a master: each plugin is looked up at runtime with Game.GetFormFromFile (vanilla Papyrus,
no SKSE). When a plugin is not installed the call returns None and the matching global stays 0.}

GlobalVariable Property TTC_SeriesBSC Auto
{0 = Blackshield Company not installed, otherwise BSC_Rank + 1}
GlobalVariable Property TTC_SeriesSOT Auto
GlobalVariable Property TTC_SeriesCHT Auto

Event OnInit()
	Refresh()
EndEvent

Event OnPlayerLoadGame()
	Refresh()
EndEvent

Function Refresh()
	GlobalVariable bscRank = Game.GetFormFromFile(0x805, "BlackshieldCompany.esp") as GlobalVariable ; soft: BSC_Rank
	If bscRank
		TTC_SeriesBSC.SetValue(bscRank.GetValue() + 1.0)
	Else
		TTC_SeriesBSC.SetValue(0.0)
	EndIf
	GlobalVariable sotActive = Game.GetFormFromFile(0x80C, "SteelOath.esp") as GlobalVariable ; soft: SOT_ActiveCount
	If sotActive
		TTC_SeriesSOT.SetValue(1.0)
	Else
		TTC_SeriesSOT.SetValue(0.0)
	EndIf
	GlobalVariable chtOpen = Game.GetFormFromFile(0x81B, "ContractHunter.esp") as GlobalVariable ; soft: CHT_OpenCount
	If chtOpen
		TTC_SeriesCHT.SetValue(1.0)
	Else
		TTC_SeriesCHT.SetValue(0.0)
	EndIf
	Debug.Trace("TTC_ series check: BSC " + TTC_SeriesBSC.GetValueInt() + ", SOT " + TTC_SeriesSOT.GetValueInt() + ", CHT " + TTC_SeriesCHT.GetValueInt())
EndFunction
