# Healer's Craft: Sickness & Remedies (Vintage Story 1.22)

Catch a cold in the cold and wet, then grind your own remedies by hand.
Empty template: the game logic is yours to write.

## 0.1.0 plan: "The Common Cold"

Loop: get cold → catch a cold → grind a remedy by hand → recover faster.

- **Cold (the illness):** chance to catch it while vanilla body temperature (`bodyTemp` watched attribute) stays low;
  `wetness` raises the chance. You get hungrier faster and recover stamina more slowly, and you cough now and then
  (vanilla `cough` animation + `cough` sounds below). It wears off after about 3 in-game days, sooner if you stay warm.
- **Mortar & pestle:** grid recipe (stone + stick). Catmint (vanilla flower) in the offhand, mortar in the main hand, hold right-click.
  Unlike the drop spindle it **never finishes by itself**. The moment you let go decides the result:
  coarse = weak remedy, fine = strong remedy, mush = wasted.
  The fine window moves a little each batch (dry vs fresh herb), so you learn to read the cues, not count seconds.
- **Cues, no UI:** the sound changes per stage (below), plus particles and the herb's icon in your offhand, so players with sound off can still read it.
- **Remedy:** hold right-click to take it. Strong cures the cold; weak shortens it.
- **Config:** catch chance, illness length.
- **Not in 0.1:** fevers, wounds, more herbs, steeping, XP or levels, contagion.

## Build

`./build.sh` (Git Bash) compiles `src/*.cs` with Visual Studio's Roslyn `csc.exe` against the game's `VintagestoryAPI.dll`
and the .NET 10 runtime, and writes the zip to `build/`. The game path defaults to
`%APPDATA%\Vintagestory` (override with `VS_GAME=...`).

For local testing, `./build-local.sh` builds and installs into
`%APPDATA%\VintageStoryData_MyMods\Mods` (override with `VS_MODS=...`).
The local script and generated `build/` folder are ignored by Git; `build.sh` is shared.

## Layout

- `src/HealersCraftMod.cs`: empty ModSystem; register your item classes in `Start`
- `assets/healerscraft/`: `itemtypes/`, `recipes/grid/`, `shapes/item/`, `textures/item/`, `lang/en.json`, `config/`, `sounds/`
- `CREDITS.md`: sound sources and licenses (update it when you add or replace assets)
- `LICENSE.txt`: license
