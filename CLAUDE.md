# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2D Vampire Survivors clone template for GAME 325. Demonstrates professional architecture patterns: ScriptableObjects for data, object pooling for performance, event-driven communication for decoupled systems.

## Build Commands

Unity projects are built through the Unity Editor.

- **Open project**: Open folder in Unity Hub
- **Build**: File > Build Settings > Build (Ctrl+Shift+B)
- **Play**: Ctrl+P

## Architecture

### Namespace Structure
```
VampireSurvivor                    # Root
VampireSurvivor.Core               # GameManager, ServiceLocator, EventBus
VampireSurvivor.Data               # ScriptableObject definitions
VampireSurvivor.Entities           # Player, Enemy, Projectile
VampireSurvivor.Systems            # Spawner, WeaponSystem
VampireSurvivor.Systems.Pooling    # ObjectPool, PoolManager
VampireSurvivor.Events             # Event structs
VampireSurvivor.Interfaces         # IDamageable, IPoolable, IKillable
VampireSurvivor.UI                 # HealthBar, WaveDisplay, GameOverUI
```

### Key Patterns

**Service Locator** (`Core/ServiceLocator.cs`): Lightweight dependency injection. Register with `ServiceLocator.Register<T>(instance)`, retrieve with `ServiceLocator.Get<T>()`.

**EventBus** (`Core/EventBus.cs`): Decoupled communication via static events. Publish with `EventBus.Publish(new SomeEvent(...))`. Subscribe in `OnEnable`, unsubscribe in `OnDisable`.

**Object Pooling** (`Systems/Pooling/`): Generic `ObjectPool<T>` for any Component implementing `IPoolable`. Configured via `PoolManager` Inspector.

**ScriptableObjects** (`Data/`): All tunable data externalized. Create assets via Assets > Create > VampireSurvivor menu.

### Project Structure
```
Assets/_Project/
├── Data/                    # ScriptableObject instances
│   ├── Enemies/            # EnemyData assets
│   ├── Weapons/            # WeaponData assets
│   └── Waves/              # WaveData assets
├── Prefabs/
│   ├── Characters/         # Player.prefab, Enemies/
│   └── Projectiles/        # Projectile prefabs
├── Scripts/
│   ├── Core/               # GameManager, EventBus, ServiceLocator, CameraFollow
│   ├── Data/               # SO definitions (GameConfig, EnemyData, WeaponData, WaveData)
│   ├── Entities/           # Player/, Enemies/, Projectiles/
│   ├── Systems/            # Spawning/, Weapons/, Pooling/
│   ├── Events/             # GameEvents.cs
│   ├── Interfaces/         # IDamageable, IPoolable, IKillable
│   └── UI/                 # HealthBar, WaveDisplay, GameOverUI
└── Scenes/
```

### Critical Files (in dependency order)
1. `Scripts/Core/ServiceLocator.cs` - Service container
2. `Scripts/Core/EventBus.cs` - Event dispatcher
3. `Scripts/Events/GameEvents.cs` - Event definitions
4. `Scripts/Interfaces/*.cs` - Contracts
5. `Scripts/Data/*.cs` - ScriptableObject definitions
6. `Scripts/Systems/Pooling/ObjectPool.cs` - Generic pool
7. `Scripts/Entities/Player/PlayerController.cs` - Input handling
8. `Scripts/Entities/Enemies/Enemy.cs` - Enemy base class
9. `Scripts/Systems/Weapons/WeaponSystem.cs` - Auto-fire logic
10. `Scripts/Systems/Spawning/EnemySpawner.cs` - Wave management

## Unity Setup Required

After cloning, complete these steps in Unity Editor:

### 1. Layers (Edit > Project Settings > Tags and Layers)
- Layer 8: `Player`
- Layer 9: `Enemy`
- Layer 10: `PlayerProjectile`

### 2. Physics2D Collision Matrix (Edit > Project Settings > Physics 2D)
- Uncheck: PlayerProjectile vs Player
- Check: PlayerProjectile vs Enemy

### 3. Tags (Edit > Project Settings > Tags and Layers)
- `Player`
- `Enemy`

### 4. Create ScriptableObject Assets (Assets > Create > VampireSurvivor)
- `Data/GameConfig.asset` - Reference starting weapon and waves
- `Data/Enemies/Enemy_Bat.asset` - PoolKey: "enemy_bat"
- `Data/Weapons/Weapon_MagicMissile.asset` - ProjectilePoolKey: "projectile_magic"
- `Data/Waves/Wave_01.asset` - Reference Enemy_Bat

### 5. Create Prefabs
**Player.prefab**: SpriteRenderer (blue square), Rigidbody2D (freeze rotation), BoxCollider2D, PlayerInput, PlayerController, PlayerHealth, WeaponSystem

**Enemy_Basic.prefab**: SpriteRenderer, Rigidbody2D (freeze rotation), CircleCollider2D (trigger), Enemy, EnemyMovement. Tag: "Enemy", Layer: "Enemy"

**Projectile_Basic.prefab**: SpriteRenderer (circle), CircleCollider2D (trigger), Projectile. Layer: "PlayerProjectile"

### 6. Scene Hierarchy
```
[GameManager]         - GameManager.cs
[PoolManager]         - PoolManager.cs (configure pools in Inspector)
[EnemySpawner]        - EnemySpawner.cs (assign GameConfig)
Player                - Instance of Player.prefab
Main Camera           - CameraFollow.cs (assign Player as target, GameConfig)
Canvas
  └── HealthBar       - HealthBar.cs
  └── WaveText        - WaveDisplay.cs
  └── GameOverPanel   - GameOverUI.cs (initially inactive)
```

## Git LFS Configuration

Binary assets tracked via `.gitattributes`:
- Images: `.psd`, `.png`, `.jpg`, `.jpeg`
- Audio: `.wav`, `.mp3`, `.ogg`
- Models: `.fbx`, `.obj`
- Fonts: `.ttf`, `.otf`

## Unity Version

Unity 6 with 2D template (`com.unity.template.2d@10.1.0`).
