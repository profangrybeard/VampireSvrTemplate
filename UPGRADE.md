# Upgrade Guide

## Upgrading via ZIP Download

Since you're downloading ZIP files from GitHub (not using git), follow these steps to update while preserving your work.

---

## v1.0 → v1.1

### What's New in v1.1
- **Weighted Spawn System**: New SpawnMode options (Sequential/Interleaved/PureWeighted)
- **Version Tracking**: Check your version via menu `VampireSurvivor > Project Version`
- **Console Logging**: Version displays in console when game starts

### Upgrade Steps

1. **Backup your project first!**
   - Copy your entire project folder somewhere safe

2. **Download the new ZIP** from GitHub

3. **Close Unity** (important!)

4. **Replace ONLY the Scripts folder:**
   ```
   DELETE: YourProject/Assets/_Project/Scripts/
   COPY IN: NewZip/Assets/_Project/Scripts/
   ```

5. **Open Unity** and let it recompile

6. **Done!** Your data is preserved.

### What's Safe to Replace
| Folder | Safe to Replace? | Notes |
|--------|------------------|-------|
| `Assets/_Project/Scripts/` | ✅ YES | All code - replace entirely |
| `ProjectSettings/` | ✅ YES | Unity settings |
| `Packages/` | ✅ YES | Package manifest |

### What to KEEP (Don't Overwrite)
| Folder | Keep? | Notes |
|--------|-------|-------|
| `Assets/_Project/Data/` | ⚠️ KEEP | Your enemy configs, waves, game config |
| `Assets/_Project/Prefabs/` | ⚠️ KEEP | Your modified prefabs |
| `Assets/_Project/Scenes/` | ⚠️ KEEP | Your scene changes |
| `Assets/_Project/Art/` | ⚠️ KEEP | Your art assets |

### Compatibility Notes

Your existing `WaveData` assets will work without changes because:
- `SpawnMode` defaults to `Sequential` (the original behavior)
- Your `Enemies[]` array is unchanged
- New `WeightedEnemies[]` field is optional and empty by default

**To use the new weighted spawning:**
1. Select your WaveData asset
2. Change `SpawnMode` from `Sequential` to `Interleaved` or `PureWeighted`
3. Populate the `Weighted Enemies` array with enemy types and weights

---

## Checking Your Version

Three ways to check which version you have:

1. **Unity Menu**: `VampireSurvivor > Project Version`
2. **Console**: Look for cyan `[VampireSurvivor] v1.1.0` message when playing
3. **Code**: `Debug.Log(ProjectVersion.FullVersion);`

---

## Troubleshooting

### "Missing script" errors after upgrade
- Make sure you copied the `.meta` files along with the `.cs` files
- If issues persist, delete `Library/` folder and reopen Unity

### WaveData shows "Missing (Mono Script)"
- The WaveData.cs file didn't copy correctly
- Re-copy `Assets/_Project/Scripts/Data/WaveData.cs` and its `.meta` file

### Enemies not spawning
- Check that `SpawnMode` matches your setup:
  - `Sequential` uses `Enemies[]` array
  - `Interleaved`/`PureWeighted` use `WeightedEnemies[]` array
