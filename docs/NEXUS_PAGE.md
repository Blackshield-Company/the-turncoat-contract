# Nexus Mods page: The Turncoat Contract

Early beta: v0.1.3 passed every automated build check but hasn't been playtested in-game. Exact upload-form values are in [NEXUS_FIELDS.md](NEXUS_FIELDS.md).

## Page settings

- **Game:** Skyrim Special Edition
- **Mod name:** The Turncoat Contract
- **Version:** 0.1.3 (early beta)
- **Category:** Quests and Adventures
- **Suggested tags:** Quests, Lore-Friendly, Dialogue, Choices and Consequences, Weapons, ESL, No SKSE, Scripted
- **Requirements:** none.
- **Permissions (suggested):** source on GitHub under MIT; allow modification with credit, no re-uploads of the archive.
- **File to upload:** `TurncoatContract-0.1.3.zip` (FOMOD), main file, named "The Turncoat Contract".

## Description (paste into the BBCode editor)

Same text as `nexus/ttc-description.txt` in the workspace:

```bbcode
[center][color=#e06c3c][b]EARLY RELEASE (v0.1.3)[/b]: new mod, not yet widely playtested. Please report bugs in the Bugs tab with your Papyrus log lines (grep TTC_). Source: [url=https://github.com/synthalorian/the-turncoat-contract]GitHub[/url][/color][/center]

[center][size=6][b]THE TURNCOAT CONTRACT[/b][/size]
[size=3][i]An ambushed payroll, a sergeant with a short list of people she trusts, and a traitor wearing the Blackshield.[/i][/size][/center]

[line]

[size=4][b]Summary[/b][/size]
[b]Status: Early beta (v0.1.3).[/b] Every automated build check passes, but it hasn't had wide in-game playtesting yet. It's playable now. Bug reports help shape 0.1.x.
A questline for the Blackshield mercenary company. Sergeant [b]Hjalla Iron-Vow[/b] camps by the [b]Solitude Stables[/b]: the company payroll wagon never reached Falkreath, four of hers died with it, and the raiders knew the route. Someone inside sold it.

[size=4][b]The quest[/b][/size]
[list]
[*][b]"You wear the Blackshield."[/b] Talk to Hjalla near the Solitude Stables (level 8+). Accept and she hands you a contract writ and a plain [b]Blackshield Oathblade[/b].
[*][b]The ambush site[/b] is a dungeon in Haafingar, the Reach, Hjaalmarch, Whiterun or Falkreath hold, picked when the quest starts. The raider captain who led the ambush waits at its entrance.
[*][b]The payroll ledger and the sealed orders[/b] are on the captain. Reading the orders names the traitor: [b]Corporal Edric Vane[/b], Hjalla's own man, now hiding near Falkreath.
[*][b]Back to Hjalla[/b] with the ledger: she re-edges your blade ([b]Oathblade, Edged[/b]: shock enchantment) and sends you after Vane.
[*][b]Vane[/b] tells you why he did it if you ask, then the choice is yours.
[/list]

[size=4][b]Choices and rewards[/b][/size]
[list]
[*][b]Arrest[/b] ("Surrender. You'll answer to Hjalla." then report): [b]Oathblade of Mercy[/b] (ebony, 15 damage, absorb health + shock) and [b]Hjalla's Captain Shield[/b] (fortify block, resist magic), 500 gold
[*][b]Kill[/b] ("There's only one answer for a turncoat." (or kill him any other way), then report): [b]Oathblade of Judgement[/b] (ebony, 15 damage, fire + absorb stamina), 500 gold, Vane's [b]Emerald Signet[/b] (fortify one-handed, stamina) from his body
[*][b]Take the bribe[/b] ("Make me an offer. (1500 gold)"): 1,500 gold and Vane's Emerald Signet. Hjalla won't speak to you again, and the blade stays at its second tier
[/list]

[size=4][b]Enhanced items[/b][/size]
[list]
[*]Blackshield Oathblade (3 tiers, swapped in place and re-equipped if you were holding it)
[*]Hjalla's Captain Shield
[*]Vane's Emerald Signet
[*]Contract Writ, Sealed Orders, Payroll Ledger
[/list]

[size=4][b]Part of the series[/b][/size]
Notices the other synthalorian mods when they're installed and never requires them. Each one is looked up at runtime with vanilla Papyrus (no SKSE, no masters):
[list]
[*][b]Blackshield Company[/b]: Hjalla recognizes you from the company rolls (different opening) and the final reward includes a 250 gold company bonus ([font=Courier New]TTC_CompanyBonus[/font]).
[*][b]Steel Oath[/b]: Vane greets you differently: he can see you carry an oath.
[*][b]Contract Hunter[/b]: Detected and logged ([font=Courier New]TTC_SeriesCHT[/font]); reserved for later dialogue.
[/list]

[size=4][b]Unvoiced dialogue that you can actually read[/b][/size]
The new lines are unvoiced and shown as subtitles. Every line ships with a silent voice file sized to its length, so subtitles stay on screen. [b]No Fuz Ro D'oh needed[/b]. Turn on dialogue subtitles.

[size=4][b]Requirements[/b][/size]
[list]
[*]Skyrim Special Edition or Anniversary Edition (1.5.97 or 1.6.x). Works on Steam under Proton on Linux.
[*]Nothing else. No SKSE, no SkyUI, no DLC, no Creation Club content.
[/list]

[size=4][b]Installation[/b][/size]
Install with [b]Mod Organizer 2[/b] or [b]Vortex[/b] (FOMOD installer) and enable [b]TurncoatContract.esp[/b]. It's ESL-flagged. Safe to add mid-playthrough.

[size=4][b]Compatibility[/b][/size]
[list]
[*][b]No edits to vanilla records[/b], no cells, no navmesh. Skyrim.esm is the only master.
[*]New NPCs appear at vanilla map markers; mods that rebuild those spots may leave them standing somewhere odd.
[*]Compatible with all fifteen mods of the series (same author), checked by loading every plugin together: unique FormIDs, EditorIDs, scripts, globals and files, zero overrides.
[/list]

[size=4][b]What got scaled down / known issues[/b][/size]
[list]
[*]No scenes, packages or escort: an arrested Vane fades out a few seconds after surrendering instead of being marched to Solitude.
[*]Hjalla and Vane are new NPCs spawned at vanilla map markers (Solitude Stables, Falkreath), not hand-placed in cells, so no navmesh or Creation Kit work was needed.
[*]The ambush is told through notes and a dungeon boss; there is no wrecked-wagon set piece.
[*]Not playtested in-game.
[*]Hjalla stands at the Solitude Stables map marker; mods that rebuild that area may leave her standing somewhere odd.
[*]Weapon tiers are separate records; tempering on an earlier tier is lost when it is swapped.
[*]Settings are console globals: [code]set TTC_MinLevel to 8 / set TTC_RewardGold to 500 / set TTC_BribeGold to 1500 / set TTC_CompanyBonus to 250[/code]
[/list]

[size=4][b]Uninstalling[/b][/size]
As with any scripted mod, finish the quest before removing it.

[size=4][b]Source[/b][/size]
Open source (MIT): [url=https://github.com/synthalorian/the-turncoat-contract]github.com/synthalorian/the-turncoat-contract[/url]

[size=4][b]Credits[/b][/size]
[list]
[*][url=https://github.com/Mutagen-Modding/Mutagen]Mutagen[/url] by Noggog and contributors.
[*][url=https://github.com/russo-2025/papyrus-compiler]papyrus-compiler[/url] by russo-2025.
[*][url=https://github.com/GandaG/fomod-schema]FOMOD schema[/url] by GandaG.
[*]Bethesda Game Studios for Skyrim.
[/list]

```
