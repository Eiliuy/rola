# Rola Project Roadmap

**Project:** Rola (2D Action Roguelike)  
**Unity Version:** 2022.3.62f3c1 (2D URP)  
**Last Updated:** 2026-06-17  
**Status:** Core gameplay complete, all art is placeholder

---

## Current State

TestArena scene is fully playable. All core systems are implemented and functional:

- **Player:** Move (A/D), Jump (Space), Attack (Fire1), Air Slam (Fire2), Dash (Shift)
- **Enemy:** Patrol, Chase, Attack AI with sight/attack range
- **Combat:** Hit-stop, camera shake, knockback, damage numbers
- **Roguelike Loop:** RunManager, PlayerStats, PlayerBuild, Class system
- **Code:** 49 scripts, all compile, no errors
- **Visuals:** All placeholders (white/blue/red sprites)
- **Audio:** None assigned
- **UI:** None in scene
- **Animation:** No AnimatorControllers assigned

---

## Phase 1: Art Assets (Immediate Priority)

**Goal:** Replace all placeholder sprites with real art. No MCP needed.

**What this unblocks:** Animation, UI, and polish phases.

- [ ] **Player Character Sprite Sheet**
  - Required states: Idle (1+ frames), Run (6+ frames), Jump (1+), Fall (1+), Attack1/2/3 (2+ each), AirAttack1/2 (2+ each), AirSlam (2+), Dash (2+), Hurt (1+), Dead (1+)
  - Import to: `Assets/Sprites/Player/`
  - Settings: Texture Type = Sprite (2D and UI), PPU = 32 or 64, Filter Mode = Point, Compression = None
  - See `ASSETS_GUIDE.md` for body part system details (9 sortable slots)

- [ ] **Enemy Sprite Sheet**
  - Required states: Idle, Patrol/Run, Attack (2+ frames), Hurt, Dead
  - Import to: `Assets/Sprites/Enemy/`
  - Same import settings as player

- [ ] **Ground / Platform Tileset**
  - Ground_Main, Ground_Left, Ground_Right sprites
  - Import to: `Assets/Sprites/Environment/`
  - Replace on scene objects: Ground_Main, Ground_Left, Ground_Right

- [ ] **VFX Sprites**
  - SlashEffect: white slash shape, transparent background
  - HitEffect: spark/blood particles
  - DeathEffect: dust/particles
  - Import to: `Assets/Sprites/VFX/`

- [ ] **UI Sprites**
  - Health bar frame / fill bar
  - Skill icon placeholders (4 slots)
  - Class icon for Warrior
  - Import to: `Assets/Sprites/UI/`

---

## Phase 2: Animation

**Goal:** Create AnimatorControllers and wire them to Player/Enemy.  
**Prerequisite:** Phase 1 complete (sprites imported).

**What this unblocks:** Visual feedback for all actions, state-based transitions.

- [ ] **Create `PlayerAnimatorController`**
  - States: Idle, Run, Jump, Fall, Attack1, Attack2, Attack3, AirAttack1, AirAttack2, AirSlam, Dash, Hurt, Dead
  - Parameters: Speed (float), IsGrounded (bool), VerticalVelocity (float), IsAttacking (bool), ComboIndex (int), IsDashing (bool), IsHurt (bool), IsDead (bool), State (int)
  - Assign to Player.prefab -> Visuals -> Animator component

- [ ] **Create `EnemyAnimatorController`**
  - States: Idle, Patrol, Chase, Attack, Hurt, Dead
  - Parameters: Speed (float), IsGrounded (bool), IsAttacking (bool), IsHurt (bool), IsDead (bool), State (int)
  - Assign to Enemy.prefab -> Visuals -> Animator component

- [ ] **Verify animation events**
  - Attack animation should fire damage at correct frame (check `AttackData` on PlayerController/EnemyController)
  - Hurt/Dead should transition properly

---

## Phase 3: Audio

**Goal:** Add sound effects for all gameplay actions.  
**Prerequisite:** None (can be done in parallel with art).

**What this unblocks:** Game feel and feedback.

- [ ] **Import AudioClips**
  - Jump, Attack (3 variants), Dash, Hurt (player), Hurt (enemy), Enemy Attack, Enemy Death
  - Import to: `Assets/Audio/SFX/`
  - Format: Short WAV or compressed OGG, stereo or mono

- [ ] **Configure AudioManager**
  - Currently an empty GameObject created by `GameInitializer`
  - Add AudioSource components or configure `AudioManager` script fields
  - Assign clips to SFX slots

- [ ] **Assign clips to controllers**
  - PlayerController: jumpSFX, attackSFX, dashSFX, hurtSFX
  - EnemyController: attackSFX, hurtSFX, deathSFX

---

## Phase 4: UI

**Goal:** Create in-game UI and menu scenes.  
**Prerequisite:** Phase 1 (UI sprites) and Phase 3 (optional, for menu SFX).

**What this unblocks:** Playable user experience, health feedback, pause/menu flow.

- [ ] **Create Canvas in TestArena scene**
  - GameObject -> UI -> Canvas (Screen Space - Overlay)
  - Add CanvasScaler (reference resolution 1920x1080)
  - Add GraphicRaycaster

- [ ] **Create HealthBar**
  - Create GameObject under Canvas
  - Add `HealthBar` script (`Assets/Scripts/HealthBar.cs`)
  - Wire to `PlayerStats.OnHPChanged` event
  - Assign health bar frame and fill sprites

- [ ] **Create PauseMenu**
  - Create UI panel with Resume, Restart, Quit buttons
  - Add `PauseMenu` script
  - Hook to Escape key input

- [ ] **Create MainMenu scene**
  - New scene: `Assets/Scenes/MainMenu.unity`
  - Add title, Start Game, Settings, Quit buttons
  - Add `MainMenu` script
  - Configure Build Settings to include MainMenu scene

---

## Phase 5: Content

**Goal:** Fill data assets with real content and create additional classes.  
**Prerequisite:** Phase 1 (appearance sprites).

**What this unblocks:** Class selection, meaningful run progression, replayability.

- [ ] **Create `Appearance_Warrior` CharacterAppearanceData**
  - Right-click in Project -> Create -> Rola -> Character Appearance Data
  - Assign body part sprites (or single full-body sprite to `bodySprite`)
  - Set skinColor, hairColor, eyeColor
  - Save to `Assets/Data/Appearance_Warrior.asset`

- [ ] **Assign appearance to Class_Warrior**
  - Select `Assets/Data/Class_Warrior.asset`
  - Drag `Appearance_Warrior` into `Default Appearance` field

- [ ] **Create starting skills for Warrior**
  - Create SkillData ScriptableObjects (Assets -> Create -> Rola -> Skill Data)
  - Assign to `Class_Warrior.startingSkills` array

- [ ] **Create starting equipment for Warrior**
  - Create EquipmentData ScriptableObjects
  - Assign to `Class_Warrior.startingEquipment` array

- [ ] **Create additional classes**
  - Mage, Archer, etc.
  - Create appearance data, skills, equipment for each
  - Add to `RunManager` or class selection UI

---

## Phase 6: Polish

**Goal:** Tune feel, add juice, and expand content.  
**Prerequisite:** All prior phases.

**What this unblocks:** Shippable quality.

- [ ] **Camera Tuning**
  - Adjust `CameraFollow` smoothing, offset, and bounds on `Main Camera`
  - Test with fast movement and combat

- [ ] **Hit-Stop Tuning**
  - Adjust `HitStopManager` durations per attack type in `AttackData`
  - Ensure it feels impactful but not disruptive

- [ ] **Particle Effects**
  - Add ParticleSystem components to HitEffect/DeathEffect prefabs
  - Configure emission, color, lifetime

- [ ] **2D Lighting & Post-Processing**
  - Add 2D lights to scene (Global light, player point light)
  - Configure URP 2D Renderer post-processing if needed

- [ ] **Level Design**
  - Expand TestArena with more platforms, enemy spawn points
  - Add checkpoint objects
  - Add death zone (fall off screen)
  - Design additional scenes/levels

- [ ] **Build & Test**
  - Configure Build Settings scenes
  - Test standalone build
  - Balance HP, damage, move speeds

---

## Quick Reference: What Works Now

| Feature | Status | How to Test |
|---------|--------|-------------|
| Player movement | Working | A/D move, Space jump, Shift dash |
| Player combat | Working | Fire1 attack (3-hit combo), Fire2 air slam |
| Enemy AI | Working | Enemy patrols, chases, attacks |
| Combat feedback | Working | Hit-stop, knockback, camera shake, damage numbers |
| Roguelike loop | Working | RunManager starts run, PlayerStats tracks HP/gold |
| Class system | Code only | Class_Warrior exists but empty |
| Save/load | Code only | No persistent save system yet |

## Quick Reference: What Needs Manual Work

| Task | Location | Notes |
|------|----------|-------|
| Replace placeholder sprites | `Assets/Prefabs/Player.prefab`, `Enemy.prefab` | Drag new sprites to SpriteRenderer |
| Create AnimatorControllers | `Assets/Animation/` | Create in Animator window, assign to prefabs |
| Add UI to scene | `TestArena` scene | Create Canvas + HealthBar GameObjects |
| Assign audio clips | `AudioManager` GameObject | Created empty by GameInitializer |
| Fill Class_Warrior data | `Assets/Data/Class_Warrior.asset` | Add appearance, skills, equipment |

---

## Next Recommended MCP Session

When MCP is available again, the highest-value automated tasks are:

1. **Create UI Canvas + HealthBar** in TestArena scene (scene object creation)
2. **Create AnimatorControllers** with all states and parameters (asset creation + prefab assignment)
3. **Bulk-create ScriptableObjects** for skills, equipment, appearances (data assets)
4. **Add AudioManager clips** and wire to controllers (component field assignment)

Until then, all art asset creation can proceed manually through the Unity Editor.
