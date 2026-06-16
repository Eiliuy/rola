# ASSETS_GUIDE.md

Step-by-step guide for creating and importing art assets for the rola Unity project without MCP.

All steps can be done through the Unity Editor Inspector and Project window.

---

## Sprite Import Settings

When you import a PNG into the project, select it in the Project window and use these settings in the Inspector:

| Setting | Recommended Value | Notes |
|---------|-------------------|-------|
| Texture Type | Sprite (2D and UI) | Required for 2D rendering |
| Sprite Mode | Single (or Multiple for sheets) | Use Multiple for sprite sheets |
| Pixels Per Unit | 32 or 64 | Pixel art: 32 or 64. High-res: 100 |
| Filter Mode | Point (no filter) | Sharp pixels for pixel art. Use Bilinear for high-res |
| Compression | None | For pixel art to avoid artifacts. Use Normal Quality for large textures |
| Max Size | 2048 or 4096 | Match your sprite sheet size |

After changing settings, click **Apply**.

---

## Player Character Sprites

The player uses a multi-part sprite system with 9 body part slots. Each slot is a separate SpriteRenderer with a specific sorting order:

| Slot | Sorting Order | Description |
|------|---------------|-------------|
| BackHair | 0 | Hair behind the head |
| Body | 1 | Base body (required) |
| Bottom | 2 | Pants, skirt, legs |
| Top | 3 | Shirt, armor chest |
| Shoes | 4 | Footwear |
| Gloves | 5 | Handwear |
| Weapon | 6 | Held weapon |
| FrontHair | 7 | Hair in front of face |
| Accessory | 8 | Hats, glasses, etc. |

Each part should be a separate sprite with a transparent background. The `Body` sprite is the base; all other parts overlay on top.

**For a simple start:** create a single full-body sprite and assign it only to the `bodySprite` field. Leave all other part slots empty (null). The system skips null holders gracefully.

### Required animation states and frame counts

| State | Minimum Frames | Notes |
|-------|----------------|-------|
| Idle | 1+ | Can be a single static frame |
| Run | 6+ | Looping cycle |
| Jump | 1+ | Takeoff or airborne pose |
| Fall | 1+ | Descending pose |
| Attack1 | 2+ | First combo hit |
| Attack2 | 2+ | Second combo hit |
| Attack3 | 2+ | Third combo hit |
| AirAttack1 | 2+ | Airborne attack variant 1 |
| AirAttack2 | 2+ | Airborne attack variant 2 |
| AirSlam | 2+ | Downward slam attack |
| Dash | 2+ | Quick dash motion |
| Hurt | 1+ | Hit reaction |
| Dead | 1+ | Death pose |

---

## Enemy Sprites

The enemy uses a simpler single-sprite setup (no multi-part system).

### Required animation states and frame counts

| State | Minimum Frames | Notes |
|-------|----------------|-------|
| Idle | 1+ | Static or subtle breathing |
| Patrol / Run | 4+ | Looping movement cycle |
| Attack | 2+ | Attack animation |
| Hurt | 1+ | Hit reaction |
| Dead | 1+ | Death pose |

---

## VFX Sprites

| Effect | Description | Import Notes |
|--------|-------------|--------------|
| SlashEffect | White slash shape, transparent background | Assign to `SlashEffect` prefab SpriteRenderer |
| HitEffect | Spark or blood particle sprite | Used by particle system or SpriteRenderer |
| DeathEffect | Dust or particle burst sprite | Used by particle system |

These are typically small sprites (32x32 to 64x64) used by ParticleSystems or short-lived SpriteRenderers.

---

## UI Sprites

| Sprite | Purpose | Notes |
|--------|---------|-------|
| HealthBarFrame | Border/frame for HP bar | 9-slice sprite recommended for scaling |
| HealthBarFill | Colored fill for HP bar | Usually a simple rectangle |
| SkillIcon1-4 | Skill slot icons | 64x64 or 128x128, square |
| ClassIcon_Warrior | Class selection icon | 64x64 or 128x128, square |

Import UI sprites with the same settings as other sprites (Texture Type = Sprite (2D and UI)).

---

## Creating a CharacterAppearanceData Asset

1. In the Project window, right-click the `Assets/Data/` folder
2. Choose `Create > Rola > Character Appearance Data`
3. Name the asset (e.g., `Appearance_Warrior`)
4. Select the asset and configure in the Inspector:
   - **Body Sprite:** drag your player full-body sprite here (required)
   - **Skin Color:** set a color value
   - **Hair Color:** set a color value
   - **Eye Color:** set a color value
   - **Body Parts:** expand the list and assign `CharacterPartData` assets to each slot (optional for simple setup)

### Creating CharacterPartData assets (optional)

1. Right-click in `Assets/Data/`
2. `Create > Rola > Character Part Data`
3. Name it (e.g., `Part_Warrior_Weapon`)
4. Assign the sprite and configure offset/rotation if needed
5. Drag it into the corresponding slot on `Appearance_Warrior`

---

## Creating a PlayerClassData Asset

1. Right-click in `Assets/Data/`
2. `Create > Rola > Player Class Data`
3. Name it (e.g., `Class_Mage`)
4. Configure fields in the Inspector:
   - **Class Name:** display name
   - **Description:** flavor text
   - **Base Stats:** maxHP, moveSpeed, jumpForce, attack, defense, etc.
   - **Default Appearance:** drag a `CharacterAppearanceData` asset here
   - **Starting Skills:** drag `SkillData` assets into the array
   - **Starting Equipment:** drag `EquipmentData` assets into the array

---

## Replacing Placeholder Sprites on Prefabs

### Player prefab
1. Open `Assets/Prefabs/Player.prefab` (double-click in Project window)
2. Select the `Visuals` child object
3. In the Inspector, find the **SpriteRenderer** component
4. Drag your new sprite from the Project window into the **Sprite** field
5. Save the prefab (Ctrl+S)
6. If using multi-part rendering, check the `CharacterAppearanceRenderer` component on `Visuals` and assign sprites to each holder slot

### Enemy prefab
1. Open `Assets/Prefabs/Enemy.prefab`
2. Select the root `Enemy` object
3. In the Inspector, find the **SpriteRenderer** component
4. Replace the sprite
5. Save the prefab

### Ground / Platform sprites
1. In the `TestArena` scene, select `Ground_Main`, `Ground_Left`, or `Ground_Right`
2. In the Inspector, find the **SpriteRenderer** component
3. Drag the new tileset sprite into the **Sprite** field
4. Adjust the **BoxCollider2D** size to match the new sprite dimensions
5. Save the scene (Ctrl+S)

---

## Ground and Platform Tileset

The TestArena scene uses three ground objects:
- `Ground_Main` (center)
- `Ground_Left` (left extension)
- `Ground_Right` (right extension)

These are simple SpriteRenderer + BoxCollider2D objects. To replace them with a proper tileset:

1. Import your tileset PNG to `Assets/Sprites/Environment/`
2. Set Texture Type = Sprite (2D and UI), Sprite Mode = Multiple
3. Use the Sprite Editor to slice the sheet into individual tiles
4. In the scene, select each ground object and replace its sprite
5. Adjust BoxCollider2D to match the new sprite shape

Alternatively, replace the static sprites with a **Tilemap** for more complex level design:
1. `GameObject > 2D Object > Tilemap`
2. Create a Tile Palette from your sliced sprites
3. Paint the ground using the Tilemap tools
4. Add a **TilemapCollider2D** for automatic collision

---

## Summary Checklist for Artists

- [ ] Import all player sprites to `Assets/Sprites/Player/`
- [ ] Import all enemy sprites to `Assets/Sprites/Enemy/`
- [ ] Import environment sprites to `Assets/Sprites/Environment/`
- [ ] Import VFX sprites to `Assets/Sprites/VFX/`
- [ ] Import UI sprites to `Assets/Sprites/UI/`
- [ ] Create `Appearance_Warrior` CharacterAppearanceData in `Assets/Data/`
- [ ] Assign `Appearance_Warrior` to `Class_Warrior.asset`
- [ ] Replace placeholder sprites on `Player.prefab` and `Enemy.prefab`
- [ ] Replace ground sprites in `TestArena` scene
- [ ] Save all scenes and prefabs (Ctrl+S)
