# In-game test checklist

The build verifies the plugin, scripts, dialogue structure, voice files and SEQ automatically; none of that proves the quest plays well. Turn on **dialogue subtitles** and Papyrus logging:

```ini
; Skyrim.ini (or SkyrimCustom.ini), [Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

Every log line from this mod starts with `TTC_`. On each load you should see `TTC_ series check: ...` with the detected series mods. `help TTC_ 4` lists the records (ESL-flagged, FormIDs look like `FExxx8xx`).

## Setup

- [ ] `player.setlevel 10`, then travel to the Solitude Stables: Hjalla stands at its map marker. `help TTC_ 4` lists the records.

## Opening

- [ ] "You wear the Blackshield." is offered; her lines stay on screen (silent voice files).
- [ ] With Blackshield Company installed the opening changes ("I know your face from the company rolls").
- [ ] Accept: Contract Writ + Blackshield Oathblade added, journal stage 10, log `TTC_ questline started`.

## Questline

- [ ] Raider captain at the ambush dungeon entrance; loot gives the Payroll Ledger (stage 20) and Sealed Orders.
- [ ] Read the orders (before or after taking the ledger): stage 25, Vane enabled near Falkreath.
- [ ] Hjalla: "I have the payroll ledger, and a name." removes the ledger, swaps the blade for Oathblade, Edged (re-equipped if held), stage 30.

## Endings (save at stage 30)

- [ ] Arrest: Vane fades out, stage 50; report: Oathblade of Mercy + shield + gold, quest completes.
- [ ] Fight: Vane turns hostile; on death stage 60; report: Oathblade of Judgement + gold.
- [ ] Kill Vane before returning the ledger: report still takes the ledger and gives both upgrades in order.
- [ ] Bribe: 1,500 gold + ring, quest completes, Hjalla now only has a cold line.

## Series integration

- [ ] Without any other series mod: the plain opening is offered, the log shows all integration values 0, nothing errors.
- [ ] With each listed mod installed, its variant appears (see README, Series integration).

## What to report

Your game version, load order, the `TTC_` lines from Papyrus.0.log, and which step went wrong.
