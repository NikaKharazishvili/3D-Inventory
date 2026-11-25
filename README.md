# 3D Inventory

A first-person Unity project exploring a fully three-dimensional inventory: items are real physical objects you drag through the world, not icons in a UI panel.

The inventory chest lives inside the player camera, creating the illusion of a fixed UI while keeping everything in 3D space. Items have weight, fall with physics, and make different sounds depending on what they are.

[Download on itch.io](https://nikson212.itch.io/3d-inventory) · [Watch on YouTube](https://www.youtube.com/watch?v=nrxqgaaZu5o)

[![Watch on YouTube](https://img.youtube.com/vi/nrxqgaaZu5o/maxresdefault.jpg)](https://www.youtube.com/watch?v=nrxqgaaZu5o)

## Features
- **Physical items**: grab, push, pull, rotate and throw items. Heavier items fall harder and play their own impact sound.
- **3D chest inventory**: 16 slots laid out in a grid, with a live item count and total weight.
- **Auto-sorting slots**: removing an item shifts the rest forward to fill the gap.
- **Icon preview**: hold U to see the stored items as 2D icons.
- **Item spawner**: random items drop into the scene up to a set limit.
- **Custom cursor**: a UI image that follows the mouse while the chest is open and sits at the screen center as a crosshair otherwise. It replaces the system cursor.
- **In-world log**: short messages that explain what just happened.

## Controls
| Input | Action |
| --- | --- |
| WASD | Move |
| LMB (hold) | Grab and move an item |
| LMB + Mouse wheel | Push or pull the held item |
| LMB + RMB | Rotate the held item |
| E | Store the held or aimed item in the chest |
| I | Open / close the chest |
| U (hold) | Preview the inventory icons |
| G | Throw the held item, or drop everything while the chest is open |
| Click an item in the open chest | Drop it back into the world |

## Project structure
```
Assets/
  Animations/  Chest and player animator controllers and clips
  Art/         Icons (sprite atlas), materials, 3D models and textures
  Audio/SFX/   Chest, item and footstep sounds (item sounds are mono, played in 3D)
  Data/Items/  ScriptableItem assets (name, weight, sound, icon)
  Prefabs/     Item prefabs
  Scenes/      Main
  Scripts/
    Core/        GameConstants (tuning values), Utils (shared helpers), GlobalReferences (scene hub)
    Data/        ScriptableItem
    Gameplay/    Player, Item, ItemSpawner
    Managers/    Inventory
    UI/          LogMessage, GameCursor
  ThirdParty/  Farland Skies (skybox)
```

## Technical notes
- All input is handled in `Player`. `Item` owns only its physics state (`Free`, `Held`, `Stored`), and `Inventory` owns the slots, weight, icons and chest animation.
- `GlobalReferences` is the only singleton. Scripts get their scene references from it, so there are no `Find` calls anywhere.
- Item types are `ScriptableItem` assets, so adding an item needs a prefab and an asset but no code changes.
- Tuning values are centralized in `GameConstants`.
- Release builds rename private methods and fields after compilation (`CodeObfuscator`). Use `[DontObfuscate]` to exclude a member.

## Requirements
Unity 6000.3.12f1. Open the project, load `Assets/Scenes/Main.unity` and press Play.

## Credits
Skybox: Farland Skies (Nebula One), included under `Assets/ThirdParty`.

## License
Copyright (c) Nikson. All rights reserved.

This repository is provided for viewing and evaluation only. No part of it, including code, art, audio and other assets, may be copied, modified, redistributed or used in other projects without explicit written permission. Third-party assets remain the property of their respective owners.
