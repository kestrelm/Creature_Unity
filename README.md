# Creature-Runtimes

This is the runtime for Creature, the advanced 2D Skeletal and Mesh Animation Tool. This runtime is for Unity 5.0 and up. It allows you to load in, play back and control your authored Creature Characters in the Unity environment. Mecanim support is also provided.

For more information on how to use the runtimes, please head over to this [site](http://www.kestrelmoon.com/creaturedocs/Game_Engine_Runtimes_And_Integration/Runtimes_Introduction.html)

## Deploying for iOS and other platforms
If you are having issues with deploying for platforms like iOS ( **UnusedBytecodeStripper2.exe did not run properly etc.** ), you should give the following a go:

1) Remove the **CreatureFlatData.dll** from the **Distro** directory

2) Create a new folder in your project and include the files:

[Creature FlatBuffers Source](https://github.com/kestrelm/Creature_Unity/tree/master/FlatBuffersCSharp)

3) Build and deploy your project

This should remove any sort of DLL dependency on your project.

## Creature 2D Side-Scrolling Platformer Demo

This is a simple 2D Side-Scrolling Demo authored in Creature and Unity. Characters are animated with the Creature 2D Animation Tool and brought to life in the Unity Game Engine using the Creature Unity Plugin.

[![Non](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/unity_platformer.png)](https://youtu.be/4UXp4-L6YEE)

Download the full Unity source + assets of the demo [here.](https://github.com/kestrelm/CreatureDemos/tree/master/CreaturePlatformer)

## Creature Unity Game UI Demo

This is a demonstration of multiple Creature Animated characters playing in a reconstructed Game Starting Menu UI scene. The Reaper character and even the buttons are all authored and animated from within Creature.

[![Non](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/ui-screen.png)](https://youtu.be/XkJa1VzWrc8)

Video of the Demo is [here.](https://youtu.be/XkJa1VzWrc8)

Download the full Unity source + assets of the demo [here.](https://github.com/kestrelm/CreatureDemos/tree/master/CreatureUI)

####Artwork Credits
Reaper: Katarzyna Zalecka. CC-BY-SA 3.0, [Ancient Beast](https://github.com/FreezingMoon/AncientBeast)

Background: David Revoy, CC BY 3.0

Buttton Frame: Ironthunder, CC-BY 3.0

## Creature Unity Kraken Demo

[![Non](http://www.kestrelmoon.com/creature/img/kraken.png)](http://youtu.be/Ca6lRx0_fF8)

This is a demonstration of the Creature Runtimes in Unity. The Kraken and Orca characters are authored in Creature using Procedural Motors.
Kraken character drives a full blown Grid-based Fluid Simulation in 
the scene,  affecting 10000 fluid particles in realtime.

The Kraken Demo Project is located in the directory **"Kraken Demo"**.

Controls:

**a - Move Left**

**d - Move Right**

**w - Move Up**

**s - Move Down**

