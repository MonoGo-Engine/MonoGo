![Banner](https://raw.githubusercontent.com/BlizzCrafter/MonoGo/refs/heads/master/logos/Banner.png)

# Welcome to MonoGo!
[![Version](https://img.shields.io/nuget/v/MonoGo.Engine?style=for-the-badge&logo=git&logoColor=E73C00&logoSize=auto&label=MonoGo.Engine&labelColor=262626&color=707070)](https://github.com/MonoGo-Engine) [![NuGet](https://img.shields.io/badge/NuGet-MonoGo.Templates-blue.svg?style=for-the-badge&logo=NuGet&logoColor=0289CC&logoSize=auto&colorA=262626&colorB=707070)](https://www.nuget.org/packages/MonoGo.Templates) [![Docs](https://img.shields.io/github/labels/MonoGo-Engine/MonoGo/Documentation?style=for-the-badge&logo=gitbook&logoColor=E2B004&logoSize=auto&labelColor=262626&color=707070)](https://monogo-engine.github.io/monogo.github.io/)

A cross-platform C# 2D game engine built on top of MonoGame.

# Setup

### Automatic
The easiest and recommended way to start using the engine is by installing the Visual Studio templates:
- ```dotnet new install MonoGo.Templates```

### Manual
You can also add the packages manually:
- ```dotnet add package MonoGo.Engine```
- ```dotnet add package MonoGo.Engine.DesktopGL``` or ```dotnet add package MonoGo.Engine.WindowsDX```

### Modules
Install **optional** modules depending on your project needs:

* [(i)](https://www.mapeditor.org/) [![Tiled](https://img.shields.io/badge/NuGet-MonoGo.Tiled-blue.svg?style=flat-square&logo=NuGet&logoColor=0289CC&logoSize=auto&colorA=262626&colorB=707070)](https://www.nuget.org/packages/MonoGo.Tiled) 
* [(i)](https://github.com/RonenNess/Iguina) [![Iguina](https://img.shields.io/badge/NuGet-MonoGo.Iguina-blue.svg?style=flat-square&logo=NuGet&logoColor=0289CC&logoSize=auto&colorA=262626&colorB=707070)](https://www.nuget.org/packages/MonoGo.Iguina)
* [(i)](https://github.com/Jjagg/MgMercury) [![MercuryParticleEngine](https://img.shields.io/badge/NuGet-MonoGo.MercuryParticleEngine-blue.svg?style=flat-square&logo=NuGet&logoColor=0289CC&logoSize=auto&colorA=262626&colorB=707070)](https://www.nuget.org/packages/MonoGo.MercuryParticleEngine)

---

# AI & Agent Support (AGENTS.md)
If you are using AI coding assistants (like Cursor, GitHub Copilot, or CLI agents) to work with or contribute to MonoGo, please point them to the `AGENTS.md` file located in the root directory. 
This file contains crucial architectural rules, code style guidelines, and mandatory reference implementations specifically tailored for LLMs to ensure they write idiomatic MonoGo engine code.

---

# Features
* Graphics pipeline and automated batch/vertex buffer management.
* Easy sprite sheet animations.
* Texture packing.
* Sprite groups and dynamic graphics loading.
* Input management with GamePad support.
* Timers, alarms, cameras, and state machines.
* Coroutines.
* Hybrid Entity-Component (EC) architecture.
* Scene system with layers.
* GameDev-focused math library.
* Lightweight collision detection.
* Integrated post-processing management (includes Bloom & Color Grading effects).
* Powerful particle effect system.
* Extensive GUI system.
* Enhanced content management.
* FMOD audio support (as a standalone [library](https://github.com/Martenfur/FmodForFoxes/)).

# Sample Project

The included sample project contains many more demos, but here are a few impressions to get you started:

![Color Picker](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo/refs/heads/dev/MonoGo.Samples/Screenshots/ColorPicker.png)
![Tiled](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo/refs/heads/dev/MonoGo.Samples/Screenshots/Tiled.png)
![PostFX](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo/refs/heads/dev/MonoGo.Samples/Screenshots/PostFX.png)

# Credits

- [MonoGame](https://github.com/MonoGame/MonoGame) created by [MonoGame Foundation, Inc](https://monogame.net/)
- [Monofoxe](https://github.com/Martenfur/Monofoxe) created by [Chai Foxes](https://github.com/Martenfur) (Martenfur)
- [StbImageSharp](https://github.com/StbSharp/StbImageSharp) created by [Roman Shapiro](https://github.com/rds1983) (rds1983)
- [Iguina](https://github.com/RonenNess/Iguina) created by [Ronen Ness](https://github.com/RonenNess)
- [Mercury Particle Engine](https://github.com/Matthew-Davey/mercury-particle-engine) created by [Matthew Davey](https://github.com/Matthew-Davey) (Matt Davey)
- [ColorGrading](https://github.com/Kosmonaut3d/ColorGradingFilter-Sample) & [Bloom Filter](https://github.com/Kosmonaut3d/BloomFilter-for-Monogame-and-XNA) created by [Thomas Lüttich](https://github.com/Kosmonaut3d) (Kosmonaut3d)

For license information please take a look at the [License.txt](https://github.com/MonoGo-Engine/MonoGo/blob/master/LICENSE.txt) file.


# Now Have Fun with MonoGo!

![Banner](https://raw.githubusercontent.com/BlizzCrafter/MonoGo/refs/heads/master/logos/Social.png)
