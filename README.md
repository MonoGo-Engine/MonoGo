![Banner](https://raw.githubusercontent.com/BlizzCrafter/MonoGo/refs/heads/master/logos/Banner.png)

# Welcome to MonoGo!
[![Version](https://img.shields.io/nuget/v/MonoGo.Engine?style=for-the-badge&logo=git&logoSize=auto&label=MonoGo.Engine&labelColor=262626&color=E73C00)](https://github.com/MonoGo-Engine) [![NuGet](https://img.shields.io/badge/NuGet-MonoGo.Templates-blue.svg?style=for-the-badge&logo=NuGet&logoColor=blue&logoSize=auto&colorA=262626&colorB=E73C00)](https://www.nuget.org/packages/MonoGo.Templates) [![Docs](https://img.shields.io/github/labels/MonoGo-Engine/MonoGo/Documentation?style=for-the-badge&logo=gitbook&logoSize=auto&labelColor=262626&color=CC9C00)](https://monogo-engine.github.io/monogo.github.io/)

Cross-Platform .NET 8 C# 2D game engine build ontop of MonoGame.

# Setup

The easiest way of using this game engine is to install the templates:
- ```dotnet new install MonoGo.Templates```

If you prefere a more manual process you could also install the packages one by one in your existing MonoGame project like this:
  - ```dotnet add package MonoGo.Engine```
  - ```dotnet add package MonoGo.Engine.DesktopGL``` or ```dotnet add package MonoGo.Engine.WindowsDX```
  - ```dotnet add package MonoGo.Engine.Pipeline```

- Install **Optional** Packages (You don't need to!):
  - ```dotnet add package MonoGo.Tiled```
 
Of course it's also possible to build everything from source and working directly with the engine, which is very useful for debugging.

# Features
* Graphics Pipeline and Automated Batch\Vertex Buffer Management.
* Easy SpriteSheet-Animations.
* Texture Packing.
* Sprite Groups and Dynamic Graphics Loading.
* Input Management with GamePad support.
* Timers, Alarms, Cameras, State Machines.
* Coroutines.
* Hybrid EC.
* Scene System with Layers.
* GameDev related Math lib.
* Lightweight Collision Detection.
* Integrated Post-Processing Management (Includes Bloom & ColorGrading Effects).
* Powerful Particle-Effect-System.
* Extensive GUI-System.
* Enhanced Content Management.
* FMOD Audio Support (As a standalone [Library](https://github.com/Martenfur/FmodForFoxes/)).
### Modules:
* Tiled Map Support via [Tiled](https://www.mapeditor.org/).

# Sample Project

The **[sample project](https://github.com/MonoGo-Engine/MonoGo.Samples)** itself contains alot more demos, but here are some impressions at least:

![Color Picker](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo.Samples/refs/heads/master/docs/sample_00.jpg)
![Tiled](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo.Samples/refs/heads/master/docs/sample_01.jpg)
![PostFX](https://raw.githubusercontent.com/MonoGo-Engine/MonoGo.Samples/refs/heads/master/docs/sample_02.jpg)

Click **[here](https://github.com/MonoGo-Engine/MonoGo.Samples)** to see more engine features!

# Credits

- [MonoGame](https://github.com/MonoGame/MonoGame) created by [MonoGame Foundation, Inc](https://monogame.net/)
- [Monofoxe](https://github.com/Martenfur/Monofoxe) & [Nopipeline](https://github.com/Martenfur/Nopipeline) created by [Chai Foxes](https://github.com/Martenfur) (Martenfur)
- [StbImageSharp](https://github.com/StbSharp/StbImageSharp) created by [Roman Shapiro](https://github.com/rds1983) (rds1983)
- [Iguina](https://github.com/RonenNess/Iguina) created by [Ronen Ness](https://github.com/RonenNess)
- [Mercury Particle Engine](https://github.com/Matthew-Davey/mercury-particle-engine) created by [Matthew Davey](https://github.com/Matthew-Davey) (Matt Davey)
- [ColorGrading](https://github.com/Kosmonaut3d/ColorGradingFilter-Sample) & [Bloom Filter](https://github.com/Kosmonaut3d/BloomFilter-for-Monogame-and-XNA) created by [Thomas Lüttich](https://github.com/Kosmonaut3d) (Kosmonaut3d)

For license information please take a look at the [License.txt](https://github.com/MonoGo-Engine/MonoGo/blob/master/LICENSE.txt) file.


# Now Have Fun with MonoGo!

![Banner](https://raw.githubusercontent.com/BlizzCrafter/MonoGo/refs/heads/master/logos/Social.png)
