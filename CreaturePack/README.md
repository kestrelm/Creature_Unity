# Creature Pack Runtimes

This document describes the **Creature Pack Runtimes/Plugin** for the **Unity Game Engine**. It shows you how to install and setup a character for playback authored using the [Creature Animation Tool](http://creature.kestrelmoon.com/) in the **Unity** environment.

## Hummingbirds! ( Creature Pack Demo )

![Non](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/hummerShort2.gif)

Hummingbirds! is a demo done using the **Creature Pack Plugin**. It showcases an entire scene ( flowers, birds ) all animated in Creature and played back in Unity using the plugin in real-time.

**Live WebGL Demo**: <http://creature.kestrelmoon.com/WebDemo/BirdPacksDemo/>

**Demo Trailer**: <https://youtu.be/NpaTAHtHU_E>

## Creature Pack vs Regular Creature JSON/Flatbuffers

**Creature Pack** is a lightweight version of the original **Creature JSON/Flatbuffers** plugin. It has **MecAnim** support as well as an API that allows you to directly switch animation clips via C# scripting.

**Advantages**:
- Pure point cache, very high performance since no Bone Posing is done 
- Lighweight, less files, less setup cost
- Very useful for animating a TON of characters in Crowds situations

**Disadvantages**:
- Has no bones, gameplay that requires bone interactions will need the original plugin
- Has less gameplay features ( no GameController etc. )
- Less power/flexibility compared to the original plugin

## Step 1: Setup

First, make a **CreaturePack** folder in your Unity assets folder. Then, drag the **Distro** and **Editor** from the **CreaturePack** directory ( <https://github.com/kestrelm/Creature_Unity/tree/master/CreaturePack> ) into your **CreaturePack** folder:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen1.png)

You should now have the **CreaturePack** menu items enabled in Unity. Check to see this is the case:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen2.png)

## Step 2: Export CreaturePack File

Go back into Creature, open up your character. Go into **Animate** mode, click on **Export -> Game Engines**:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen3.png)

There are a number of options that deal with the **CreaturePack** file format:

- **Gap Step**: This determines the quality of the export. Gap Step is the gap between frames the export process will sample from. The **higher the gap step**, the **smaller the file size** since the export will sample from **fewer frames**. However, this will also reduce quality. Play around with this value to find the optimal settings ( recommended 2 to 5 )

- **Pack UV Swaps**: If you are using Sprite Swapping for your character, check this option. Save on file sizes/disable sprite swapping by unchecking this option.

- **Pack Colors**: If you are animating opacity for your character, check this option. Save on file sizes/disable opacity animation by unchecking this option.

So proceed to export your character into a destination folder. There will be a couple of files in the folder:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen4.png)

1) **NOTE!** There will already be a file with the extension of **.bytes**. This is the  **Flatbuffers** file built for the original plugin. If you are using CreaturePack, you should move or delete this file since it is not needed. 

2) After performing 1), **RENAME** the **.creature_pack** file INTO a **.bytes** extension. This is needed because Unity expects all binary files to have a **.bytes** extension. (**IMPORTANT!**)

3) Drag the newly renamed **.bytes** file into Unity's asset folder.

### Step 3: Create CreaturePackAsset

Go to **CreaturePack -> CreaturePackAsset** and create the **CreaturePackAsset**. Drag the **.bytes** asset file into the **Creature Pack Bytes** slot:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen5.png)

The asset object should populate and show all the available animation clips for that character along with their associated frame ranges.

### Step 4: Create CreaturePackRenderer

Go to **CreaturePack -> CreaturePackRenderer** and create the **CreaturePackRenderer**. Drag the created **CreaturePackAsset** into the **Pack_asset** slot of the new renderer:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen6.png)

### Step 5: Import Texture & View Character

Now from the exported character directory ( from Creature ), import in the **.png** texture atlas of your character into Unity. Drag the texture into your **CreaturePackRenderer** as a new material component. A particle material with the blended option works. You should now be able to view your character in Unity:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen7.png)

Pressing play on the game should now play the character!

## Using MecAnim

**CreaturePack** supports the construction of **MecAnim** animation state machines. You do this by clicking on the **Build State Machine** button in the **CreaturePackAsset** object.

The state machine is created in your **Assets** folder. Double click on the newly created **State Machine** asset to open it up:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen8.png)

From the screen above, you can setup your state machine transition modes to control the switching of animation.

After that is done, drag the **State Machine** asset as a component into your **CreaturePackRenderer** to activate it:

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/screen9.png)

## Controlling Animation with C# Scripting

You can also control the switching of animations via C# scripting. Have a look at the **Hummingbirds!** demo to see how this is accomplished: ( <https://github.com/kestrelm/CreatureDemos/tree/master/BirdsPackDemo> )

The file to examine is this: <https://github.com/kestrelm/CreatureDemos/blob/master/BirdsPackDemo/Assets/Scripts/HummerScript.cs>

The standard steps involve first getting a reference to the **CreaturePackRenderer**. The **CreaturePackRenderer** has a property called **pack_player**. You can control the switching of animation clips via this property.

It has the following functions you care about:

-**blendToAnimation(string nameIn, float blendDelta)** This smoothly transitions to a target animation with a given blendDelta. This value is > 0 and <= 1.0. The higher the value, the faster the blending

-**setActiveAnimation(string nameIn)** This switches immediately to the target animation


