# Creature Pack Runtimes

This document describes the **Creature Pack Runtimes/Plugin** for the **Unity Game Engine**. It shows you how to install and setup a character for playback authored using the [Creature Animation Tool](http://creature.kestrelmoon.com/) in the **Unity** environment.

## Hummingbirds! ( Creature Pack Demo )

![Non](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/hummerShort2.gif)

Hummingbirds! is a demo done using the **Creature Pack Plugin**. It showcases an entire scene ( flowers, birds ) all animated in Creature and played back in Unity using the plugin in real-time.

**Live WebGL Demo**: <http://creature.kestrelmoon.com/WebDemo/BirdPacksDemo/>

**Demo Trailer**: <https://youtu.be/NpaTAHtHU_E>

**Tutorial Video**: <https://youtu.be/jCU1c_9peFE>

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

## Super Small Animations with Delta Compression

**Creature** supports a **Delta Compression** scheme which enables you to compress your animations even further. Typical sizes of complex mesh animations < 1MB are common using this scheme. During **Game Engine Export**, there is a **Delta Compress** option in the export window:

-**0x** No compression

-**2x** 2x compression. This is for typical usage.

-**4x** 4x compression. There might be some artifacts for certain animations so experiment a bit with this to see if it is suitable for your character.

For more information on how to use it, please check out the tutorial video [here](https://youtu.be/MuYe_Wh5mnc)

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

## Speeding up Load times/Reducing Memory cost with CreaturePack Split Files

CreaturePack was designed for performance and memory efficiency in mind. If you require even faster load times or need to tweak memory efficiency further, consider using the **CreaturePack Split Files** format. 

This format follows the **LWYN ( Load What You Need)** principle. You only need to load whatever animations you need for your character in a given situation. To illustrate this point, after you run **Game Engine Export** exporting your character into a designated export folder, there will be a subfolder called **packMdlAnim:**

![](https://raw.githubusercontent.com/kestrelm/Creature_Unity/master/CreaturePack/CreaturePackSplit1.png)

Types of files listed in this sub-folder:

- **.mdl** This is the model file of the character, contains just the base character model
- **.anim** These are the individual animation files of the character

As you can see, this allows you to pick and choose whichever animations you require for the character during Game Engine runtime. For the case of Unity, please remember to **rename the .mdl and .anim** files to a **.bytes** extension before dragging them into the Unity asset folder.

### Using CreaturePack Split Files

1. First, you need to create the standard **CreaturePack Asset** object. Make it use the **.mdl** model ( drag it into the asset slot )

2. Setup your standard **CreaturePack Renderer** object and drag the **CreaturePack Asset** into the slot specified in the renderer

3. Create a number of **CreaturePack SplitAnimAsset** objects ( the same number as the animations you have ). You probably want to name them the same name as your intended animations. Drag each **.anim** asset into the slot of each of your created objects.

If you play the game now, you should see your character running with no animation ( static frame ).

Next, you want to dynamically load the character during runtime with C# Gameplay code. There are 2 APIs available:

-**addSplitAnimClip(Stream byteStream, CreaturePackPlayer playerIn)** Adds in a new animation clip from a CreaturePack Anim Data byte stream

-**removeSplitAnimClip(string animName, Stream byteStream, CreaturePackPlayer playerIn)** Removes the designated animation clip from the list of loaded animation clips

Here is an example usage, we have created a simple C# Script that is attached to the **CreaturePack Renderer** and will dynamically load an animation when the character starts in the game. We have created a couple of convenient slots for the **CreaturePack SplitAnimAsset** objects we created above. We also assigned them in the editor so they are easily referenced in the code:
```
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using CreaturePackModule;

public class SplitScript : MonoBehaviour {
    public CreaturePackSplitAnimAsset idleAsset = null;
    public CreaturePackSplitAnimAsset fireAsset = null;
    public CreaturePackSplitAnimAsset walkAsset = null;
    public CreaturePackSplitAnimAsset reverseAsset = null;
    int cnt = 0;

    // Use this for initialization
    void Start () {
        
        CreaturePackRenderer cpackRender = gameObject.GetComponent(typeof(CreaturePackRenderer)) as CreaturePackRenderer;
        CreaturePackAsset cAsset = cpackRender.pack_asset;
        CreaturePackLoader cLoader = cAsset.GetCreaturePackLoader();
        CreaturePackPlayer cPlayer = cpackRender.pack_player;

        // Load in split animations
        Debug.Log("Loading Split Animations...");
        List<CreaturePackSplitAnimAsset> loadAssets = new List<CreaturePackSplitAnimAsset>();
        loadAssets.Add(idleAsset);
        loadAssets.Add(fireAsset);
        loadAssets.Add(walkAsset);
        //loadAssets.Add(reverseAsset);

        foreach (var curData in loadAssets)
        {
            Debug.Log("Loading Split Animation: " + curData.name);
            Stream s = new MemoryStream(curData.creaturePackSplitAnimBytes.bytes);
            cLoader.addSplitAnimClip(s, cPlayer);
        }

        cPlayer.setActiveAnimation("ground_walk");
    }

    // Update is called once per frame
    void Update () {
        cnt++;
        if(cnt == 200)
        {
            CreaturePackRenderer cpackRender = gameObject.GetComponent(typeof(CreaturePackRenderer)) as CreaturePackRenderer;
            CreaturePackPlayer cPlayer = cpackRender.pack_player;
            cPlayer.setActiveAnimation("ground_fire");
        }
    }
}
```
In the above example, we are getting a reference to the required loader and player objects, then crucially loading the clips when the character first starts up:

```
foreach (var curData in loadAssets)
{
    Debug.Log("Loading Split Animation: " + curData.name);
    Stream s = new MemoryStream(curData.creaturePackSplitAnimBytes.bytes);
    cLoader.addSplitAnimClip(s, cPlayer);
}
```

After that, everything functions as per normal. You can switch to a new animation etc. in your Gameplay code for your character.

