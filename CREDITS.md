# Credits

## Sounds included in this mod

| Files | Source |
|---|---|
| `sounds/effect/grind-coarse1.ogg` | https://freesound.org/people/Alex_hears_things/sounds/316688/ |

## Vanilla sounds referenced (not included)

The mod plays some base-game sounds by asset location (`game:sounds/...`). They are **not copied or redistributed**:
they load from the player's own Vintage Story install. They belong to Anego Studios.

| Asset | Used for |
|---|---|
| `game:sounds/player/drink1`, `drink2` | taking the remedy |
| `game:sounds/player/collect1-3` | remedy made |

Update this table to match what the code actually ends up using.

## Tools

- Sound generation: Python with [NumPy](https://numpy.org) and [soundfile](https://github.com/bastibe/python-soundfile)
  (libsndfile, Ogg Vorbis encoding). These are only used to create the files and don't ship with the mod.
- Game: [Vintage Story](https://www.vintagestory.at) by Anego Studios.
