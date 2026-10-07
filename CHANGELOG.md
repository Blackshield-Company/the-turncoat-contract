# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

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
