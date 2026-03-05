# MonoGo Engine - Agent Operating Procedures

## 1. Environment & Architecture
* **Language:** C# 9.0+ (.NET 9 Target)
* **Framework:** MonoGame
* **Architecture:** Component-based Game Engine (Hybrid EC framework)
* **Goal:** Cross-platform 2D game engine. Always mimic the engine's object-oriented principles, modularity, and maintain performance optimization.

## 2. Build & Lint Commands
Agents MUST use standard `.NET Core` CLI commands for executing workflows.
* **Build Solution:** `dotnet build MonoGo.sln`
* **Clean Project:** `dotnet clean MonoGo.sln`
* **Run Samples:** `dotnet run --project ./MonoGo.Samples/MonoGo.Samples.csproj`
* **Linting / Code Quality:** `dotnet format` or rely on IDE/MSBuild-level analyzers (CSxxxx warnings). Address warnings proactively.

## 3. Testing Commands
Tests use VSTest (`dotnet test`).
* **List Available Tests:** `dotnet test -t`
* **Run All Tests:** `dotnet test`
* **Run a Single Test:** `dotnet test --filter <TestName>`
* **Run a Specific Test Class:** `dotnet test --filter FullyQualifiedName~Namespace.TestClassName`

## 4. Code Style & Formatting Guidelines
* **Namespaces:** Block-scoped namespaces (`namespace MonoGo.Engine { ... }`). Group `using` directives at the very top.
* **Naming:** `PascalCase` for Classes/Methods/Properties. `_camelCase` for private fields (prefix with `_`). `camelCase` for parameters/locals.
* **Formatting:** 4 spaces (No Tabs). Allman style braces (new line for `{`). Single-line `if` without braces is permitted if concise.
* **Documentation:** All public members MUST have XML documentation (`/// <summary>...</summary>`).
* **Nullability:** Mark nullability constructs appropriately. Use the null-forgiving operator (`!`) only when logically safe.

## 5. Global Managers (*Mgr) & Core Systems
The engine utilizes static Manager classes for globally accessible states. Always use these instead of creating local instances:
* `CameraMgr`: Access `CameraMgr.Cameras[0]` for the currently active camera.
* `GameMgr` & `WindowMgr`: For core game lifecycle and window/display properties.
* `GraphicsMgr`: For accessing the active `GraphicsDevice` and the custom `VertexBatch` (used instead of MonoGame's `SpriteBatch`).
* `RenderMgr`: Handles surfaces, viewport adapters, and full-screen post-processing.

## 6. Entity Component System (ECS) & Game Logic
* **Architecture:** Create logic by extending `MonoGo.Engine.EC.Entity` or `MonoGo.Engine.EC.Component`.
* **Coroutines & Jobs:** DO NOT use standard C# `async/await` for game-tick logic. Use `Entity.StartCoroutine(IEnumerator routine)` or `Entity.StartJob()` which are integrated directly into the engine's update loop.
* **Alarms:** Do NOT use `System.Timers.Timer` or `System.Threading.Timer` for delayed logic. Use `MonoGo.Engine.Utils.Alarm` instead, as it safely ties into the game loop's time.
* **State Machines:** The engine provides a native stack-based `MonoGo.Engine.Utils.StateMachine<T>`. Use this instead of building custom state managers.

## 7. Scene System & Content Management
* **Scene System (`MonoGo.Engine.SceneSystem`):** Use `Scene` and `SceneMgr` to manage game states and layers.
* **Content Management (`MonoGo.Engine.Resources`):** The engine uses `ResourceHub` which holds `ResourceBox` implementations (e.g., `DirectoryResourceBox`) to load and unload assets dynamically. Do NOT rely purely on MonoGame's `ContentManager`.

## 8. Cameras, Rendering & World Space
* **Viewport Adapters:** For Independent-Resolution-Rendering, assign a ViewportAdapter to the RenderMgr (e.g., `RenderMgr.ViewportAdapter = new ScalingViewportAdapter(1280, 720);`).
* **World vs Screen Coordinates:** `Input.ScreenMousePosition` provides raw screen coordinates. To get transformed world-space coordinates, ALWAYS use `CameraMgr.Cameras[0].GetRelativeMousePosition()`.
* **Surfaces (Managed RenderTargets):** Use `MonoGo.Engine.Drawing.Surface` instead of raw `RenderTarget2D`. Surfaces automatically integrate with `VertexBatch` and matrix transformations. They use a static stack-based targeting system: call `Surface.SetTarget(surface)` to begin rendering to it (this automatically handles matrix states and batch flushing) and `Surface.ResetTarget()` when done. Render the surface itself easily via `surface.Draw()`.
* **Rendering & Debugging:** The engine uses a custom `MonoGo.Engine.Drawing.VertexBatch`. For debugging, utilize built-in drawing tools like `LineShape` and `CircleShape`.
* **Culling & Layers:** Use `Camera.RenderMask` for bitwise culling of specific layers and scenes.

## 9. Serialization & Data Formats
* **Engine Serialization:** Always refer to `MonoGo.Engine.Serialization` when serializing/deserializing engine formats.
* **Polymorphism & Converters:** This system handles polymorphic serialization using attributes like `[MonoGoConverter]`. Do not use generic `System.Text.Json` settings without integrating the engine's configurations.
* **JsonNodes:** Use the `SerializeToNode` method to simply and directly create a `JsonNode` from an object.

## 10. Sprites & Animations
* **Sprite System:** The engine uses its own robust sprite system. Avoid using standard raw `Texture2D` rendering for entities when sprites are more appropriate.
* **Frames & SpriteSheets:** Use `MonoGo.Engine.Drawing.Sprite` which inherently supports animations by holding an array of `MonoGo.Engine.Drawing.Frame` objects. These map directly to Texture Atlases/SpriteSheets.

## 11. Lightweight Collision Detection
* **Built-in System:** Do NOT import heavy physics engines (like Farseer or Velcro) unless explicitly required. The engine features a fast, built-in lightweight collision detection system.
* **Usage:** Use `MonoGo.Engine.Collisions.CollisionChecker.CheckCollision(Collider a, Collider b)` along with the engine's built-in colliders (e.g., `CircleCollider`, `RectangleCollider`, `LineCollider`) and shapes for performant spatial checks.

## 12. Audio Support
* **Default Audio:** By default, use standard MonoGame `SoundEffect` or `Song` classes for game audio.
* **FMOD (Optional):** The engine supports FMOD as an optional, standalone library via **FmodForFoxes** (https://github.com/Martenfur/FmodForFoxes/). Only use FMOD if the library is explicitly referenced in the project or requested by the user.

## 13. Optional Modules (Iguina, Tiled, Particles)
The engine can operate entirely on its own, but offers optional modules. **BEFORE** using any of these, always verify they are actually referenced in the project's `.csproj`:
* **UI System (`MonoGo.Iguina`):** If included, the global UI system is accessible via `MonoGo.Iguina.GUIMgr.System`. 
* **Tile Maps (`MonoGo.Tiled`):** Used for parsing and rendering Tiled maps. 
* **Particles (`MonoGo.MercuryParticleEngine`):** Provides advanced particle effect components (e.g., `ParticleEffectComponent`).

## 14. Key Engine Utilities (Do NOT reinvent the wheel)
* **Colors:** Use `MonoGo.Engine.ColorHelper` and `MonoGo.Engine.HSLColor` for conversions (Color, HSL, HEX). DO NOT use `System.Drawing.Color`.
* **Math & Geometry:** Use `MonoGo.Engine.Utils.GameMath` for common calculations. Check `Vector2Extensions`, `RectangleExtensions`, and `NumberExtensions` before writing custom math. Always use `MonoGo.Engine.Angle` for angle representations to ensure correct 0..359 degree wrapping. Use `MonoGo.Engine.Utils.SlowRotator` and `MonoGo.Engine.Utils.AngleDamper` for smooth angle transitions and damping over time.
* **Collections:** Use `MonoGo.Engine.Utils.CustomCollections.SafeList<T>` when you need a list that allows safe modification during enumeration. Use `MonoGo.Engine.Utils.CustomCollections.Pool<T>` for object pooling (`IPoolable`) to prevent garbage collection spikes.
* **Animations:** Use `MonoGo.Engine.Utils.Animation` and `MonoGo.Engine.Utils.Easing` for generic value interpolations and tweening.
* **Input Management:** Use `MonoGo.Engine.Input` to check Keyboard, Mouse, and GamePad states.
* **Time:** Use `MonoGo.Engine.Utils.TimeKeeper.Global` for tracking elapsed time (`TimeKeeper.Global.TimeMultiplier` for slow-motion).

## 15. Reference Implementations (Code Snippets & Examples)
DO NOT invent implementation patterns. If you need examples of how to correctly implement engine features, ALWAYS read the respective files in the `MonoGo.Samples/Demos/` directory first. 

**Important for NuGet Users:** If the `MonoGo.Samples/Demos/` directory is not available locally, you MUST use web-fetching tools to read the raw files directly from the repository:
`https://raw.githubusercontent.com/MonoGo-Engine/MonoGo/refs/heads/master/MonoGo.Samples/Demos/[FileName]`

Each demo showcases specific capabilities:
* **`CollisionsDemo.cs`**: Lightweight Collision Detection (Colliders, spatial checks).
* **`CoroutinesDemo.cs`**: Coroutines, Jobs, and Alarms (async logic tied to game ticks).
* **`ECDemo.cs`**: Entity-Component (ECS) architecture usage and instantiation.
* **`InputDemo.cs`**: Keyboard, Mouse, and Gamepad input handling.
* **`ParticlesDemo.cs`**: Advanced particle effects (requires `MonoGo.MercuryParticleEngine`).
* **`PrimitiveDemo.cs`**: Rendering raw geometric primitives and basic geometry.
* **`SceneSystemDemo.cs`**: Scene setup, layer management, and `SceneMgr` usage.
* **`ShapeDemo.cs`**: Utilizing built-in debug drawing tools (`LineShape`, `CircleShape`, etc.).
* **`SpriteDemo.cs`**: Sprite sheets, `Frame` usage, and animations.
* **`TiledDemo.cs`**: Parsing and rendering Tiled maps (requires `MonoGo.Tiled`).
* **`UIDemo.cs`**: Comprehensive GUI system implementation using `MonoGo.Iguina`.
* **`UtilsDemo.cs`**: Utilizing `GameMath`, `TimeKeeper`, `ColorHelper`, and general utilities.
* **`VertexBatchDemo.cs`**: High-performance custom `VertexBatch` rendering (alternative to SpriteBatch).
