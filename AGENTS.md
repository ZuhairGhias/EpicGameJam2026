# AGENTS.md — Unity Game Jam Guide

This file tells coding agents (Codex / Cursor / etc.) how to work safely and quickly in this Unity game jam repo.

## Goal & constraints (game jam mode)
- Optimize for **shipping a playable build fast**, not perfect architecture.
- Prefer **small, reversible changes** over sweeping refactors.
- If uncertain, choose the option with **least risk to breaking the Unity project**.

## Project basics
- Engine: Unity **[PUT UNITY VERSION HERE]**
- Render pipeline: **[URP/HDRP/Built-in]**
- Target platform(s): **[Windows/WebGL/Mac/Linux/etc.]**
- Input: **[Old Input Manager / New Input System]**
- Networking: **[None/Netcode/etc.]**

## Do / Don’t (very important)
### DO
- Keep edits focused to `Assets/` (scripts, prefabs, scenes) unless explicitly asked.
- Prefer adding new scripts/components over rewriting existing systems.
- Add short comments only where intent is unclear (jam speed > docs).
- When creating new features, also add a tiny “how to test” note in the PR/summary.

### DON’T
- Don’t mass-edit `.meta` files. Unity manages them.
- Don’t reorganize folders or rename assets without a clear reason (breaks GUID refs).
- Don’t touch `ProjectSettings/` or `Packages/` unless asked or required for a fix.
- Don’t introduce heavy dependencies/packages during a jam unless it unlocks big value.

## Repo layout conventions
- `Assets/_Project/` — our main game code and assets (preferred location)
  - `Assets/_Project/Scripts/`
  - `Assets/_Project/Art/`
  - `Assets/_Project/Audio/`
  - `Assets/_Project/Prefabs/`
  - `Assets/_Project/Scenes/`
- `Assets/ThirdParty/` — external assets/packages (avoid modifying)

If `_Project` doesn’t exist, follow existing structure; don’t invent a new one.

## Coding style (Unity/C#)
- Use `SerializeField` for inspector wiring; avoid making fields `public` unless necessary.
- Use `TryGetComponent` over `GetComponent` in hot paths.
- Prefer composition: small MonoBehaviours over deep inheritance.
- Avoid LINQ in per-frame loops (`Update`, `FixedUpdate`) to reduce GC allocations.
- Prefer `Awake` for references, `Start` for initialization that depends on other objects.
- Keep script names == class names == file names.

### Naming
- Classes: `PascalCase`
- Methods/fields: `camelCase`
- Serialized private fields: `[SerializeField] private Type someField;` (no leading underscore unless repo already uses it)

## Scenes, prefabs, and assets (Unity safety)
- If editing a **scene**:
  - Minimize changes; don’t reformat/rewire unrelated objects.
  - Prefer adding new objects under a clearly named root like `__Jam/` or `Gameplay/`.
- If editing a **prefab**:
  - Prefer prefab variants when you need one-off changes.
  - Avoid applying overrides that touch unrelated components.
- If creating assets:
  - Put them in the closest relevant folder under `Assets/_Project/`.
  - Name assets clearly: `Player.prefab`, `Enemy_Goblin.prefab`, `SFX_Click.wav`, etc.

## “Definition of Done” for a change
A change is done when:
1. Scripts compile (no Console errors).
2. The feature works in Play Mode in the intended scene(s).
3. Any new input/actions are wired and testable.
4. Any new prefabs/scenes are saved and referenced correctly.

If you can’t run Unity locally, still ensure:
- Code compiles logically (no missing namespaces/types),
- No obvious Unity serialization traps (renaming serialized fields, removing components referenced elsewhere, etc.).

## Common jam patterns (prefer these)
- State handling: simple enums or a lightweight state machine (no framework).
- Data: ScriptableObjects for tunables (e.g., `WeaponData`, `EnemyData`).
- Events: direct references or UnityEvents; avoid complex event buses unless already present.
- UI: keep it simple, avoid reworking layout systems late in the jam.

Unity CLI builds are optional in jam mode. Only add CI/build scripts if explicitly requested.

## When you are unsure
- Ask for the **smallest clarifying detail** needed (which scene, which prefab, desired behavior).
- Otherwise, implement the smallest safe version and leave TODO markers for polish.

## Notes for Codex/agents
- Keep instructions and changes **high-signal**.
- Avoid large “cleanup” commits.
- Prefer patches that can be reviewed quickly.