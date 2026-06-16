# CLAUDE.md
<!-- Funplay Unity MCP managed project skills -->

# Funplay Unity MCP Project Guidance

This file is managed by Funplay MCP for Unity for Claude Code.

## Installed skills

- `unity-mcp-workflow` - Efficient workflow for using Unity MCP to edit, import, compile, inspect, and test Unity projects.

## Preferred workflow

- Use Funplay MCP tools for Unity editor state and automation.
- Use `execute_code` for non-trivial Unity orchestration. For new snippets, implement `IFunplayCommand` and use `ctx.RegisterObjectCreation` / `RegisterObjectModification` / `DestroyObject` so changes participate in Undo and `ctx.Log` for traceable output.
- Inspect Unity objects through MCP before changing user-named scene or prefab targets. Carry the returned `instanceId` into follow-up calls (`find_method=by_id`) instead of re-resolving by name.
- Tool returns are structured JSON (`{success, message, data}` / `{success: false, code, error, data}`). Branch on `code`, not free-form text.
- Set component fields with `set_component_property(ies)` — it picks up `[SerializeField] private` fields and accepts Object references as `{"fileID": <instanceId>}` or `{"assetPath": "Assets/..."}`.
- Read editor state through `get_selection`, `get_prefab_stage`, `get_tags`, `get_layers`, `get_build_settings`; try `execute_menu_item` before writing ad-hoc `execute_code`.
- Save only the scene or prefab assets intentionally modified, then read back exact values.
- With default `core` exposure, use the focused workflow tools. With default `full` exposure, prefer specific MCP tools for simple editor operations.
- `execute_code` refreshes assets and waits for compilation before running. For other tools that depend on freshly compiled code, still call `request_recompile` after external script edits.
- `request_recompile` is rejected while Unity is in Play Mode. Call `exit_play_mode` first, then retry.
- After `enter_play_mode`, the HTTP server briefly drops while Unity reloads the domain. Poll `tools/list` or `get_reload_recovery_status` until it responds again before issuing the next tool call.
- If domain reload interrupts a request, follow with `get_reload_recovery_status`.
- Additional installed skills are available under `.claude/skills/`.

## Project

- Project root: `D:\mySpace\project\games\rola`
- Product name: `rola`

---

## Current Project Status

- **Unity version:** 2022.3.62f3c1 (2D URP project)
- **Playable scene:** `TestArena` is fully functional
- **Core systems:** Movement, combat, roguelike loop, hit-stop, camera shake, knockback all working
- **Code:** 49 scripts across 12 categories, all compile with no errors
- **Prefabs:** `Player.prefab` and `Enemy.prefab` created and configured
- **Data asset:** `Class_Warrior.asset` created with base stats
- **Visuals:** All sprites are placeholder white/blue/red squares
- **Audio:** No clips assigned (AudioManager creates an empty GameObject)
- **UI:** No Canvas or HealthBar in scene (scripts exist, no GameObjects)
- **Animation:** No AnimatorControllers assigned to Player or Enemy

### What Works Right Now

Enter Play Mode in `TestArena` and you can:
- Move with A/D, jump with Space, dash with LeftShift
- Attack with Fire1 (mouse left / Ctrl), combo up to 3 hits
- Air attack with Fire1 while airborne, air slam with Fire2 (mouse right / Alt)
- Enemy patrols, chases when player is in sight, attacks when in range
- Combat includes hit-stop, camera shake, knockback, and damage numbers

---

## Working Without MCP

If MCP is unavailable, open the Unity Editor directly at `D:\mySpace\project\games\rola` and work manually.

### Scene edits
- Drag and drop objects in the Hierarchy window
- Select objects and configure components in the Inspector
- Use the Scene view to position objects visually

### Prefab edits
- Double-click a prefab in the Project window to enter Prefab Mode
- Make changes, then save with Ctrl+S
- All instances in scenes update automatically

### Script edits
- Modify `.cs` files in any IDE (VS Code, Rider, Visual Studio)
- Unity auto-detects changes and recompiles
- Check the Console window for compilation errors

### Creating ScriptableObjects manually
1. In the Project window, right-click the target folder (e.g., `Assets/Data/`)
2. Choose `Create > Rola > [type]` (e.g., `Character Appearance Data`, `Player Class Data`)
3. Name the asset and configure fields in the Inspector

### Importing sprites
1. Copy PNG files into `Assets/Sprites/`
2. Select the imported texture in the Project window
3. In the Inspector, set **Texture Type** to `Sprite (2D and UI)`
4. Set **Pixels Per Unit** to 32 or 64 (pixel art) or 100 (high-res)
5. Set **Filter Mode** to `Point (no filter)` for pixel art
6. Click **Apply**

### Testing without MCP
- Press the Play button in the Unity Editor
- Use the Game view to test gameplay
- Check the Console for runtime logs and errors
- No MCP tools are required for any of this

---

## Asset Pipeline for Artists

### Key directories
| Directory | Purpose |
|-----------|---------|
| `Assets/Sprites/` | All sprite textures (player, enemy, VFX, UI, tileset) |
| `Assets/Data/` | ScriptableObject data assets (classes, appearances, parts) |
| `Assets/Prefabs/` | Reusable GameObject prefabs (Player, Enemy, effects, UI) |
| `Assets/Scripts/` | All C# source code |
| `Assets/Scenes/` | Scene files (TestArena, future menus/levels) |
| `Assets/Animation/` | AnimatorControllers and AnimationClips (to be created) |
| `Assets/Audio/` | Audio clips and AudioMixer (to be created) |

### Replacing placeholder sprites on a prefab
1. Open the prefab (double-click `Assets/Prefabs/Player.prefab`)
2. Select the child object with the SpriteRenderer (e.g., `Visuals`)
3. In the Inspector, find the **SpriteRenderer** component
4. Drag your new sprite from the Project window into the **Sprite** field
5. Save the prefab (Ctrl+S)

### Creating a character appearance data asset
1. Right-click in `Assets/Data/`
2. `Create > Rola > Character Appearance Data`
3. Name it (e.g., `Appearance_Warrior`)
4. In the Inspector, assign sprites to each body part slot:
   - `bodySprite` is the base (required)
   - Other parts overlay on top with sorting orders:
     - BackHair = 0, Body = 1, Bottom = 2, Top = 3, Shoes = 4, Gloves = 5, Weapon = 6, FrontHair = 7, Accessory = 8
5. Set `skinColor`, `hairColor`, `eyeColor` as desired
6. For a simple start: create one full-body sprite, assign it to `bodySprite`, leave other parts null

### Assigning appearance to a class
1. Select `Assets/Data/Class_Warrior.asset`
2. Drag `Appearance_Warrior` into the **Default Appearance** field
3. Save the project (Ctrl+S)

---

## Known Gaps / Placeholders

The following areas need art and design work. All gameplay code is complete.

| Area | Status | What is needed |
|------|--------|----------------|
| **Sprites** | All placeholder | Player, enemy, ground, platform, VFX, UI sprites |
| **Animation** | No controllers | AnimatorControllers for Player and Enemy with all states |
| **Audio** | No clips | Jump, attack, dash, hurt, enemy SFX, background music |
| **UI** | No GameObjects | Canvas, HealthBar, PauseMenu, MainMenu scene |
| **Class data** | Base stats only | Starting skills, equipment, default appearance for Warrior |
| **Additional classes** | None | Mage, Archer, etc. (create new PlayerClassData assets) |
| **Levels** | One test scene | Design additional arena sections, add checkpoints/death zones |
| **Lighting / Post-processing** | Default | 2D lighting, post-processing volume for visual polish |

---

## Next Steps for Art / Assets (No MCP Required)

### Phase 1: Create sprite assets
1. **Player character sprite sheet:** Idle (1+ frames), Run (6+), Jump (1+), Fall (1+), Attack1/2/3 (2+ each), AirAttack1/2 (2+ each), AirSlam (2+), Dash (2+), Hurt (1+), Dead (1+)
2. **Enemy sprite sheet:** Idle, Patrol/Run, Attack (2+), Hurt, Dead
3. **Ground/platform tileset:** Reusable ground pieces
4. **VFX sprites:** Slash effect (white slash, transparent), hit spark, death dust
5. **UI sprites:** Health bar frame, skill icon placeholders (4), class icon for Warrior

### Phase 2: Animation
1. Create `PlayerAnimatorController` in `Assets/Animation/`
   - States: Idle, Run, Jump, Fall, Attack1, Attack2, Attack3, AirAttack1, AirAttack2, AirSlam, Dash, Hurt, Dead
   - Parameters: Speed (float), IsGrounded (bool), VerticalVelocity (float), IsAttacking (bool), ComboIndex (int), IsDashing (bool), IsHurt (bool), IsDead (bool), State (int)
2. Create `EnemyAnimatorController`
   - States: Idle, Patrol, Chase, Attack, Hurt, Dead
3. Assign controllers to prefabs: open Player/Enemy prefabs, add Animator component, set Controller field

### Phase 3: Audio
1. Import or create audio clips: jump, attack (3 variants), dash, hurt, enemy attack, enemy hurt, enemy death
2. Place clips in `Assets/Audio/`
3. Select `AudioManager` in scene (or create one if missing) and assign clips to public fields
4. Assign clips to PlayerController and EnemyController audio fields

### Phase 4: UI
1. Create Canvas in TestArena: `GameObject > UI > Canvas`
2. Add HealthBar: create a UI slider or image-based bar, attach `HealthBar` script, wire `PlayerStats.OnHPChanged` event
3. Create PauseMenu panel with resume/quit buttons, attach `PauseMenu` script
4. Create `MainMenu` scene with Start/Quit buttons

### Phase 5: Content
1. Create `Appearance_Warrior` CharacterAppearanceData (see Asset Pipeline above)
2. Assign it to `Class_Warrior.defaultAppearance`
3. Create starting skills and equipment ScriptableObjects for Warrior
4. Create additional classes (Mage, Archer, etc.) as new PlayerClassData assets

### Phase 6: Polish
1. Tune camera follow smoothing in `CameraFollow` component on `Main Camera`
2. Tune hit-stop durations in `PlayerController` attack data arrays
3. Add particle systems to HitEffect/DeathEffect prefabs
4. Add 2D lighting and post-processing volume
5. Design additional TestArena sections with platforms and hazards

---

## Key File Locations

| File / Folder | Purpose |
|---------------|---------|
| `Assets/Scenes/TestArena.unity` | Main playable test scene |
| `Assets/Prefabs/Player.prefab` | Player character prefab |
| `Assets/Prefabs/Enemy.prefab` | Enemy character prefab |
| `Assets/Data/Class_Warrior.asset` | Warrior class data (stats, skills, equipment, appearance) |
| `Assets/Scripts/` | Player movement, combat, input, stats, build |
| `Assets/Scripts/Core/` | CharacterStats, PlayerBuild, SkillCooldown, StatModifier |
| `Assets/Scripts/Data/` | Data definitions (PlayerClassData, CharacterAppearanceData, etc.) |
| `Assets/Scripts/Equipment/` | Equipment system base + effect types |
| `Assets/Scripts/Player/Visual/` | Sprite rendering, animation bridge, visual feedback |
| `Assets/Scripts/Roguelike/` | RunManager, RunData, UpgradeManager |
| `Assets/Scripts/` (root) | Enemy, camera, UI, narration, audio, level scripts |
| `Assets/Sprites/` | Sprite textures (currently `PlaceholderWhite.png`) |
| `Assets/Animation/` | AnimatorControllers (to be created) |
| `Assets/Audio/` | Audio clips (to be created) |

---

## Quick Reference: Testing the Game

1. Open Unity Editor at `D:\mySpace\project\games\rola`
2. Open `Assets/Scenes/TestArena.unity` (if not already open)
3. Press the Play button (or Ctrl+P)
4. In the Game view:
   - A/D or Left/Right Arrow: move
   - Space: jump
   - LeftShift: dash
   - Mouse Left / Ctrl: attack (ground combo or air attack)
   - Mouse Right / Alt: air slam (while airborne)
5. Press Escape for pause menu (once UI is added)
6. Press Ctrl+P to stop Play Mode

---

## Additional Documentation

- `ROADMAP.md` — Phased task list for art, animation, audio, UI, content, and polish
- `ASSETS_GUIDE.md` — Step-by-step guide for creating and importing art assets
- `SCRIPTS_REFERENCE.md` — Quick reference for all 49 scripts (purpose, key fields, artist notes)
