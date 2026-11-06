# RankedCrossplay

**Play Gorilla Tag ranked on PC against Quest players, at your real rank.**

On PC, the ranked queue only looks for other PC players, and that pool is often empty, so you wait and never get a match. RankedCrossplay sends your ranked search to the Quest player pool, where the players are. You keep your own rank, get matched at your own level, and rank up normally.

## What it does

- Searches for ranked matches in the **Quest** pool, not the PC pool.
- Uses your **real** rank tier. Nothing about your rank, rating or match scores is faked or changed.
- Writes a short line to the log for each ranked search, and when the ranked server accepts or refuses your join.
- Can be turned off in its settings file, with no need to remove it.

That's all. The only thing it changes is the platform name the ranked search uses (`Quest` instead of `PC`).

## Requirements

- Gorilla Tag on PC (Steam)
- [BepInEx 5](https://github.com/BepInEx/BepInEx/releases)

## Install

1. Install BepInEx 5 in your Gorilla Tag folder, and start the game once so it creates its folders.
2. Download `RankedCrossplay.dll` from the [Releases](../../releases) page.
3. Put it in `Gorilla Tag\BepInEx\plugins`.
4. Remove any other mod that changes the ranked queue. Two of them patching the same search will conflict.

## How to use

Walk into the ranked queue the way you normally do. That's it.

To check that it's working, open `Gorilla Tag\BepInEx\LogOutput.log`. You should see:

```
Ranked search: tier Medium, Quest pool
Ranked match join accepted by the server
```

The first line appears when you queue. The tier is your own. The second appears once you're in a match.

## Settings

The file `BepInEx\config\dev.itsreallyhex.rankedcrossplay.cfg` is created the first time you launch the game with the mod.

| Setting | Default | What it does |
|---------|---------|--------------|
| `JoinQuestLobbies` | `true` | `true`: search the Quest pool. `false`: the normal PC search. |

## FAQ

**I'm matched in lobbies below my rank.**
Your rank loads a few seconds after you spawn. Until then the game treats you as the lowest tier. Wait a moment after spawning before you join the queue.

**The log says `refused`.**
The ranked server turned down the join, and the game left the match. Queue again. If it keeps happening, set `JoinQuestLobbies` to `false` and play the normal PC queue.

## Disclaimer

This is not an official feature. It changes what the game tells the matchmaker about your platform, and the developers may consider that against their rules. You use it at your own risk, and any action taken against your account is on you.

## License

[MIT](LICENSE)
