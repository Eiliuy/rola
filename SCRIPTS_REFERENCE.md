# SCRIPTS_REFERENCE.md

Quick reference for all 49 scripts in the rola project. Use this to find which script to edit when tweaking behavior, values, or visual effects.

## How to Use This Reference

- **Path** is relative to `Assets/Scripts/`
- **Key Inspector Fields** are the most common values artists/designers adjust
- **Notes** explain relationships or caveats

---

## Player Scripts

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `PlayerController` | `PlayerController.cs` | Movement, combat, input handling | `moveSpeed`, `jumpForce`, `dashSpeed`, `dashDuration`, `attackRange`, `groundCheck`, `attackPoint`, `airSlamAttackPoint`, `groundCombo[]`, `airCombo[]`, `airSlam` | `AttackData` arrays configure combo timing, damage, hit-stop, and knockback per attack |
| `PlayerStats` | `PlayerStats.cs` | HP, death, respawn, invincibility | `maxHP`, `invincibleDuration`, `respawnPosition` | Listens to `PlayerBuild` stat changes; exposes `OnHPChanged` event for UI |
| `PlayerSpawnManager` | `PlayerSpawnManager.cs` | Spawns/moves Player to spawn point | `defaultSpawnPoint`, `playerPrefab` | Looks for GameObject tagged `Player` |
| `PlayerVisualController` | `Player/Visual/PlayerVisualController.cs` | Appearance + animation bridge | `appearanceRenderer`, `animationController`, `defaultAnimatorController`, `hurtFlashColor` | Calls `ApplyCurrentAppearance()` on Start; flips sprite via scale |
| `CharacterAppearanceRenderer` | `Player/Visual/CharacterAppearanceRenderer.cs` | Multi-part sprite assembly | `bodyHolder`, `backHairHolder`, `bottomHolder`, `topHolder`, `glovesHolder`, `shoesHolder`, `weaponHolder`, `frontHairHolder`, `accessoryHolder`, `baseSortingOrder` | 9 slots; null holders are skipped. Body is base, others overlay with sorting orders 0-8 |
| `PlayerAnimationController` | `Player/Visual/PlayerAnimationController.cs` | Animator wrapper | `animator` | Auto-finds Animator on same GameObject if not assigned |
| `CharacterPartSlot` | `Player/Visual/CharacterPartSlot.cs` | Enum definition for body part slots | - | Used by `CharacterAppearanceRenderer` and `CharacterPartData` |

## Enemy Scripts

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `EnemyController` | `EnemyController.cs` | Enemy AI state machine | `maxHP`, `sightRange`, `attackRange`, `patrolSpeed`, `patrolDistance`, `chaseSpeed`, `attackData`, `attackPoint` | States: Idle, Patrol, Chase, Attack, Hurt, Dead. Finds Player via `FindObjectOfType` |

## Data Assets (ScriptableObjects)

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `PlayerClassData` | `Data/PlayerClassData.cs` | Class definition asset | `className`, `baseMaxHP`, `baseAttackPower`, `baseMoveSpeed`, `baseJumpForce`, `defaultAppearance`, `startingSkills`, `startingEquipments` | Create via `Assets > Create > Rola > Player Class Data` |
| `CharacterAppearanceData` | `Data/CharacterAppearanceData.cs` | Appearance definition | `bodySprite`, `skinColor`, `hairColor`, `eyeColor`, `backHair`, `frontHair`, `top`, `bottom`, `gloves`, `shoes`, `weapon`, `accessory`, `animatorController` | Create via `Assets > Create > Rola > Character Appearance Data` |
| `CharacterPartData` | `Data/CharacterPartData.cs` | Single body part definition | `sprite`, `slot`, `colorTint`, `sortingOrderOffset`, `positionOffset`, `rotationOffset`, `scaleOverride` | Create via `Assets > Create > Rola > Character Part Data` |
| `SkillData` | `Data/SkillData.cs` | Skill definition | `skillName`, `cooldown`, `mpCost`, `baseDamage`, `rangeMultiplier`, `knockbackForce`, `animationTrigger`, `effectPrefab`, `castSound` | Create via `Assets > Create > Rola > Skill Data` |
| `EquipmentData` | `Data/EquipmentData.cs` | Equipment definition | `equipmentName`, `slot`, `rarity`, `modifiers`, `effects` | Create via `Assets > Create > Rola > Equipment Data` |
| `UpgradeData` | `Data/UpgradeData.cs` | Upgrade reward definition | `upgradeType`, `skillData`, `equipmentData`, `statModifier`, `specialEffect` | Used by `UpgradeManager` |
| `AttackData` | `AttackData.cs` | Attack configuration | `damage`, `damageMultiplier`, `rangeMultiplier`, `knockbackForce`, `knockbackUpForce`, `startupTime`, `activeTime`, `recoveryTime`, `inputBufferTime`, `hitStopDuration`, `cameraShakeDuration`, `cameraShakeMagnitude`, `canCrit`, `usableInAir` | Used in PlayerController combo arrays and EnemyController |

## Roguelike / Core Systems

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `RunManager` | `Roguelike/RunManager.cs` | Run lifecycle + resources | `defaultClass` | Singleton. Auto-created by `GameInitializer` |
| `RunData` | `Roguelike/RunData.cs` | Run data container | - | Tracks gold, exp, level, room, build |
| `UpgradeManager` | `Roguelike/UpgradeManager.cs` | Upgrade reward logic | - | Singleton. Auto-created by `GameInitializer` |
| `PlayerBuild` | `Core/PlayerBuild.cs` | Build assembly (class + skills + equipment + upgrades) | - | Code-only; recalculates stats and manages equipment effects |
| `CharacterStats` | `Core/CharacterStats.cs` | Final stat container with modifiers | - | Base stats + modifiers = final values |
| `SkillCooldown` | `Core/SkillCooldown.cs` | Skill cooldown tracking | - | Used by `PlayerController` |
| `StatModifier` | `Core/StatModifier.cs` | Stat modification definition | `statType`, `value`, `modificationType` | Used by equipment and class data |

## Equipment Effects

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `EquipmentEffectBase` | `Equipment/Effects/EquipmentEffectBase.cs` | Base class for effects | - | Extend for new effect types |
| `IEquipmentEffect` | `Equipment/Effects/IEquipmentEffect.cs` | Effect interface | - | Implemented by all effect classes |
| `OnHitEffect` | `Equipment/Effects/OnHitEffect.cs` | Effects triggered on hit | - | Ignite, Bleed, Stun |
| `OnDodgeEffect` | `Equipment/Effects/OnDodgeEffect.cs` | Effects triggered on dodge | - | Heal, Invincible |
| `OnLowHPEffect` | `Equipment/Effects/OnLowHPEffect.cs` | Effects triggered on low HP | - | Shield, Heal |
| `OnKillEffect` | `Equipment/Effects/OnKillEffect.cs` | Effects triggered on kill | - | Heal, Gold, AttackSpeed |

## Camera & FX

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `CameraFollow` | `CameraFollow.cs` | Smooth camera follow | `target`, `offset`, `smoothSpeed`, `useBounds`, `cameraBounds` | Attached to `Main Camera` |
| `CameraShake` | `CameraShake.cs` | Screen shake on hit | - | Singleton. Triggered by combat events |
| `HitStopManager` | `HitStopManager.cs` | Freeze time on hit | - | Singleton. Call `TriggerHitStop(duration)` |
| `SlashEffect` | `SlashEffect.cs` | Slash VFX | - | Spawned by `PlayerController` at attack point |
| `HitEffect` | `HitEffect.cs` | Hit impact VFX | - | Spawned at hit position |
| `DeathEffect` | `DeathEffect.cs` | Death burst VFX | - | Spawned on enemy death |

## Managers & Singletons

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `GameInitializer` | `GameInitializer.cs` | Bootstraps all singleton managers | `hitStopManagerPrefab`, `audioManagerPrefab`, `narrationManagerPrefab`, `settingsManagerPrefab`, `runManagerPrefab`, `upgradeManagerPrefab` | Currently all prefabs null; falls back to empty GameObjects |
| `AudioManager` | `AudioManager.cs` | SFX playback | `attackSounds[]`, `hurtSound`, `dashSound`, `jumpSound`, `enemyAttackSound`, `enemyHurtSound`, `enemyDeathSound`, `sfxVolume` | Singleton. Auto-created by `GameInitializer` |
| `SettingsManager` | `SettingsManager.cs` | Game settings | - | Singleton. Auto-created by `GameInitializer` |

## UI / Menus

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `HealthBar` | `HealthBar.cs` | UI health bar display | - | Listen to `PlayerStats.OnHPChanged` |
| `MainMenu` | `MainMenu.cs` | Main menu scene logic | - | Exists but no MainMenu scene yet |
| `PauseMenu` | `PauseMenu.cs` | Pause menu logic | - | Exists but no UI in scene yet |

## Narration

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `NarrationManager` | `NarrationManager.cs` | Narration/event system manager | - | Singleton. Auto-created by `GameInitializer` |
| `NarrationData` | `NarrationData.cs` | Narration line data | - | ScriptableObject-style data |
| `NarrationTrigger` | `NarrationTrigger.cs` | Trigger narration on enter | - | Attach to trigger Collider2D |
| `NarrationEventTrigger` | `NarrationEventTrigger.cs` | Trigger narration via UnityEvent | - | For button/event-driven narration |
| `NarrationBehaviorWatcher` | `NarrationBehaviorWatcher.cs` | Watches for behavior conditions | - | Triggers narration based on game state |

## Level / Misc

| Script | Path | Purpose | Key Inspector Fields | Notes |
|--------|------|---------|----------------------|-------|
| `Checkpoint` | `Checkpoint.cs` | Sets player respawn point | - | Attach to trigger Collider2D |
| `DeathZone` | `DeathZone.cs` | Kills player on enter | - | Attach to trigger Collider2D |
| `SceneTransition` | `SceneTransition.cs` | Scene loading transitions | - | For fade/transition effects |
| `IDamageable` | `IDamageable.cs` | Damageable interface | - | Implemented by PlayerController and EnemyController |

---

## Common Artist / Designer Tasks

**I want to change how fast the player moves:**
- Script: `PlayerController`
- Field: `moveSpeed`
- Location: Select Player in scene or open `Player.prefab`

**I want to change the camera follow smoothness:**
- Script: `CameraFollow`
- Field: `smoothSpeed`
- Location: Select `Main Camera` in TestArena scene

**I want to change how long the screen freezes on hit:**
- Script: `PlayerController` or `EnemyController`
- Field: `attackData.hitStopDuration` / `attackData.hitStopDuration`
- Location: Select Player/Enemy, expand the attackData array

**I want to add a new class:**
- Script: `PlayerClassData` (create a new asset)
- Steps: Right-click `Assets/Data/` > Create > Rola > Player Class Data, fill fields, assign appearance

**I want to replace the player sprite:**
- Script: `CharacterAppearanceRenderer` / `PlayerVisualController`
- Steps: Open `Player.prefab`, select `Visuals`, replace SpriteRenderer sprite or assign a `CharacterAppearanceData`

**I want to add UI to the scene:**
- Scripts: `HealthBar`, `PauseMenu`
- Steps: Create Canvas in scene, attach scripts, wire events and button onClick handlers

**I want to add sound effects:**
- Scripts: `AudioManager`, `PlayerController`, `EnemyController`
- Steps: Import clips to `Assets/Audio/`, assign to AudioManager and controller fields

---

## Script Category Summary

| Category | Count | Scripts |
|----------|-------|---------|
| Player | 7 | PlayerController, PlayerStats, PlayerSpawnManager, PlayerVisualController, CharacterAppearanceRenderer, PlayerAnimationController, CharacterPartSlot |
| Enemy | 1 | EnemyController |
| Data Assets | 7 | PlayerClassData, CharacterAppearanceData, CharacterPartData, SkillData, EquipmentData, UpgradeData, AttackData |
| Roguelike / Core | 7 | RunManager, RunData, UpgradeManager, PlayerBuild, CharacterStats, SkillCooldown, StatModifier |
| Equipment Effects | 6 | EquipmentEffectBase, IEquipmentEffect, OnHitEffect, OnDodgeEffect, OnLowHPEffect, OnKillEffect |
| Camera / FX | 6 | CameraFollow, CameraShake, HitStopManager, SlashEffect, HitEffect, DeathEffect |
| Managers | 3 | GameInitializer, AudioManager, SettingsManager |
| UI / Menus | 3 | HealthBar, MainMenu, PauseMenu |
| Narration | 5 | NarrationManager, NarrationData, NarrationTrigger, NarrationEventTrigger, NarrationBehaviorWatcher |
| Level / Misc | 4 | Checkpoint, DeathZone, SceneTransition, IDamageable |

**Total: 49 scripts**
