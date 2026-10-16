# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

## [0.1.4] - 2026-10-16

Save-compatible with 0.1.3 (no records added or removed, no form IDs changed).

### Fixed
- Corporal Vane's bribe line showed the raw tag `<Global=TTC_BribeGold>` instead of the amount. Skyrim does not expand `<Global=...>` tags in NPC subtitles, so the amount now appears in your reply instead ("Make me an offer. (1500 gold)"), where the game fills it in from `TTC_BribeGold`.

### Changed
- The generator now adds every global named by a `<Global=...>` tag in quest text, player dialogue or message boxes to the owning quest's Text Display Globals automatically (`TextGlobals.Register` in modkit).
- The build check now fails when a `<Global=...>` tag is in NPC response text, names a global that does not exist, has no owning quest, or names a global missing from that quest's Text Display Globals (dialogue topics, prompts and message boxes are now covered as well as journal and objectives).

## [0.1.3] - 2026-10-14

Still early beta, not yet playtested in game. Save-compatible with 0.1.2 (no records added or removed, no form IDs changed).

### Fixed
- Hjalla's status line after you accept, and the contract writ, could show a raw location tag (`<Alias=AmbushLoc>`). The status line no longer uses a tag (the site is still named in the journal and objective), and the writ now uses the quest's stored text.

### Changed
- The build check now fails on any `<Alias=...>` tag in dialogue (responses, prompts, topic text) or quest text (name, journal, objectives) that is not a filled, non-optional alias of the quest that owns the text, and on alias-created notes whose tags would not resolve.

## [0.1.2] - 2026-10-13

Still early beta, not yet playtested in game.

### Fixed
- The quest giver's accept line showed a raw location alias.

### Added
- A build check for cross-quest alias tokens.

## [0.1.1] - 2026-10-11

Still early beta, not yet playtested in game.

### Fixed
- The payroll ledger could be missing from the raider captain, so the quest never moved past "find the ambush". The quest now gives the ledger and the sealed orders to the captain by script when it starts, and checks again when he loads and when he dies. If you already carry the ledger, the quest catches up.
- Picking up the sealed orders when the captain is dead and the ledger is gone now gives you the ledger.
- Stuck saves (0.1.0): with the sealed orders in your inventory, tell Hjalla "The captain didn't have your ledger." She hands it over and the quest continues.

### Changed
- The raider captain now waits at the ambush site's boss spot instead of the outdoor map marker (sites without one keep the marker).

## [0.1.0] - 2026-10-07

Early beta. First public release. It passes every automated check in the build (Papyrus compile, plugin and dialogue validation, round trip, voice file and SEQ checks, FOMOD schema, 15-plugin series compatibility check) but has not been playtested in-game yet.

### Added
- "You wear the Blackshield." Talk to Hjalla near the Solitude Stables (level 8+). Accept and she hands you a contract writ and a plain Blackshield Oathblade.
- The ambush site is a dungeon in Haafingar, the Reach, Hjaalmarch, Whiterun or Falkreath hold, picked when the quest starts. The raider captain who led the ambush waits at its entrance.
- The payroll ledger and the sealed orders are on the captain. Reading the orders names the traitor: Corporal Edric Vane, Hjalla's own man, now hiding near Falkreath.
- Back to Hjalla with the ledger: she re-edges your blade (Oathblade, Edged: shock enchantment) and sends you after Vane.
- Vane tells you why he did it if you ask, then the choice is yours.
- Choice: Arrest: Oathblade of Mercy (ebony, 15 damage, absorb health + shock) and Hjalla's Captain Shield (fortify block, resist magic), 500 gold.
- Choice: Kill: Oathblade of Judgement (ebony, 15 damage, fire + absorb stamina), 500 gold, Vane's Emerald Signet (fortify one-handed, stamina) from his body.
- Choice: Take the bribe: 1,500 gold and Vane's Emerald Signet. Hjalla won't speak to you again, and the blade stays at its second tier.
- Soft integration with Blackshield Company (runtime lookup, not a master).
- Soft integration with Steel Oath (runtime lookup, not a master).
- Soft integration with Contract Hunter (runtime lookup, not a master).
- Unvoiced dialogue with 28 silent voice files; `SEQ/TurncoatContract.seq` for the start-game-enabled dialogue quest.
- FOMOD installer with optional Papyrus sources.
