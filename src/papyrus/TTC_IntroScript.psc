Scriptname TTC_IntroScript extends Quest
{The Turncoat Contract - always-running quest. Holds Hjalla near the Solitude Stables and her opening topic,
and starts the questline (TTC_Main) when the player accepts.}

Quest Property TTC_Main Auto
GlobalVariable Property TTC_State Auto
{0 = not started, 1 = running, 2 = done (arrest), 3 = done (killed Vane), 4 = done (took the bribe)}
Message Property TTC_MsgNoStart Auto
ReferenceAlias Property Hjalla Auto

Function Accept()
	If TTC_State.GetValueInt() != 0 || TTC_Main.IsRunning()
		Debug.Trace("TTC_ Accept ignored: state " + TTC_State.GetValueInt())
		Return
	EndIf
	If TTC_Main.Start()
		TTC_State.SetValue(1)
		(TTC_Main as TTC_MainScript).Begin(Hjalla.GetReference())
		Debug.Trace("TTC_ questline started")
	Else
		Debug.Trace("TTC_ TTC_Main failed to start (no free dungeon for the ambush site)")
		TTC_MsgNoStart.Show()
	EndIf
EndFunction
